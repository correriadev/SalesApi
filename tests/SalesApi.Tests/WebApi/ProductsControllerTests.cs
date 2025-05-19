using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SalesApi.WebApi.Controllers.V1;
using SalesApi.ViewModel.V1.Products;
using Xunit;
using Microsoft.Extensions.Logging;

namespace SalesApi.Tests.WebApi;

public class ProductsControllerTests
{
    private readonly ProductsController _controller;
    private readonly ILogger<ProductsController> _logger;

    public ProductsControllerTests()
    {
        _logger = Substitute.For<ILogger<ProductsController>>();
        _controller = new ProductsController(_logger);
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsAccepted()
    {
        // Arrange
        var request = new ProductViewModel.Request
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var acceptedResult = Assert.IsType<AcceptedResult>(result);
        Assert.Equal(202, acceptedResult.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithEmptyList()
    {
        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ProductViewModel.ListResponse>(okResult.Value);
        Assert.NotNull(response);
    }
} 