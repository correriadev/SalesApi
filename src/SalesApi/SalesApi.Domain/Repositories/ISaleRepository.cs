using SalesApi.Domain.Entities;

namespace SalesApi.Domain.Repositories;

public interface ISaleRepository : IRepository<Sale>
{
    Task<IEnumerable<Sale>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sale>> GetByBranchIdAsync(Guid branchId, CancellationToken cancellationToken = default);
} 