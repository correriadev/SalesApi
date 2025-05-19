using AutoMapper;
using MediatR;
using SalesApi.Application.Common.Mappings;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.ViewModel.V1.Sales;

namespace SalesApi.Application.Sales.Queries.GetAllSales;

public record GetAllSalesQuery : IRequest<IEnumerable<SaleViewModel.Response>>, IMapFrom<Sale>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Sale, SaleViewModel.Response>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items.Select(i => new SaleItemViewModel
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.ToDecimal(),
                Discount = i.Discount.ToDecimal()
            })));
    }
} 