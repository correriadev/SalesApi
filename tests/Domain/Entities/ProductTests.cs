using FluentAssertions;
using Xunit;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;

namespace SalesApi.Tests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Product_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var product = new Product(
            "Test Product",
            Money.FromDecimal(99.99m),
            "Test Description",
            "Test Category",
            "test.jpg"
        );

        // Assert
        product.Title.Should().Be("Test Product");
        product.Price.Should().Be(Money.FromDecimal(99.99m));
        product.Description.Should().Be("Test Description");
        product.Category.Should().Be("Test Category");
        product.Image.Should().Be("test.jpg");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Product_WithInvalidTitle_ShouldThrowArgumentException(string? invalidTitle)
    {
        // Act & Assert
        var action = () => new Product(
            invalidTitle!,
            Money.FromDecimal(99.99m),
            "Test Description",
            "Test Category",
            "test.jpg"
        );
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot be null or empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Product_WithInvalidDescription_ShouldThrowArgumentException(string? invalidDescription)
    {
        // Act & Assert
        var action = () => new Product(
            "Test Product",
            Money.FromDecimal(99.99m),
            invalidDescription!,
            "Test Category",
            "test.jpg"
        );
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Description cannot be null or empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Product_WithInvalidCategory_ShouldThrowArgumentException(string? invalidCategory)
    {
        // Act & Assert
        var action = () => new Product(
            "Test Product",
            Money.FromDecimal(99.99m),
            "Test Description",
            invalidCategory!,
            "test.jpg"
        );
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Category cannot be null or empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Product_WithInvalidImage_ShouldThrowArgumentException(string? invalidImage)
    {
        // Act & Assert
        var action = () => new Product(
            "Test Product",
            Money.FromDecimal(99.99m),
            "Test Description",
            "Test Category",
            invalidImage!
        );
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Image cannot be null or empty*");
    }

    [Fact]
    public void Product_UpdateTitle_ShouldUpdateSuccessfully()
    {
        // Arrange
        var product = new Product(
            "Original Title",
            Money.FromDecimal(99.99m),
            "Test Description",
            "Test Category",
            "test.jpg"
        );

        // Act
        product.UpdateTitle("New Title");

        // Assert
        product.Title.Should().Be("New Title");
    }

    [Fact]
    public void Product_UpdatePrice_ShouldUpdateSuccessfully()
    {
        // Arrange
        var product = new Product(
            "Test Product",
            Money.FromDecimal(99.99m),
            "Test Description",
            "Test Category",
            "test.jpg"
        );

        // Act
        product.UpdatePrice(Money.FromDecimal(149.99m));

        // Assert
        product.Price.Should().Be(Money.FromDecimal(149.99m));
    }
} 