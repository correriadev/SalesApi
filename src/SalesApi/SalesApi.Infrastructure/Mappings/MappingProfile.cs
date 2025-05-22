using AutoMapper;
using SalesApi.Application.Products.Commands.CreateProduct;
using SalesApi.Application.Sales.Commands.CreateSale;
using SalesApi.Application.Sales.Queries.GetAllSales;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.ViewModel.V1.Products;
using SalesApi.ViewModel.V1.Sales;

namespace SalesApi.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<ProductViewModel.Request, CreateProductCommand>();
        CreateMap<CreateProductCommand, Product>()
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => Money.FromDecimal(s.Price)))
            .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category))
            .ForMember(d => d.Image, opt => opt.MapFrom(s => s.Image));
        CreateMap<Product, ProductViewModel.Response>()
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price.ToDecimal()));

        // Sale mappings
        CreateMap<SaleViewModel.Request, CreateSaleCommand>();
        CreateMap<SaleItemViewModel, SaleItem>()
            .ConstructUsing((src, ctx) => 
            {
                var saleItem = new SaleItem(
                    src.ProductId,
                    src.Quantity,
                    Money.FromDecimal(src.UnitPrice)
                );
                return saleItem;
            })
            .AfterMap((src, dest) => {
                dest.UpdateQuantity(src.Quantity);
                dest.UpdateUnitPrice(Money.FromDecimal(src.UnitPrice));
            });
        CreateMap<CreateSaleCommand, Sale>()
            .ConstructUsing((src, ctx) => 
            {
                var customerId = Guid.NewGuid(); // In a real application, this would come from a customer service
                var branchId = Guid.NewGuid();   // In a real application, this would come from configuration
                var saleNumber = $"SALE-{Guid.NewGuid().ToString().Substring(0, 8)}";
                var sale = new Sale(saleNumber, customerId, branchId);
                
                foreach (var item in src.Items)
                {
                    var saleItem = ctx.Mapper.Map<SaleItem>(item);
                    sale.AddItem(saleItem);
                }
                
                return sale;
            });
        CreateMap<Sale, SaleViewModel.Response>()
            .ForMember(d => d.TotalAmount, opt => opt.MapFrom(s => s.TotalAmount.ToDecimal()))
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items.Select(i => new SaleItemViewModel
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.ToDecimal(),
                Discount = i.Discount.ToDecimal(),
                Total = i.Total.ToDecimal()
            })));
    }
} 