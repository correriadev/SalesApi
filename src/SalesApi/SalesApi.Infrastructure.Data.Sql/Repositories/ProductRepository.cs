using Microsoft.EntityFrameworkCore;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Repositories;
using SalesApi.Infrastructure.Data.Sql;

namespace SalesApi.Infrastructure.Data.Sql.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Category == category)
            .ToListAsync(cancellationToken);
    }
} 