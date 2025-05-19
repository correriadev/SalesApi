using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using SalesApi.Application.Products.Queries.GetAllProducts;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Repositories;
using SalesApi.Domain.ValueObjects;
using SalesApi.ViewModel.V1.Products;
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
        _mapper = Substitute.For<IMapper>();
        _handler = new GetAllProductsQueryHandler(_productRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product("Test Product 1", Money.FromDecimal(10.99m), "Description 1", "Category 1", "image1.jpg"),
            new Product("Test Product 2", Money.FromDecimal(20.99m), "Description 2", "Category 2", "image2.jpg")
        };

        var expectedResponses = new List<ProductViewModel.Response>
        {
            new() { Title = "Test Product 1", Description = "Description 1", Price = 10.99m, Category = "Category 1", Image = "image1.jpg" },
            new() { Title = "Test Product 2", Description = "Description 2", Price = 20.99m, Category = "Category 2", Image = "image2.jpg" }
        };

        _productRepository.GetAllAsync().Returns(products);
        _mapper.Map<IEnumerable<ProductViewModel.Response>>(products).Returns(expectedResponses);

        var query = new GetAllProductsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResponses);
        await _productRepository.Received(1).GetAllAsync();
        _mapper.Received(1).Map<IEnumerable<ProductViewModel.Response>>(products);
    }

    [Fact]
    public async Task Handle_WhenNoProducts_ShouldReturnEmptyList()
    {
        // Arrange
        var products = new List<Product>();
        var expectedResponses = new List<ProductViewModel.Response>();

        _productRepository.GetAllAsync().Returns(products);
        _mapper.Map<IEnumerable<ProductViewModel.Response>>(products).Returns(expectedResponses);

        var query = new GetAllProductsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        await _productRepository.Received(1).GetAllAsync();
        _mapper.Received(1).Map<IEnumerable<ProductViewModel.Response>>(products);
    }
} 