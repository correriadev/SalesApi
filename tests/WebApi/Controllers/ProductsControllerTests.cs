using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SalesApi.Application.Products.Commands.CreateProduct;
using SalesApi.Application.Products.Queries.GetAllProducts;
using SalesApi.ViewModel.V1.Common;
using SalesApi.ViewModel.V1.Products;
using SalesApi.WebApi.Controllers.V1;
using Xunit;

namespace SalesApi.Tests.WebApi.Controllers;

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
    public async Task CreateProduct_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new ProductViewModel.Request
        {
            Title = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test Category",
            Image = "test.jpg"
        };

        var response = new ProductViewModel.Response
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            Image = request.Image
        };

        _mediator.Send(Arg.Any<CreateProductCommand>(), Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _controller.CreateProduct(request);

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<ProductViewModel.Response>>>(result);
        var apiResponse = Assert.IsType<OkObjectResult>(okResult.Result).Value as ApiResponse<ProductViewModel.Response>;
        Assert.NotNull(apiResponse);
        Assert.Equal("success", apiResponse.Status);
        Assert.Equal(response, apiResponse.Data);
        Assert.Equal("Product successfully created", apiResponse.Message);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsOkResult()
    {
        // Arrange
        var products = new List<ProductViewModel.Response>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Test Product 1",
                Description = "Test Description 1",
                Price = 99.99m,
                Category = "Test Category 1",
                Image = "test1.jpg"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Test Product 2",
                Description = "Test Description 2",
                Price = 149.99m,
                Category = "Test Category 2",
                Image = "test2.jpg"
            }
        };

        _mediator.Send(Arg.Any<GetAllProductsQuery>(), Arg.Any<CancellationToken>())
            .Returns(products);

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<IEnumerable<ProductViewModel.Response>>>>(result);
        var apiResponse = Assert.IsType<OkObjectResult>(okResult.Result).Value as ApiResponse<IEnumerable<ProductViewModel.Response>>;
        Assert.NotNull(apiResponse);
        Assert.Equal("success", apiResponse.Status);
        Assert.Equal(products, apiResponse.Data);
        Assert.Equal("Products retrieved successfully", apiResponse.Message);
    }
} 