using AutoMapper;
using MediatR;
using SalesApi.Application.Common.Mappings;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.ViewModel.V1.Sales;
using System.Linq;

namespace SalesApi.Application.Sales.Commands.CreateSale;

public record CreateSaleCommand : IRequest<SaleViewModel.Response>, IMapFrom<SaleViewModel.Request>
{
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public List<SaleItemViewModel> Items { get; init; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<SaleViewModel.Request, CreateSaleCommand>();
        profile.CreateMap<CreateSaleCommand, Sale>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items.Select(i => new SaleItem(
                i.ProductId,
                i.Quantity,
                Money.FromDecimal(i.UnitPrice),
                Money.FromDecimal(i.Discount)
            ))));
    }
} 