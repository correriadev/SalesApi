using Microsoft.EntityFrameworkCore;
using SalesApi.Domain.Entities;
using SalesApi.Domain.Repositories;
using SalesApi.Infrastructure.Data.Sql;

namespace SalesApi.Infrastructure.Data.Sql.Repositories;

public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Items)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Items)
            .Where(s => s.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Items)
            .Where(s => s.BranchId == branchId)
            .ToListAsync(cancellationToken);
    }

    public override async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(sale, cancellationToken);
    }

    public override void Update(Sale sale)
    {
        _dbSet.Update(sale);
    }

    public override void Delete(Sale sale)
    {
        _dbSet.Remove(sale);
    }
} 