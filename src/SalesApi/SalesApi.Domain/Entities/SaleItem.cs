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

    public SaleItem(Guid productId, int quantity, Money unitPrice, Money discount)
    {
        ValidateQuantity(quantity, nameof(Quantity));
        
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        Total = CalculateTotal();
    }

    private Money CalculateTotal() => 
        (UnitPrice * Quantity) - Discount;

    public void UpdateQuantity(int quantity)
    {
        ValidateQuantity(quantity, nameof(Quantity));
        Quantity = quantity;
        Total = CalculateTotal();
    }

    public void UpdateUnitPrice(Money unitPrice)
    {
        UnitPrice = unitPrice;
        Total = CalculateTotal();
    }

    public void UpdateDiscount(Money discount)
    {
        Discount = discount;
        Total = CalculateTotal();
    }

    public void SetSale(Sale sale)
    {
        Sale = sale;
        SaleId = sale.Id;
    }
} 