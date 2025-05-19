using System;
using System.Text.Json.Serialization;

namespace SalesApi.ViewModel.V1.Products;

public static class ProductViewModel
{
    [JsonSchemaName("ProductCreateRequest")]
    public class Request
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    [JsonSchemaName("ProductResponse")]
    public class Response
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [JsonSchemaName("ProductListResponse")]
    public class ListResponse
    {
        public List<Response> Products { get; set; } = new();
    }
} 