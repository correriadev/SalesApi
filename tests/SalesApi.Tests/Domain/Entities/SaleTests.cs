using FluentAssertions;
using Xunit;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;

namespace SalesApi.Tests.Domain.Entities;

public class SaleTests
{
    [Fact]
    public void Sale_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());

        // Assert
        sale.SaleNumber.Should().Be("SALE-001");
        sale.Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        sale.Cancelled.Should().BeFalse();
        sale.Items.Should().BeEmpty();
        sale.TotalAmount.Should().Be(Money.Zero);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Sale_WithInvalidSaleNumber_ShouldThrowArgumentException(string? invalidSaleNumber)
    {
        // Act & Assert
        var action = () => new Sale(
            invalidSaleNumber!,
            Guid.NewGuid(),
            Guid.NewGuid()
        );
        action.Should().Throw<ArgumentException>()
            .WithMessage("*SaleNumber cannot be null or empty*");
    }

    [Fact]
    public void Sale_AddItem_ShouldAddItemSuccessfully()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m),
            Money.FromDecimal(5m)
        );

        // Act
        sale.AddItem(saleItem);

        // Assert
        sale.Items.Should().ContainSingle();
        sale.Items.Should().Contain(saleItem);
        sale.TotalAmount.Should().Be(Money.FromDecimal(95m)); // (50 * 2) - 5
    }

    [Fact]
    public void Sale_RemoveItem_ShouldRemoveItemSuccessfully()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m),
            Money.FromDecimal(5m)
        );
        sale.AddItem(saleItem);

        // Act
        sale.RemoveItem(saleItem);

        // Assert
        sale.Items.Should().BeEmpty();
        sale.TotalAmount.Should().Be(Money.Zero);
    }

    [Fact]
    public void Sale_Cancel_ShouldCancelSuccessfully()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());

        // Act
        sale.Cancel();

        // Assert
        sale.Cancelled.Should().BeTrue();
    }

    [Fact]
    public void Sale_Cancel_WhenAlreadyCancelled_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        sale.Cancel();

        // Act & Assert
        var action = () => sale.Cancel();
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Sale is already cancelled");
    }
} 