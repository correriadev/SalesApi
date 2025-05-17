using SalesApi.Domain.Common;

namespace SalesApi.Domain.Entities;

public class SaleItem : Entity
{
    private decimal _unitPrice;
    private decimal _discount;
    private decimal _total;
    private int _quantity;

    public Guid Id { get; set; }
    public Guid ProductId { get; set; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            ValidateQuantity(value, nameof(Quantity));
            _quantity = value;
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            ValidateMonetaryValue(value, nameof(UnitPrice));
            _unitPrice = value;
        }
    }

    public decimal Discount
    {
        get => _discount;
        set
        {
            ValidateMonetaryValue(value, nameof(Discount));
            _discount = value;
        }
    }

    public decimal Total
    {
        get => _total;
        set
        {
            ValidateMonetaryValue(value, nameof(Total));
            _total = value;
        }
    }

    public Guid SaleId { get; set; }
    public Sale? Sale { get; set; }
} 