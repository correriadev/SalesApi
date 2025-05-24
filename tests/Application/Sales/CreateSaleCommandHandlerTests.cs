using AutoMapper;
using NSubstitute;
using SalesApi.Application.Sales.Commands.CreateSale;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Interfaces;
using SalesApi.Domain.Repositories;
using SalesApi.Infrastructure.Bus.Publishers;
using SalesApi.ViewModel.V1.Sales;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        _salePublisher = Substitute.For<ISalePublisher>();
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SalesApi.Infrastructure.Mappings.MappingProfile>();
        });
        _mapper = configuration.CreateMapper();
        _handler = new CreateSaleCommandHandler(_saleRepository, _unitOfWork, _mapper, _salePublisher);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateSale()
    {
        var command = new CreateSaleCommand
        {
            CustomerName = "Test Customer",
            CustomerEmail = "test@email.com",
            Items = new List<SaleItemViewModel> { new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 10 } }
        };
        _saleRepository.AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Sale>()));
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);
        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldThrowException()
    {
        var command = new CreateSaleCommand
        {
            CustomerName = "Test Customer",
            CustomerEmail = "test@email.com",
            Items = new List<SaleItemViewModel> { new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 10 } }
        };
        _saleRepository.AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult(callInfo.Arg<Sale>()));
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(0);
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

} 