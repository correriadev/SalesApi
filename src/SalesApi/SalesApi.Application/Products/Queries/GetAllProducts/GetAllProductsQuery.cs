using AutoMapper;
using MediatR;
using SalesApi.Application.Common.Mappings;
using SalesApi.Domain.Entities;
using SalesApi.ViewModel.V1.Products;

namespace SalesApi.Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductViewModel.Response>>, IMapFrom<Product>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Product, ProductViewModel.Response>();
    }
} 