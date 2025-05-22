using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SalesApi.Application.Products.Commands.CreateProduct;
using SalesApi.Application.Products.Queries.GetAllProducts;
using SalesApi.WebApi.Controllers.V1;
using SalesApi.ViewModel.V1.Products;
using Xunit;
using MediatR;

namespace SalesApi.Tests.WebApi;

public class ProductsControllerTests
{
    private readonly IMediator _mediator;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new ProductsController(_mediator);
    }

    [Fact]
    public async Task Create_WithValidRequest_ShouldReturnCreatedResult()
    {
        // Arrange
        var request = new ProductViewModel.Request
        {
            Title = "Test Product",
            Description = "Test Description",
            Price = 99.99m
        };

        var response = new ProductViewModel.Response
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Price = request.Price
        };

        _mediator.Send(Arg.Any<CreateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(response, createdResult.Value);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult()
    {
        // Arrange
        var products = new List<ProductViewModel.Response>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Test Product",
                Description = "Test Description",
                Price = 99.99m
            }
        };

        _mediator.Send(Arg.Any<GetAllProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(products, okResult.Value);
    }
} 