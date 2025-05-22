using AutoMapper;
using MediatR;
using NSubstitute;
using SalesApi.Application.Products.Commands.CreateProduct;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Repositories;
using SalesApi.Domain.ValueObjects;
using SalesApi.ViewModel.V1.Products;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SalesApi.Tests.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SalesApi.Infrastructure.Mappings.MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _handler = new CreateProductCommandHandler(
            _productRepository,
            _unitOfWork,
            _mapper);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateProduct()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test Category",
            Image = "test.jpg"
        };

        var product = new Product(
            command.Title,
            Money.FromDecimal(command.Price),
            command.Description,
            command.Category,
            command.Image);

        _productRepository
            .AddAsync(Arg.Any<Product>())
            .Returns(Task.FromResult(product));

        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Title, result.Title);
        Assert.Equal(command.Description, result.Description);
        Assert.Equal(command.Price, result.Price);
        Assert.Equal(command.Category, result.Category);
        Assert.Equal(command.Image, result.Image);

        await _productRepository.Received(1).AddAsync(Arg.Is<Product>(p =>
            p.Title == command.Title &&
            p.Description == command.Description &&
            p.Price == Money.FromDecimal(command.Price) &&
            p.Category == command.Category &&
            p.Image == command.Image));

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = string.Empty,
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test Category",
            Image = "test.jpg"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmptyDescription_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Test Product",
            Description = string.Empty,
            Price = 99.99m,
            Category = "Test Category",
            Image = "test.jpg"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmptyCategory_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = string.Empty,
            Image = "test.jpg"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmptyImage_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test Category",
            Image = string.Empty
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NegativePrice_ShouldThrowArgumentException()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Test Product",
            Description = "Test Description",
            Price = -99.99m,
            Category = "Test Category",
            Image = "test.jpg"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_SaveChangesFails_ShouldThrowException()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Category = "Test Category",
            Image = "test.jpg"
        };

        var product = new Product(
            command.Title,
            Money.FromDecimal(command.Price),
            command.Description,
            command.Category,
            command.Image);

        _productRepository
            .AddAsync(Arg.Any<Product>())
            .Returns(Task.FromResult(product));

        _unitOfWork
            .SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(0);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }
} 