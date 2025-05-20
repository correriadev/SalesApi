using FluentAssertions;
using Xunit;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.Domain.Common;

namespace SalesApi.Tests.Domain.Entities;

public class SaleItemTests
{
    [Fact]
    public void SaleItem_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m)
        );

        // Assert
        saleItem.Quantity.Should().Be(2);
        saleItem.UnitPrice.Should().Be(Money.FromDecimal(50m));
        saleItem.Discount.Should().Be(Money.Zero);
        saleItem.Total.Should().Be(Money.FromDecimal(100m)); // (50 * 2) - 0
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SaleItem_WithInvalidQuantity_ShouldThrowArgumentException(int invalidQuantity)
    {
        // Act & Assert
        var action = () => new SaleItem(
            Guid.NewGuid(),
            invalidQuantity,
            Money.FromDecimal(50m)
        );
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Quantity must be greater than zero*");
    }

    [Fact]
    public void SaleItem_WithQuantityAboveMax_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new SaleItem(
            Guid.NewGuid(),
            BusinessRules.SaleItem.MAX_QUANTITY + 1,
            Money.FromDecimal(50m)
        );
        action.Should().Throw<ArgumentException>()
            .WithMessage(BusinessRules.SaleItem.GetMaxQuantityExceededMessage(BusinessRules.SaleItem.MAX_QUANTITY + 1));
    }

    [Fact]
    public void SaleItem_WithStandardDiscount_ShouldCalculateCorrectly()
    {
        // Arrange & Act
        var quantity = BusinessRules.SaleItem.MIN_QUANTITY_FOR_DISCOUNT;
        var unitPrice = Money.FromDecimal(50m);
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            quantity,
            unitPrice
        );

        // Assert
        var expectedDiscount = unitPrice * quantity * BusinessRules.SaleItem.STANDARD_DISCOUNT_PERCENTAGE;
        saleItem.Discount.Should().Be(expectedDiscount);
        saleItem.Total.Should().Be((unitPrice * quantity) - expectedDiscount);
    }

    [Fact]
    public void SaleItem_WithHigherDiscount_ShouldCalculateCorrectly()
    {
        // Arrange & Act
        var quantity = BusinessRules.SaleItem.MIN_QUANTITY_FOR_HIGHER_DISCOUNT;
        var unitPrice = Money.FromDecimal(50m);
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            quantity,
            unitPrice
        );

        // Assert
        var expectedDiscount = unitPrice * quantity * BusinessRules.SaleItem.HIGHER_DISCOUNT_PERCENTAGE;
        saleItem.Discount.Should().Be(expectedDiscount);
        saleItem.Total.Should().Be((unitPrice * quantity) - expectedDiscount);
    }

    [Fact]
    public void SaleItem_UpdateQuantity_ShouldUpdateSuccessfully()
    {
        // Arrange
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m)
        );

        // Act
        saleItem.UpdateQuantity(3);

        // Assert
        saleItem.Quantity.Should().Be(3);
        saleItem.Total.Should().Be(Money.FromDecimal(150m)); // (50 * 3) - 0
    }

    [Fact]
    public void SaleItem_UpdateUnitPrice_ShouldUpdateSuccessfully()
    {
        // Arrange
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m)
        );

        // Act
        saleItem.UpdateUnitPrice(Money.FromDecimal(60m));

        // Assert
        saleItem.UnitPrice.Should().Be(Money.FromDecimal(60m));
        saleItem.Total.Should().Be(Money.FromDecimal(120m)); // (60 * 2) - 0
    }

    [Fact]
    public void SaleItem_SetSale_ShouldSetSaleSuccessfully()
    {
        // Arrange
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m)
        );
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());

        // Act
        saleItem.SetSale(sale);

        // Assert
        saleItem.Sale.Should().Be(sale);
        saleItem.SaleId.Should().Be(sale.Id);
    }
} 