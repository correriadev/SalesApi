using AutoMapper;
using NSubstitute;
using SalesApi.Application.Products.Queries.GetAllProducts;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Repositories;
using SalesApi.ViewModel.V1.Products;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SalesApi.Tests.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SalesApi.Infrastructure.Mappings.MappingProfile>();
        });
        _mapper = configuration.CreateMapper();
        _handler = new GetAllProductsQueryHandler(_productRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product("Product 1", SalesApi.Domain.ValueObjects.Money.FromDecimal(10), "Desc 1", "Cat 1", "img1"),
            new Product("Product 2", SalesApi.Domain.ValueObjects.Money.FromDecimal(20), "Desc 2", "Cat 2", "img2")
        };
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(products);

        // Act
        var result = (await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Product 1", result[0].Title);
        Assert.Equal("Product 2", result[1].Title);
    }

    [Fact]
    public async Task Handle_EmptyList_ShouldReturnEmpty()
    {
        // Arrange
        _productRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Product>());

        // Act
        var result = await _handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
} 