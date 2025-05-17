using FluentAssertions;
using Xunit;
using SalesApi.Domain.Entities;

namespace SalesApi.Tests.Domain.Entities;

public class SaleItemTests
{
    [Fact]
    public void SaleItem_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var saleItem = new SaleItem
        {
            ProductId = Guid.NewGuid(),
            Quantity = 2,
            UnitPrice = 50m,
            Discount = 5m,
            Total = 95m
        };

        // Assert
        saleItem.Quantity.Should().Be(2);
        saleItem.UnitPrice.Should().Be(50m);
        saleItem.Discount.Should().Be(5m);
        saleItem.Total.Should().Be(95m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SaleItem_WithInvalidQuantity_ShouldThrowArgumentException(int invalidQuantity)
    {
        // Arrange
        var saleItem = new SaleItem();

        // Act & Assert
        var action = () => saleItem.Quantity = invalidQuantity;
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Quantity must be greater than zero*");
    }

    [Fact]
    public void SaleItem_WithNegativeUnitPrice_ShouldThrowArgumentException()
    {
        // Arrange
        var saleItem = new SaleItem();

        // Act & Assert
        var action = () => saleItem.UnitPrice = -10m;
        action.Should().Throw<ArgumentException>()
            .WithMessage("*UnitPrice cannot be negative*");
    }

    [Fact]
    public void SaleItem_WithNegativeDiscount_ShouldThrowArgumentException()
    {
        // Arrange
        var saleItem = new SaleItem();

        // Act & Assert
        var action = () => saleItem.Discount = -5m;
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Discount cannot be negative*");
    }

    [Fact]
    public void SaleItem_WithNegativeTotal_ShouldThrowArgumentException()
    {
        // Arrange
        var saleItem = new SaleItem();

        // Act & Assert
        var action = () => saleItem.Total = -100m;
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Total cannot be negative*");
    }
} 