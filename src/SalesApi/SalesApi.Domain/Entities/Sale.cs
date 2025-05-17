using SalesApi.Domain.Common;

namespace SalesApi.Domain.Entities;

public class Sale : Entity
{
    private string _saleNumber = string.Empty;
    private decimal _totalAmount;

    public Guid Id { get; set; }

    public string SaleNumber
    {
        get => _saleNumber;
        set
        {
            ValidateString(value, nameof(SaleNumber));
            _saleNumber = value;
        }
    }

    public DateTime Date { get; set; }
    public Guid CustomerId { get; set; }
    public Guid BranchId { get; set; }

    public decimal TotalAmount
    {
        get => _totalAmount;
        set
        {
            ValidateMonetaryValue(value, nameof(TotalAmount));
            _totalAmount = value;
        }
    }

    public bool Cancelled { get; set; }
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
} 