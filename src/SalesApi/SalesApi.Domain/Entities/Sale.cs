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
        ValidateGuid(customerId, nameof(CustomerId));
        ValidateGuid(branchId, nameof(BranchId));

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
        if (Cancelled)
            throw new InvalidOperationException("Cannot add items to a cancelled sale");

        if (item == null)
            throw new ArgumentNullException(nameof(item));

        // First check if this exact item instance is already in the collection
        if (Items.Contains(item))
            return;

        // Then check if we already have this product in the sale
        var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            // If adding the new quantity would exceed the limit, throw an exception
            if (existingItem.Quantity + item.Quantity > BusinessRules.SaleItem.MAX_QUANTITY)
                throw new InvalidOperationException($"Adding {item.Quantity} more items would exceed the maximum limit of {BusinessRules.SaleItem.MAX_QUANTITY} items per product");

            // Update the existing item's quantity
            existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity);
        }
        else
        {
            Items.Add(item);
            item.SetSale(this);
        }

        RecalculateTotal();
    }

    public void RemoveItem(SaleItem item)
    {
        if (Cancelled)
            throw new InvalidOperationException("Cannot remove items from a cancelled sale");

        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (!Items.Contains(item))
            throw new InvalidOperationException("Item does not belong to this sale");

        Items.Remove(item);
        RecalculateTotal();
    }

    public void Cancel()
    {
        if (Cancelled)
            throw new InvalidOperationException("Sale is already cancelled");

        if (Items.Count == 0)
            throw new InvalidOperationException("Cannot cancel a sale with no items");

        Cancelled = true;
    }

    public void RecalculateTotal()
    {
        if (!Items.Any())
        {
            TotalAmount = Money.Zero;
            return;
        }

        TotalAmount = Items.Aggregate(Money.Zero, (total, item) => total + item.Total);
    }

    private static void ValidateString(string value, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{propertyName} cannot be null or empty", propertyName);
    }

    private static void ValidateGuid(Guid value, string propertyName)
    {
        if (value == Guid.Empty)
            throw new ArgumentException($"{propertyName} cannot be empty", propertyName);
    }
} 