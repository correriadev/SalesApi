using System;
using System.Text.Json.Serialization;

namespace SalesApi.ViewModel.V1.Sales;

public static class SaleViewModel
{
    [JsonSchemaName("SaleCreateRequest")]
    public class Request
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid CustomerId { get; set; }
    }

    [JsonSchemaName("SaleResponse")]
    public class Response
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    [JsonSchemaName("SaleCreateResponse")]
    public class CreateResponse : Response
    {
    }

    [JsonSchemaName("SaleListResponse")]
    public class ListResponse
    {
        public List<Response> Sales { get; set; } = new();
    }

    [JsonSchemaName("SaleCancelResponse")]
    public class CancelResponse
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = "Cancelled";
        public DateTime CancelledAt { get; set; }
    }
} 