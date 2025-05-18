namespace SalesApi.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ISaleRepository Sales { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
} 