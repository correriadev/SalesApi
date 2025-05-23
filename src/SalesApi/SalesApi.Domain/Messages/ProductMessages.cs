namespace SalesApi.Domain.Messages;

public class CreateProductMessage
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public string Image { get; set; }
}

public class ProductCreatedMessage
{
    public Guid Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
} 