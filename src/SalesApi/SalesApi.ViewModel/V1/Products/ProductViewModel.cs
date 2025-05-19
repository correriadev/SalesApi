using System;
using System.Text.Json.Serialization;

namespace SalesApi.ViewModel.V1.Products;

public static class ProductViewModel
{
    [JsonSchemaName("ProductCreateRequest")]
    public class Request
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }

    [JsonSchemaName("ProductResponse")]
    public class Response
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    [JsonSchemaName("ProductListResponse")]
    public class ListResponse
    {
        public List<Response> Products { get; set; } = new();
    }
} 