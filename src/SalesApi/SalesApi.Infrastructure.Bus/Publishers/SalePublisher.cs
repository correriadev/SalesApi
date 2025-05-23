using Rebus.Bus;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Interfaces;
using SalesApi.Domain.Messages;

namespace SalesApi.Infrastructure.Bus.Publishers;

public class SalePublisher : ISalePublisher
{
    private readonly IBus _bus;

    public SalePublisher(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishCreateSaleAsync(Sale sale)
    {
        var message = new CreateSaleMessage
        {
            Id = sale.Id,
            Items = sale.Items.Select(item => new SaleItemMessage
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.ToDecimal()
            }).ToList()
        };

        await _bus.Publish(message);
    }
} 