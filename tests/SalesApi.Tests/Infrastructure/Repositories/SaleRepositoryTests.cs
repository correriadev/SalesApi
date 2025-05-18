using Microsoft.EntityFrameworkCore;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.Infrastructure.Data.Sql;
using SalesApi.Infrastructure.Data.Sql.Repositories;
using Xunit;

namespace SalesApi.Tests.Infrastructure.Repositories;

public class SaleRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly SaleRepository _repository;

    public SaleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new SaleRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSaleExists_ShouldReturnSale()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        await _context.Sales.AddAsync(sale);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(sale.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(sale.Id, result.Id);
        Assert.Equal(sale.SaleNumber, result.SaleNumber);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSaleDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSales()
    {
        // Arrange
        var sales = new List<Sale>
        {
            new("SALE-001", Guid.NewGuid(), Guid.NewGuid()),
            new("SALE-002", Guid.NewGuid(), Guid.NewGuid())
        };
        await _context.Sales.AddRangeAsync(sales);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByCustomerIdAsync_ShouldReturnSalesForCustomer()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var sales = new List<Sale>
        {
            new("SALE-001", customerId, Guid.NewGuid()),
            new("SALE-002", customerId, Guid.NewGuid()),
            new("SALE-003", Guid.NewGuid(), Guid.NewGuid())
        };
        await _context.Sales.AddRangeAsync(sales);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCustomerIdAsync(customerId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, s => Assert.Equal(customerId, s.CustomerId));
    }

    [Fact]
    public async Task GetByBranchIdAsync_ShouldReturnSalesForBranch()
    {
        // Arrange
        var branchId = Guid.NewGuid();
        var sales = new List<Sale>
        {
            new("SALE-001", Guid.NewGuid(), branchId),
            new("SALE-002", Guid.NewGuid(), branchId),
            new("SALE-003", Guid.NewGuid(), Guid.NewGuid())
        };
        await _context.Sales.AddRangeAsync(sales);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBranchIdAsync(branchId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, s => Assert.Equal(branchId, s.BranchId));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
} 