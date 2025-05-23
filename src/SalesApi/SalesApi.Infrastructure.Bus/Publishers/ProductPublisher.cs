using Rebus.Bus;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Interfaces;
using SalesApi.Domain.Messages;

namespace SalesApi.Infrastructure.Bus.Publishers;

public class ProductPublisher : IProductPublisher
{
    private readonly IBus _bus;

    public ProductPublisher(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishCreateProductAsync(Product product)
    {
        var message = new CreateProductMessage
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            Price = product.Price.ToDecimal(),
            Category = product.Category,
            Image = product.Image
        };

        await _bus.Publish(message);
    }
} 