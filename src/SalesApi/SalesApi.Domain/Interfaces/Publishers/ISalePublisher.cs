using SalesApi.Domain.Entities;

namespace SalesApi.Domain.Interfaces;

public interface ISalePublisher
{
    Task PublishCreateSaleAsync(Sale sale);
} 