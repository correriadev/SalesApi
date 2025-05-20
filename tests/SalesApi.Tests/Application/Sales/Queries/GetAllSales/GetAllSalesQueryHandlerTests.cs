using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using SalesApi.Application.Sales.Queries.GetAllSales;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Repositories;
using SalesApi.Domain.ValueObjects;
using SalesApi.ViewModel.V1.Sales;
using Xunit;

namespace SalesApi.Tests.Application.Sales.Queries.GetAllSales;

public class GetAllSalesQueryHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetAllSalesQueryHandler _handler;

    public GetAllSalesQueryHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetAllSalesQueryHandler(_saleRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedSales()
    {
        // Arrange
        var customerId1 = Guid.NewGuid();
        var customerId2 = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var sales = new List<Sale>
        {
            new Sale("SALE-001", customerId1, branchId),
            new Sale("SALE-002", customerId2, branchId)
        };

        sales[0].AddItem(new SaleItem(
            productId1,
            4,
            Money.FromDecimal(10.99m)
        ));

        sales[1].AddItem(new SaleItem(
            productId2,
            10,
            Money.FromDecimal(20.99m)
        ));

        var expectedResponses = new List<SaleViewModel.Response>
        {
            new()
            {
                CustomerName = "Customer 1",
                CustomerEmail = "customer1@email.com",
                Items = new List<SaleItemViewModel>
                {
                    new()
                    {
                        ProductId = productId1,
                        Quantity = 4,
                        UnitPrice = 10.99m,
                        Discount = 4.40m
                    }
                }
            },
            new()
            {
                CustomerName = "Customer 2",
                CustomerEmail = "customer2@email.com",
                Items = new List<SaleItemViewModel>
                {
                    new()
                    {
                        ProductId = productId2,
                        Quantity = 10,
                        UnitPrice = 20.99m,
                        Discount = 41.98m
                    }
                }
            }
        };

        _saleRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(sales);
        _mapper.Map<IEnumerable<SaleViewModel.Response>>(sales).Returns(expectedResponses);

        var query = new GetAllSalesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResponses);
        await _saleRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<IEnumerable<SaleViewModel.Response>>(sales);
    }

    [Fact]
    public async Task Handle_WhenNoSales_ShouldReturnEmptyList()
    {
        // Arrange
        var sales = new List<Sale>();
        var expectedResponses = new List<SaleViewModel.Response>();

        _saleRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(sales);
        _mapper.Map<IEnumerable<SaleViewModel.Response>>(sales).Returns(expectedResponses);

        var query = new GetAllSalesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        await _saleRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<IEnumerable<SaleViewModel.Response>>(sales);
    }
} 