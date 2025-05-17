using FluentAssertions;
using Xunit;
using SalesApi.Domain.Entities;

namespace SalesApi.Tests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Product_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var product = new Product
        {
            Title = "Test Product",
            Price = 99.99m,
            Description = "Test Description",
            Category = "Test Category",
            Image = "test.jpg"
        };

        // Assert
        product.Title.Should().Be("Test Product");
        product.Price.Should().Be(99.99m);
        product.Description.Should().Be("Test Description");
        product.Category.Should().Be("Test Category");
        product.Image.Should().Be("test.jpg");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Product_WithInvalidTitle_ShouldThrowArgumentException(string invalidTitle)
    {
        // Arrange
        var product = new Product();

        // Act & Assert
        var action = () => product.Title = invalidTitle;
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot be null or empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Product_WithInvalidDescription_ShouldThrowArgumentException(string invalidDescription)
    {
        // Arrange
        var product = new Product();

        // Act & Assert
        var action = () => product.Description = invalidDescription;
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Description cannot be null or empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Product_WithInvalidCategory_ShouldThrowArgumentException(string invalidCategory)
    {
        // Arrange
        var product = new Product();

        // Act & Assert
        var action = () => product.Category = invalidCategory;
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Category cannot be null or empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Product_WithInvalidImage_ShouldThrowArgumentException(string invalidImage)
    {
        // Arrange
        var product = new Product();

        // Act & Assert
        var action = () => product.Image = invalidImage;
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Image cannot be null or empty*");
    }

    [Fact]
    public void Product_WithNegativePrice_ShouldThrowArgumentException()
    {
        // Arrange
        var product = new Product();

        // Act & Assert
        var action = () => product.Price = -10m;
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Price cannot be negative*");
    }
} 