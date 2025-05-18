using SalesApi.Domain.Common;
using SalesApi.Domain.ValueObjects;

namespace SalesApi.Domain.Entities;

public class Sale : Entity
{
    public string SaleNumber { get; private set; }
    public DateTime Date { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid BranchId { get; private set; }
    public Money TotalAmount { get; private set; }
    public bool Cancelled { get; private set; }
    public ICollection<SaleItem> Items { get; private set; }

    private Sale() 
    { 
        // Initialize non-nullable properties for EF Core
        SaleNumber = string.Empty;
        TotalAmount = Money.Zero;
        Items = new List<SaleItem>();
    } // For EF Core

    public Sale(string saleNumber, Guid customerId, Guid branchId)
    {
        ValidateString(saleNumber, nameof(SaleNumber));

        SaleNumber = saleNumber;
        Date = DateTime.UtcNow;
        CustomerId = customerId;
        BranchId = branchId;
        TotalAmount = Money.Zero;
        Cancelled = false;
        Items = new List<SaleItem>();
    }

    public void AddItem(SaleItem item)
    {
        Items.Add(item);
        item.SetSale(this);
        RecalculateTotal();
    }

    public void RemoveItem(SaleItem item)
    {
        Items.Remove(item);
        RecalculateTotal();
    }

    public void Cancel()
    {
        if (Cancelled)
            throw new InvalidOperationException("Sale is already cancelled");

        Cancelled = true;
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Aggregate(Money.Zero, (total, item) => total + item.Total);
    }
} 