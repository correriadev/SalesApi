using AutoMapper;
using MediatR;
using SalesApi.Application.Common.Mappings;
using SalesApi.Domain.Entities;
using SalesApi.ViewModel.V1.Products;

namespace SalesApi.Application.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<ProductViewModel.Response>, IMapFrom<ProductViewModel.Request>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ProductViewModel.Request, CreateProductCommand>();
        profile.CreateMap<CreateProductCommand, Product>();
    }
} 