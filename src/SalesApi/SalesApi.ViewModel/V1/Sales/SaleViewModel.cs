using System;
using System.Text.Json.Serialization;

namespace SalesApi.ViewModel.V1.Sales;

public static class SaleViewModel
{
    [JsonSchemaName("SaleCreateRequest")]
    public record Request
    {
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public List<SaleItemViewModel> Items { get; init; } = new();
    }

    [JsonSchemaName("SaleResponse")]
    public record Response
    {
        public Guid Id { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public decimal TotalAmount { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public List<SaleItemViewModel> Items { get; init; } = new();
    }

    [JsonSchemaName("SaleCreateResponse")]
    public record CreateResponse : Response
    {
    }

    [JsonSchemaName("SaleListResponse")]
    public record ListResponse
    {
        public Guid Id { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string CustomerEmail { get; init; } = string.Empty;
        public decimal TotalAmount { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    [JsonSchemaName("SaleCancelResponse")]
    public class CancelResponse
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = "Cancelled";
        public DateTime CancelledAt { get; set; }
    }
}

public record SaleItemViewModel
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Discount { get; init; }
    public decimal Total { get; init; }
} 