using MediatR;
using SalesApi.ViewModel.V1.Products;

namespace SalesApi.Application.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<ProductViewModel.Response>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Category { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
} 