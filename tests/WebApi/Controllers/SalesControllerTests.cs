using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using SalesApi.Application.Sales.Commands.CreateSale;
using SalesApi.Application.Sales.Queries.GetAllSales;
using SalesApi.ViewModel.V1.Common;
using SalesApi.ViewModel.V1.Sales;
using SalesApi.WebApi.Controllers.V1;
using Xunit;

namespace SalesApi.Tests.WebApi.Controllers;

public class SalesControllerTests
{
    private readonly IMediator _mediator;
    private readonly SalesController _controller;

    public SalesControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _controller = new SalesController(_mediator);
    }

    [Fact]
    public async Task CreateSale_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new SaleViewModel.Request
        {
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            Items = new List<SaleItemViewModel>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 2,
                    UnitPrice = 10.99m,
                    Discount = 0
                }
            }
        };

        var response = new SaleViewModel.Response
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            TotalAmount = 21.98m,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            Items = request.Items
        };

        _mediator.Send(Arg.Any<CreateSaleCommand>(), Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _controller.CreateSale(request);

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<SaleViewModel.Response>>>(result);
        var apiResponse = Assert.IsType<OkObjectResult>(okResult.Result).Value as ApiResponse<SaleViewModel.Response>;
        Assert.NotNull(apiResponse);
        Assert.Equal("success", apiResponse.Status);
        Assert.Equal(response, apiResponse.Data);
        Assert.Equal("Sale successfully created", apiResponse.Message);
    }

    [Fact]
    public async Task GetAllSales_ReturnsOkResult()
    {
        // Arrange
        var sales = new List<SaleViewModel.Response>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CustomerName = "Test Customer",
                CustomerEmail = "test@example.com",
                TotalAmount = 21.98m,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Items = new List<SaleItemViewModel>()
            }
        };

        _mediator.Send(Arg.Any<GetAllSalesQuery>(), Arg.Any<CancellationToken>())
            .Returns(sales);

        // Act
        var result = await _controller.GetAllSales();

        // Assert
        var okResult = Assert.IsType<ActionResult<ApiResponse<IEnumerable<SaleViewModel.Response>>>>(result);
        var apiResponse = Assert.IsType<OkObjectResult>(okResult.Result).Value as ApiResponse<IEnumerable<SaleViewModel.Response>>;
        Assert.NotNull(apiResponse);
        Assert.Equal("success", apiResponse.Status);
        Assert.Equal(sales, apiResponse.Data);
        Assert.Equal("Sales retrieved successfully", apiResponse.Message);
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