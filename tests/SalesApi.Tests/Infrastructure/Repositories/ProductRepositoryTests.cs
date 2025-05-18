using Microsoft.EntityFrameworkCore;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.Infrastructure.Data.Sql;
using SalesApi.Infrastructure.Data.Sql.Repositories;
using Xunit;

namespace SalesApi.Tests.Infrastructure.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ShouldReturnProduct()
    {
        // Arrange
        var product = new Product(
            "Test Product",
            Money.FromDecimal(99.99m),
            "Test Description",
            "Test Category",
            "test.jpg"
        );
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(product.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new("Product 1", Money.FromDecimal(99.99m), "Description 1", "Category 1", "image1.jpg"),
            new("Product 2", Money.FromDecimal(149.99m), "Description 2", "Category 2", "image2.jpg")
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByCategoryAsync_ShouldReturnProductsInCategory()
    {
        // Arrange
        var products = new List<Product>
        {
            new("Product 1", Money.FromDecimal(99.99m), "Description 1", "Category 1", "image1.jpg"),
            new("Product 2", Money.FromDecimal(149.99m), "Description 2", "Category 1", "image2.jpg"),
            new("Product 3", Money.FromDecimal(199.99m), "Description 3", "Category 2", "image3.jpg")
        };
        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCategoryAsync("Category 1");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal("Category 1", p.Category));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
} 