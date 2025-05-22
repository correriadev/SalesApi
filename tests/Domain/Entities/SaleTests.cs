using FluentAssertions;
using Xunit;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.Domain.Common;
using SalesApi.Domain.Exceptions;

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
    public void Sale_WithInvalidSaleNumber_ShouldThrowDomainException(string? invalidSaleNumber)
    {
        // Act & Assert
        var action = () => new Sale(
            invalidSaleNumber!,
            Guid.NewGuid(),
            Guid.NewGuid()
        );
        action.Should().Throw<DomainException>()
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
            Money.FromDecimal(50m)
        );

        // Act
        sale.AddItem(saleItem);

        // Assert
        sale.Items.Should().ContainSingle();
        sale.Items.Should().Contain(saleItem);
        sale.TotalAmount.Should().Be(Money.FromDecimal(100m)); // (50 * 2) - 0
    }

    [Fact]
    public void Sale_AddItem_WithExistingProduct_ShouldUpdateQuantity()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        var productId = Guid.NewGuid();
        var saleItem1 = new SaleItem(
            productId,
            2,
            Money.FromDecimal(50m)
        );
        var saleItem2 = new SaleItem(
            productId,
            3,
            Money.FromDecimal(50m)
        );

        // Act
        sale.AddItem(saleItem1);
        sale.AddItem(saleItem2);

        // Assert
        sale.Items.Should().ContainSingle();
        sale.Items.First().Quantity.Should().Be(5);
        // When quantities are combined to 5, it qualifies for 10% discount
        var expectedTotal = Money.FromDecimal(225m); // (50 * 5) * 0.9
        sale.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact]
    public void Sale_AddItem_WithExistingProduct_WhenExceedingMaxQuantity_ShouldThrowException()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        var productId = Guid.NewGuid();
        var saleItem1 = new SaleItem(
            productId,
            BusinessRules.SaleItem.MAX_QUANTITY - 1,
            Money.FromDecimal(50m)
        );
        var saleItem2 = new SaleItem(
            productId,
            2,
            Money.FromDecimal(50m)
        );

        // Act
        sale.AddItem(saleItem1);

        // Assert
        var action = () => sale.AddItem(saleItem2);
        action.Should().Throw<DomainException>()
            .WithMessage($"You cannot buy more than {BusinessRules.SaleItem.MAX_QUANTITY} pieces of the same item");
    }

    [Fact]
    public void Sale_AddItem_WhenCancelled_ShouldThrowException()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m)
        );
        sale.AddItem(saleItem);
        sale.Cancel();

        // Act & Assert
        var action = () => sale.AddItem(saleItem);
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot add items to a cancelled sale");
    }

    [Fact]
    public void Sale_RemoveItem_ShouldRemoveItemSuccessfully()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m)
        );
        sale.AddItem(saleItem);

        // Act
        sale.RemoveItem(saleItem);

        // Assert
        sale.Items.Should().BeEmpty();
        sale.TotalAmount.Should().Be(Money.Zero);
    }

    [Fact]
    public void Sale_RemoveItem_WhenCancelled_ShouldThrowException()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m)
        );
        sale.AddItem(saleItem);
        sale.Cancel();

        // Act & Assert
        var action = () => sale.RemoveItem(saleItem);
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot remove items from a cancelled sale");
    }

    [Fact]
    public void Sale_Cancel_ShouldCancelSuccessfully()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m)
        );
        sale.AddItem(saleItem);

        // Act
        sale.Cancel();

        // Assert
        sale.Cancelled.Should().BeTrue();
    }

    [Fact]
    public void Sale_Cancel_WhenAlreadyCancelled_ShouldThrowDomainException()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());
        var saleItem = new SaleItem(
            Guid.NewGuid(),
            2,
            Money.FromDecimal(50m)
        );
        sale.AddItem(saleItem);
        sale.Cancel();

        // Act & Assert
        var action = () => sale.Cancel();
        action.Should().Throw<DomainException>()
            .WithMessage("Sale is already cancelled");
    }

    [Fact]
    public void Sale_Cancel_WhenNoItems_ShouldThrowDomainException()
    {
        // Arrange
        var sale = new Sale("SALE-001", Guid.NewGuid(), Guid.NewGuid());

        // Act & Assert
        var action = () => sale.Cancel();
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot cancel a sale with no items");
    }
} 