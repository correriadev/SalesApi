using System;
using System.Threading;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;

namespace SalesApi.Infrastructure.Bus;

public class RebusMessageBus : IMessageBus, IDisposable
{
    private readonly IBus _bus;
    private bool _disposed;

    public RebusMessageBus(string connectionString, string queueName)
    {
        _bus = Configure.With(new BuiltinHandlerActivator())
            .Transport(t => t.UseRabbitMq(connectionString, queueName))
            .Options(o => o.SimpleRetryStrategy(maxDeliveryAttempts: 3))
            .Start();
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        await _bus.Publish(message);
    }

    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        await _bus.Send(message);
    }

    public async Task SubscribeAsync<T>(CancellationToken cancellationToken = default) where T : class
    {
        await _bus.Subscribe<T>();
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
            _bus?.Dispose();
        }
        _disposed = true;
    }
} 