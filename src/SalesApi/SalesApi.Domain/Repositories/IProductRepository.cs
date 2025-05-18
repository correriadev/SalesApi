using SalesApi.Domain.Entities;

namespace SalesApi.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
} 