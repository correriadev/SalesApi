using SalesApi.Domain.Repositories;
using SalesApi.Infrastructure.Data.Sql;

namespace SalesApi.Infrastructure.Data.Sql.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IProductRepository? _productRepository;
    private ISaleRepository? _saleRepository;
    private bool _disposed;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products => 
        _productRepository ??= new ProductRepository(_context);

    public ISaleRepository Sales => 
        _saleRepository ??= new SaleRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
} 