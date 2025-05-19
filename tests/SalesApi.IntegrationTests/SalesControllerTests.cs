using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using FluentAssertions;
using System.Reflection.Metadata.Ecma335;
using System.Xml;

namespace SalesApi.IntegrationTests
{
    public class TestPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public TestPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }

    public class ProductResponse
    {
        public List<ProductData> Data { get; set; } = new();
    }

    public class ProductData
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
    }

    public class SalesResponse
    {
        public List<SaleData> Data { get; set; } = new();
    }

    public class SaleData
    {
        public Guid Id { get; set; }
    }

    [TestCaseOrderer("SalesApi.IntegrationTests.CustomTestOrderer", "SalesApi.IntegrationTests")]
    public class SalesApiIntegrationTests
    {
        private readonly HttpClient _client;

        public SalesApiIntegrationTests()
        {
            _client = new HttpClient { BaseAddress = new Uri("http://ocelot-gateway:7777/api/v1") };
            //_client = new HttpClient { BaseAddress = new Uri("http://localhost:7777") };
        }

        [Fact, TestPriority(0)]
        //[Fact(Skip = "Este teste está desativado temporariamente.")]
        public async Task Create_Products()
        {
            for (int i = 1; i <= 10; i++)
            {
                var newProduct = new
                {
                    title = $"titulo do produto - {i}",
                    price = 5,
                    description = $"Descricao do titulo {i}",
                    category = $"Categoria {(int)i / 2}",
                    Image = "imagem"
                };
                var json = JsonConvert.SerializeObject(newProduct);
                Console.WriteLine("Enviando JSON: " + json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("/products", content);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail($"[ERRO] Requisição falhou!\n" +
                                $"Response: {result}");
                }
            }
        }

        [Fact, TestPriority(2)]
        public async Task Should_Create_Sale_With_NoDiscount()
        {
            // 1. Buscar produtos do endpoint
            var responseProducts = await _client.GetAsync("/products");
            responseProducts.EnsureSuccessStatusCode();

            var jsonResponse = await responseProducts.Content.ReadAsStringAsync();
            var productsResponse = JsonConvert.DeserializeObject<ProductResponse>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.NotNull(productsResponse.Data);
            Assert.True(productsResponse.Data.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var produto1 = productsResponse.Data[1]; // Segundo produto da lista
            var produto2 = productsResponse.Data[0]; // Primeiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new
            {
                SaleNumber = new Random().Next(1000, 9999).ToString(), // Número aleatório para a venda
                SaleDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Data no formato ISO 8601
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new List<object>
                {
                    new
                    {
                        ProductId = produto1.Id,
                        Quantity = 3,
                        UnitPrice = produto1.Price,
                    },
                    new
                    {
                        ProductId = produto2.Id,
                        Quantity = 2,
                        UnitPrice = produto2.Price,
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(novaVenda), Encoding.UTF8, "application/json");

            // 4. Enviar a requisição para criar a venda
            var response = await _client.PostAsync("/sales", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            result.Should().Contain("\"totalAmount\":25");
        }

        [Fact, TestPriority(3)]
        public async Task Should_Create_Sale_With_10PercentDiscount()
        {
            // 1. Buscar produtos do endpoint
            var responseProducts = await _client.GetAsync("/products");
            responseProducts.EnsureSuccessStatusCode();

            var jsonResponse = await responseProducts.Content.ReadAsStringAsync();
            var productsResponse = JsonConvert.DeserializeObject<ProductResponse>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.NotNull(productsResponse.Data);
            Assert.True(productsResponse.Data.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var produto1 = productsResponse.Data[1]; // Segundo produto da lista
            var produto2 = productsResponse.Data[0]; // Primeiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new
            {
                SaleNumber = new Random().Next(1000, 9999).ToString(), // Número aleatório para a venda
                SaleDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Data no formato ISO 8601
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new List<object>
                {
                    new
                    {
                        ProductId = produto1.Id,
                        Quantity = 3,
                        UnitPrice = produto1.Price,
                    },
                    new
                    {
                        ProductId = produto2.Id,
                        Quantity = 5,
                        UnitPrice = produto2.Price,
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(novaVenda), Encoding.UTF8, "application/json");

            // 4. Enviar a requisição para criar a venda
            var response = await _client.PostAsync("/sales", content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Assert.Fail($"[ERRO] Requisição falhou!\n" +
                            $"Response: {result}");
                response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            }

            response.EnsureSuccessStatusCode();
            Assert.NotNull(result);
            result.Should().Contain("\"discount\":2.5");
            result.Should().Contain("22.5");
        }

        [Fact, TestPriority(4)]
        public async Task Should_Create_Sale_With_10PercentDiscount_20Prod()
        {
            // 1. Buscar produtos do endpoint
            var responseProducts = await _client.GetAsync("/products");
            responseProducts.EnsureSuccessStatusCode();

            var jsonResponse = await responseProducts.Content.ReadAsStringAsync();
            var productsResponse = JsonConvert.DeserializeObject<ProductResponse>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.NotNull(productsResponse.Data);
            Assert.True(productsResponse.Data.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var produto1 = productsResponse.Data[1]; // Segundo produto da lista
            var produto2 = productsResponse.Data[0]; // Primeiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new
            {
                SaleNumber = new Random().Next(1000, 9999).ToString(), // Número aleatório para a venda
                SaleDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Data no formato ISO 8601
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new List<object>
                {
                    new
                    {
                        ProductId = produto1.Id,
                        Quantity = 9,
                        UnitPrice = produto1.Price,
                        TotalPrice = 9 * produto1.Price
                    },
                    new
                    {
                        ProductId = produto2.Id,
                        Quantity = 11,
                        UnitPrice = produto2.Price,
                        TotalPrice = 11 * produto1.Price
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(novaVenda), Encoding.UTF8, "application/json");

            // 4. Enviar a requisição para criar a venda
            var response = await _client.PostAsync("/sales", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            result.Should().Contain("\"discount\":4.5");
            result.Should().Contain("\"total\":40.5");
            result.Should().Contain("\"discount\":11");
            result.Should().Contain("\"total\":44.0");
            result.Should().Contain("84.5");
        }

        [Fact, TestPriority(5)]
        public async Task Should_Apply_20Percent_Discount_For_10_To_20_Items()
        {
            // 1. Buscar produtos do endpoint
            var responseProducts = await _client.GetAsync("/products");
            responseProducts.EnsureSuccessStatusCode();

            var jsonResponse = await responseProducts.Content.ReadAsStringAsync();
            var productsResponse = JsonConvert.DeserializeObject<ProductResponse>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.NotNull(productsResponse.Data);
            Assert.True(productsResponse.Data.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var produto1 = productsResponse.Data[1]; // Segundo produto da lista
            var produto2 = productsResponse.Data[0]; // Primeiro produto da lista
            var produto3 = productsResponse.Data[2]; // Primeiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new
            {
                SaleNumber = new Random().Next(1000, 9999).ToString(), // Número aleatório para a venda
                SaleDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Data no formato ISO 8601
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new List<object>
                {
                    new
                    {
                        ProductId = produto1.Id,
                        Quantity = 9,
                        UnitPrice = produto1.Price,
                        TotalPrice = 9 * produto1.Price
                    },
                    new
                    {
                        ProductId = produto2.Id,
                        Quantity = 11,
                        UnitPrice = produto2.Price,
                        TotalPrice = 11 * produto1.Price
                    },
                    new
                    {
                        ProductId = produto3.Id,
                        Quantity = 20,
                        UnitPrice = produto3.Price,
                        TotalPrice = 20 * produto1.Price
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(novaVenda), Encoding.UTF8, "application/json");

            // 4. Enviar a requisição para criar a venda
            var response = await _client.PostAsync("/sales", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
            result.Should().Contain("\"discount\":4.5");
            result.Should().Contain("\"total\":40.5");
            result.Should().Contain("\"discount\":11");
            result.Should().Contain("\"total\":44.0");
            result.Should().Contain("\"discount\":20");
            result.Should().Contain("\"total\":80.0");
            result.Should().Contain("164.5");
        }

        [Fact, TestPriority(6)]
        public async Task Should_Not_Allow_More_Than_20_Items()
        {
            // 1. Buscar produtos do endpoint
            var responseProducts = await _client.GetAsync("/products");
            responseProducts.EnsureSuccessStatusCode();

            var jsonResponse = await responseProducts.Content.ReadAsStringAsync();
            var productsResponse = JsonConvert.DeserializeObject<ProductResponse>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.NotNull(productsResponse.Data);
            Assert.True(productsResponse.Data.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var produto1 = productsResponse.Data[1]; // Segundo produto da lista
            var produto2 = productsResponse.Data[0]; // Primeiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new
            {
                SaleNumber = new Random().Next(1000, 9999).ToString(), // Número aleatório para a venda
                SaleDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), // Data no formato ISO 8601
                CustomerId = Guid.NewGuid(),
                BranchId = Guid.NewGuid(),
                Items = new List<object>
                {
                    new
                    {
                        ProductId = produto1.Id,
                        Quantity = 9,
                        UnitPrice = produto1.Price,
                    },
                    new
                    {
                        ProductId = produto2.Id,
                        Quantity = 25,
                        UnitPrice = produto2.Price,
                    }
                }
            };

            var requestJson = JsonConvert.SerializeObject(novaVenda, Newtonsoft.Json.Formatting.Indented);
            var content = new StringContent(JsonConvert.SerializeObject(novaVenda), Encoding.UTF8, "application/json");

            // 4. Enviar a requisição para criar a venda
            var response = await _client.PostAsync("/sales", content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Assert.Fail($"[ERRO] Requisição falhou!\n" +
                            $"Response: {result}");
                response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
                result.Should().Contain("You can buy only 20 pices of a item");
            }
            else
            {
                Assert.NotNull(result);
                result.Should().Contain("It's not possible to sell");
            }
        }

        [Fact, TestPriority(10)]
        public async Task Should_Cancel_Sale()
        {
            // 1. Buscar produtos do endpoint
            var responseProducts = await _client.GetAsync("/sales");
            responseProducts.EnsureSuccessStatusCode();

            var jsonResponse = await responseProducts.Content.ReadAsStringAsync();
            var salesResponse = JsonConvert.DeserializeObject<SalesResponse>(jsonResponse);

            Assert.NotNull(salesResponse);
            Assert.NotNull(salesResponse.Data);
            Assert.True(salesResponse.Data.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var sales = salesResponse.Data[0]; // Segundo produto da lista

            var saleId = sales.Id;
            var response = await _client.DeleteAsync($"/sales/{saleId}");

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            result.Should().ContainEquivalentOf("cancel");
        }
    }
}
