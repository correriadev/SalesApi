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
        profile.CreateMap<SaleItemViewModel, SaleItem>()
            .ConstructUsing((src, ctx) => new SaleItem(
                src.ProductId,
                src.Quantity,
                Money.FromDecimal(src.UnitPrice)
            ));
        profile.CreateMap<CreateSaleCommand, Sale>()
            .ConstructUsing((src, ctx) => new Sale("SALE-" + Guid.NewGuid().ToString().Substring(0, 8), Guid.NewGuid(), Guid.NewGuid()))
            .AfterMap((src, dest, context) =>
            {
                foreach (var item in src.Items)
                {
                    var saleItem =  new SaleItem(
                        item.ProductId,
                        item.Quantity,
                        Money.FromDecimal(item.UnitPrice)
                    );
                }
                dest.RecalculateTotal();
            });
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