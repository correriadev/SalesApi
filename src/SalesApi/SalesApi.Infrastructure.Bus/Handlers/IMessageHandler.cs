using System.Threading;
using System.Threading.Tasks;

namespace SalesApi.Infrastructure.Bus.Handlers;

public interface IMessageHandler<in TMessage> where TMessage : class
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
} 