using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SalesApi.WebApi.Controllers.V1;
using SalesApi.ViewModel.V1.Sales;
using Xunit;
using Microsoft.Extensions.Logging;

namespace SalesApi.Tests.WebApi;

public class SalesControllerTests
{
    private readonly SalesController _controller;
    private readonly ILogger<SalesController> _logger;

    public SalesControllerTests()
    {
        _logger = Substitute.For<ILogger<SalesController>>();
        _controller = new SalesController(_logger);
    }

    [Fact]
    public async Task Create_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new SaleViewModel.Request
        {
            ProductId = Guid.NewGuid(),
            Quantity = 1,
            CustomerId = Guid.NewGuid()
        };

        // Act
        var result = await _controller.Create(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SaleViewModel.CreateResponse>(okResult.Value);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithEmptyList()
    {
        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SaleViewModel.ListResponse>(okResult.Value);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task Cancel_WithValidId_ReturnsOk()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _controller.Cancel(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SaleViewModel.CancelResponse>(okResult.Value);
        Assert.NotNull(response);
    }
} 