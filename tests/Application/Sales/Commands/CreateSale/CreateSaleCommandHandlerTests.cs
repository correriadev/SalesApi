using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using SalesApi.Application.Sales.Commands.CreateSale;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Interfaces;
using SalesApi.Domain.Repositories;
using SalesApi.Domain.ValueObjects;
using SalesApi.ViewModel.V1.Sales;
using Xunit;

namespace SalesApi.Tests.Application.Sales.Commands.CreateSale;

public class CreateSaleCommandHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ISalePublisher _salePublisher;
    private readonly CreateSaleCommandHandler _handler;

    public CreateSaleCommandHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _salePublisher = Substitute.For<ISalePublisher>();
        _handler = new CreateSaleCommandHandler(_saleRepository, _unitOfWork, _mapper, _salePublisher);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateSale()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerName = "Test Customer",
            CustomerEmail = "test@email.com",
            Items = new List<SaleItemViewModel>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 4,
                    UnitPrice = 10.99m
                }
            }
        };

        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        sale.AddItem(new SaleItem(
            command.Items[0].ProductId,
            command.Items[0].Quantity,
            Money.FromDecimal(command.Items[0].UnitPrice)
        ));

        var expectedResponse = new SaleViewModel.Response
        {
            CustomerName = command.CustomerName,
            CustomerEmail = command.CustomerEmail,
            Items = new List<SaleItemViewModel>
            {
                new()
                {
                    ProductId = command.Items[0].ProductId,
                    Quantity = command.Items[0].Quantity,
                    UnitPrice = command.Items[0].UnitPrice,
                    Discount = 4.40m // 10% discount for 4 items
                }
            }
        };

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<SaleViewModel.Response>(sale).Returns(expectedResponse);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        await _saleRepository.Received(1).AddAsync(Arg.Is<Sale>(s => 
            s.SaleNumber == "SALE-001" &&
            s.Items.Count == command.Items.Count), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyCustomerName_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerName = string.Empty,
            CustomerEmail = "test@email.com",
            Items = new List<SaleItemViewModel>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmptyCustomerEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerName = "Test Customer",
            CustomerEmail = string.Empty,
            Items = new List<SaleItemViewModel>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmptyItems_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerName = "Test Customer",
            CustomerEmail = "test@email.com",
            Items = new List<SaleItemViewModel>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldThrowException()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerName = "Test Customer",
            CustomerEmail = "test@email.com",
            Items = new List<SaleItemViewModel>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 4,
                    UnitPrice = 10.99m
                }
            }
        };

        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        sale.AddItem(new SaleItem(
            command.Items[0].ProductId,
            command.Items[0].Quantity,
            Money.FromDecimal(command.Items[0].UnitPrice)
        ));

        _mapper.Map<Sale>(command).Returns(sale);
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(0);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
} 