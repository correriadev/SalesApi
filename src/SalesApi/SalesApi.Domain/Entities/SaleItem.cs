using SalesApi.Domain.Common;
using SalesApi.Domain.ValueObjects;

namespace SalesApi.Domain.Entities;

public class SaleItem : Entity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money Discount { get; private set; }
    public Money Total { get; private set; }
    public Guid SaleId { get; private set; }
    public Sale? Sale { get; private set; }

    private SaleItem() 
    { 
        // Initialize non-nullable properties for EF Core
        UnitPrice = Money.Zero;
        Discount = Money.Zero;
        Total = Money.Zero;
    } // For EF Core

    public SaleItem(Guid productId, int quantity, Money unitPrice)
    {
        ValidateQuantity(quantity, nameof(Quantity));
        
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = CalculateDiscount(quantity, unitPrice);
        Total = CalculateTotal();
    }

    private Money CalculateDiscount(int quantity, Money unitPrice)
    {
        if (quantity < BusinessRules.SaleItem.MIN_QUANTITY_FOR_DISCOUNT)
        {
            return Money.Zero;
        }

        var subtotal = unitPrice * quantity;
        var discountPercentage = quantity >= BusinessRules.SaleItem.MIN_QUANTITY_FOR_HIGHER_DISCOUNT 
            ? BusinessRules.SaleItem.HIGHER_DISCOUNT_PERCENTAGE 
            : BusinessRules.SaleItem.STANDARD_DISCOUNT_PERCENTAGE;

        return subtotal * discountPercentage;
    }

    private Money CalculateTotal() => 
        (UnitPrice * Quantity) - Discount;

    public void UpdateQuantity(int quantity)
    {
        ValidateQuantity(quantity, nameof(Quantity));
        
        Quantity = quantity;
        Discount = CalculateDiscount(quantity, UnitPrice);
        Total = CalculateTotal();
    }

    public void UpdateUnitPrice(Money unitPrice)
    {
        UnitPrice = unitPrice;
        Discount = CalculateDiscount(Quantity, unitPrice);
        Total = CalculateTotal();
    }

    public void SetSale(Sale sale)
    {
        Sale = sale;
        SaleId = sale.Id;
    }

    private static void ValidateQuantity(int quantity, string propertyName)
    {
        if (quantity <= 0)
            throw new ArgumentException($"{propertyName} must be greater than zero", propertyName);
        
        if (quantity > BusinessRules.SaleItem.MAX_QUANTITY)
            throw new ArgumentException(BusinessRules.SaleItem.GetMaxQuantityExceededMessage(quantity));
    }
} 