using AutoMapper;
using MediatR;
using SalesApi.Application.Common.Mappings;
using SalesApi.Domain.Entities;
using SalesApi.ViewModel.V1.Sales;

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
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items.Select(i => new SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            })));
    }
} 