using FluentAssertions;
using Xunit;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;

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
            Money.FromDecimal(50m),
            Money.FromDecimal(5m)
        );

        // Assert
        saleItem.Quantity.Should().Be(2);
        saleItem.UnitPrice.Should().Be(Money.FromDecimal(50m));
        saleItem.Discount.Should().Be(Money.FromDecimal(5m));
        saleItem.Total.Should().Be(Money.FromDecimal(95m)); // (50 * 2) - 5
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
            Money.FromDecimal(50m),
            Money.FromDecimal(5m)
        );
        action.Should().Throw<ArgumentException>()
            .WithMessage("*Quantity must be greater than zero*");
    }

    [Fact]
    public void SaleItem_UpdateQuantity_ShouldUpdateSuccessfully()
    {
        // Arrange
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m),
            Money.FromDecimal(5m)
        );

        // Act
        saleItem.UpdateQuantity(3);

        // Assert
        saleItem.Quantity.Should().Be(3);
        saleItem.Total.Should().Be(Money.FromDecimal(145m)); // (50 * 3) - 5
    }

    [Fact]
    public void SaleItem_UpdateUnitPrice_ShouldUpdateSuccessfully()
    {
        // Arrange
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m),
            Money.FromDecimal(5m)
        );

        // Act
        saleItem.UpdateUnitPrice(Money.FromDecimal(60m));

        // Assert
        saleItem.UnitPrice.Should().Be(Money.FromDecimal(60m));
        saleItem.Total.Should().Be(Money.FromDecimal(115m)); // (60 * 2) - 5
    }

    [Fact]
    public void SaleItem_UpdateDiscount_ShouldUpdateSuccessfully()
    {
        // Arrange
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m),
            Money.FromDecimal(5m)
        );

        // Act
        saleItem.UpdateDiscount(Money.FromDecimal(10m));

        // Assert
        saleItem.Discount.Should().Be(Money.FromDecimal(10m));
        saleItem.Total.Should().Be(Money.FromDecimal(90m)); // (50 * 2) - 10
    }

    [Fact]
    public void SaleItem_SetSale_ShouldSetSaleSuccessfully()
    {
        // Arrange
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m),
            Money.FromDecimal(5m)
        );
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());

        // Act
        saleItem.SetSale(sale);

        // Assert
        saleItem.Sale.Should().Be(sale);
        saleItem.SaleId.Should().Be(sale.Id);
    }
} 