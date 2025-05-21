using AutoMapper;
using SalesApi.Application.Sales.Commands.CreateSale;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;
using SalesApi.Infrastructure.Mappings;
using SalesApi.ViewModel.V1.Sales;
using Xunit;
using Xunit.Abstractions;

namespace SalesApi.Tests.Infrastructure.Mappings;

public class MappingProfileTests
{
    private readonly IMapper _mapper;
    private readonly ITestOutputHelper _output;

    public MappingProfileTests(ITestOutputHelper output)
    {
        _output = output;
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void Map_SaleItemViewModel_To_SaleItem_WithDiscount()
    {
        // Arrange
        var viewModel = new SaleItemViewModel
        {
            ProductId = Guid.NewGuid(),
            Quantity = 5,
            UnitPrice = 100.00m
        };

        // Act
        var saleItem = _mapper.Map<SaleItem>(viewModel);

        // Debug output
        _output.WriteLine($"Quantity: {saleItem.Quantity}");
        _output.WriteLine($"UnitPrice: {saleItem.UnitPrice.ToDecimal()}");
        _output.WriteLine($"Discount: {saleItem.Discount.ToDecimal()}");
        _output.WriteLine($"Total: {saleItem.Total.ToDecimal()}");

        // Assert
        Assert.Equal(viewModel.ProductId, saleItem.ProductId);
        Assert.Equal(viewModel.Quantity, saleItem.Quantity);
        Assert.Equal(viewModel.UnitPrice, saleItem.UnitPrice.ToDecimal());
        Assert.Equal(50.00m, saleItem.Discount.ToDecimal(), 2);
        Assert.Equal(450.00m, saleItem.Total.ToDecimal(), 2);
    }

    [Fact]
    public void Map_CreateSaleCommand_To_Sale_WithItemsAndDiscount()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            CustomerName = "Test Customer",
            CustomerEmail = "test@email.com",
            Items = new List<SaleItemViewModel>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 5,
                    UnitPrice = 100.00m
                },
                new()
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 10,
                    UnitPrice = 50.00m
                }
            }
        };

        // Act
        var sale = _mapper.Map<Sale>(command);

        // Debug output
        foreach (var item in sale.Items)
        {
            _output.WriteLine($"Item - Quantity: {item.Quantity}");
            _output.WriteLine($"Item - UnitPrice: {item.UnitPrice.ToDecimal()}");
            _output.WriteLine($"Item - Discount: {item.Discount.ToDecimal()}");
            _output.WriteLine($"Item - Total: {item.Total.ToDecimal()}");
        }

        // Assert
        Assert.Equal(2, sale.Items.Count);
        
        var firstItem = sale.Items.First();
        Assert.Equal(50.00m, firstItem.Discount.ToDecimal(), 2);
        Assert.Equal(450.00m, firstItem.Total.ToDecimal(), 2);

        var secondItem = sale.Items.Last();
        Assert.Equal(100.00m, secondItem.Discount.ToDecimal(), 2);
        Assert.Equal(400.00m, secondItem.Total.ToDecimal(), 2);

        Assert.Equal(850.00m, sale.TotalAmount.ToDecimal(), 2);
    }

    [Fact]
    public void Map_Sale_To_SaleViewModel_WithDiscount()
    {
        // Arrange
        var sale = new Sale("TEST-123", Guid.NewGuid(), Guid.NewGuid());
        var item = new SaleItem(Guid.NewGuid(), 2, Money.FromDecimal(100.00m));
        sale.AddItem(item);

        // Act
        var viewModel = _mapper.Map<SaleViewModel.Response>(sale);

        // Assert
        Assert.Equal(sale.TotalAmount.ToDecimal(), viewModel.TotalAmount);
        Assert.Single(viewModel.Items);
        
        var viewModelItem = viewModel.Items.First();
        Assert.Equal(item.ProductId, viewModelItem.ProductId);
        Assert.Equal(item.Quantity, viewModelItem.Quantity);
        Assert.Equal(item.UnitPrice.ToDecimal(), viewModelItem.UnitPrice);
        Assert.Equal(item.Discount.ToDecimal(), viewModelItem.Discount);
        Assert.Equal(item.Total.ToDecimal(), viewModelItem.Total);
    }
} 