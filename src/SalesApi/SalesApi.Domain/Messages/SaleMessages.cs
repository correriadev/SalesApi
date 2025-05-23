namespace SalesApi.Domain.Messages;

public class CreateSaleMessage
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public List<SaleItemMessage> Items { get; set; }
}

public class SaleItemMessage
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class SaleCreatedMessage
{
    public Guid Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class SaleCreated
{
    public string SaleNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string CustomerId { get; set; } = string.Empty;
} 