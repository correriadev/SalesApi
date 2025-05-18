using System.Threading;
using System.Threading.Tasks;

namespace SalesApi.Infrastructure.Bus;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    Task SubscribeAsync<T>(CancellationToken cancellationToken = default) where T : class;
} 