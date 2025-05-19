using AutoMapper;
using MediatR;
using SalesApi.Application.Common.Mappings;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.ViewModel.V1.Products;

namespace SalesApi.Application.Products.Commands.CreateProduct;

public record CreateProductCommand : IRequest<ProductViewModel.Response>, IMapFrom<ProductViewModel.Request>
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Category { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ProductViewModel.Request, CreateProductCommand>();
        profile.CreateMap<CreateProductCommand, Product>()
            .ForMember(d => d.Price, opt => opt.MapFrom(s => Money.FromDecimal(s.Price)));
    }
} 