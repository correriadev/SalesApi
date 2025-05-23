using SalesApi.Domain.Entities;

namespace SalesApi.Domain.Interfaces;

public interface IProductPublisher
{
    Task PublishCreateProductAsync(Product product);
} 