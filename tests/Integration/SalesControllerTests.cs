using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;
using FluentAssertions;
using SalesApi.ViewModel.V1.Products;
using SalesApi.ViewModel.V1.Sales;

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

    [TestCaseOrderer("SalesApi.IntegrationTests.CustomTestOrderer", "SalesApi.IntegrationTests")]
    public class SalesApiIntegrationTests
    {
        private readonly HttpClient _client;

        public SalesApiIntegrationTests()
        {
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:7777/api/v1") };
            //_client = new HttpClient { BaseAddress = new Uri("http://localhost:53348/api/v1") };
        }

        [Fact, TestPriority(0)]
        public async Task Create_Products()
        {
            for (int i = 1; i <= 10; i++)
            {
                var newProduct = new ProductViewModel.Request
                {
                    Title = $"titulo do produto - {i}",
                    Price = 5,
                    Description = $"Descricao do titulo {i}",
                    Category = $"Categoria {(int)i / 2}",
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
            var productsResponse = JsonConvert.DeserializeObject<List<ProductViewModel.Response>>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.True(productsResponse.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var produto1 = productsResponse[1]; // Segundo produto da lista
            var produto2 = productsResponse[0]; // Primeiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new SaleViewModel.Request
            {
                CustomerName = "Test Customer",
                CustomerEmail = "test@email.com",
                Items = new List<SaleItemViewModel>
                {
                    new()
                    {
                        ProductId = produto1.Id,
                        Quantity = 3,
                        UnitPrice = produto1.Price,
                    },
                    new()
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
            var productsResponse = JsonConvert.DeserializeObject<List<ProductViewModel.Response>>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.True(productsResponse.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var produto1 = productsResponse[1]; // Segundo produto da lista
            var produto2 = productsResponse[0]; // Primeiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new SaleViewModel.Request
            {
                CustomerName = "Test Customer",
                CustomerEmail = "test@email.com",
                Items = new List<SaleItemViewModel>
                {
                    new()
                    {
                        ProductId = produto1.Id,
                        Quantity = 3,
                        UnitPrice = produto1.Price,
                    },
                    new()
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
            var productsResponse = JsonConvert.DeserializeObject<List<ProductViewModel.Response>>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.True(productsResponse.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var produto1 = productsResponse[1]; // Segundo produto da lista
            var produto2 = productsResponse[0]; // Primeiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new SaleViewModel.Request
            {
                CustomerName = "Test Customer",
                CustomerEmail = "test@email.com",
                Items = new List<SaleItemViewModel>
                {
                    new()
                    {
                        ProductId = produto1.Id,
                        Quantity = 9,
                        UnitPrice = produto1.Price,
                    },
                    new()
                    {
                        ProductId = produto2.Id,
                        Quantity = 11,
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
            var productsResponse = JsonConvert.DeserializeObject<List<ProductViewModel.Response>>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.True(productsResponse.Count >= 3, "O endpoint deve retornar pelo menos 3 produtos.");

            // 2. Selecionar três produtos
            var produto1 = productsResponse[1]; // Segundo produto da lista
            var produto2 = productsResponse[0]; // Primeiro produto da lista
            var produto3 = productsResponse[2]; // Terceiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new SaleViewModel.Request
            {
                CustomerName = "Test Customer",
                CustomerEmail = "test@email.com",
                Items = new List<SaleItemViewModel>
                {
                    new()
                    {
                        ProductId = produto1.Id,
                        Quantity = 9,
                        UnitPrice = produto1.Price,
                    },
                    new()
                    {
                        ProductId = produto2.Id,
                        Quantity = 11,
                        UnitPrice = produto2.Price,
                    },
                    new()
                    {
                        ProductId = produto3.Id,
                        Quantity = 20,
                        UnitPrice = produto3.Price,
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
            var productsResponse = JsonConvert.DeserializeObject<List<ProductViewModel.Response>>(jsonResponse);

            Assert.NotNull(productsResponse);
            Assert.True(productsResponse.Count >= 2, "O endpoint deve retornar pelo menos 2 produtos.");

            // 2. Selecionar dois produtos
            var produto1 = productsResponse[1]; // Segundo produto da lista
            var produto2 = productsResponse[0]; // Primeiro produto da lista

            // 3. Criar a nova venda
            var novaVenda = new SaleViewModel.Request
            {
                CustomerName = "Test Customer",
                CustomerEmail = "test@email.com",
                Items = new List<SaleItemViewModel>
                {
                    new()
                    {
                        ProductId = produto1.Id,
                        Quantity = 9,
                        UnitPrice = produto1.Price,
                    },
                    new()
                    {
                        ProductId = produto2.Id,
                        Quantity = 25,
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
                response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
                result.Should().Contain("You can buy only 20 pieces of a item");
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
            // 1. Buscar vendas do endpoint
            var responseSales = await _client.GetAsync("/sales");
            responseSales.EnsureSuccessStatusCode();

            var jsonResponse = await responseSales.Content.ReadAsStringAsync();
            var salesResponse = JsonConvert.DeserializeObject<List<SaleViewModel.Response>>(jsonResponse);

            Assert.NotNull(salesResponse);
            Assert.True(salesResponse.Count >= 1, "O endpoint deve retornar pelo menos 1 venda.");

            // 2. Selecionar uma venda
            var sale = salesResponse[0];

            var response = await _client.DeleteAsync($"/sales/{sale.Id}");

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            result.Should().ContainEquivalentOf("cancel");
        }
    }
}
