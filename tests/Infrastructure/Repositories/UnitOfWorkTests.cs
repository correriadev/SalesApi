using Microsoft.EntityFrameworkCore;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.Infrastructure.Data.Sql;
using SalesApi.Infrastructure.Data.Sql.Repositories;
using Xunit;

namespace SalesApi.Tests.Infrastructure.Repositories;

public class UnitOfWorkTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UnitOfWork _unitOfWork;
    private bool _disposed;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public void Products_ShouldReturnProductRepository()
    {
        // Act
        var repository = _unitOfWork.Products;

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<ProductRepository>(repository);
    }

    [Fact]
    public void Sales_ShouldReturnSaleRepository()
    {
        // Act
        var repository = _unitOfWork.Sales;

        // Assert
        Assert.NotNull(repository);
        Assert.IsType<SaleRepository>(repository);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldSaveChangesToDatabase()
    {
        // Arrange
        var product = new Product(
            "Test Product",
            Money.FromDecimal(99.99m),
            "Test Description",
            "Test Category",
            "test.jpg"
        );
        await _unitOfWork.Products.AddAsync(product);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result);
        var savedProduct = await _context.Products.FindAsync(product.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal(product.Title, savedProduct.Title);
    }

    [Fact]
    public void Dispose_ShouldDisposeContext()
    {
        // Act
        _unitOfWork.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => _context.Database.GetDbConnection());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
} 