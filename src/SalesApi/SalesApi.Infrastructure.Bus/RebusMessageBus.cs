using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using SalesApi.Domain.Messages;

namespace SalesApi.Infrastructure.Bus;

public class RebusMessageBus : IMessageBus, IDisposable
{
    private readonly IBus _bus;
    private readonly ILogger<RebusMessageBus> _logger;
    private bool _disposed;

    public RebusMessageBus(string connectionString, string queueName, ILogger<RebusMessageBus> logger)
    {
        _logger = logger;
        
        var activator = new BuiltinHandlerActivator();
        
        _bus = Configure.With(activator)
            .Transport(t => t.UseRabbitMq(connectionString, queueName))
            .Routing(r => r.TypeBased()
                .Map<CreateProductMessage>(queueName)
                .Map<ProductCreatedMessage>(queueName)
                .Map<CreateSaleMessage>(queueName)
                .Map<SaleCreatedMessage>(queueName))
            .Options(o => o.RetryStrategy(maxDeliveryAttempts: 3))
            .Start();

        _logger.LogInformation("RebusMessageBus initialized with queue: {QueueName}", queueName);
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            _logger.LogInformation("Publishing message of type {MessageType}", typeof(T).Name);
            await _bus.Publish(message);
            _logger.LogInformation("Successfully published message of type {MessageType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message of type {MessageType}", typeof(T).Name);
            throw;
        }
    }

    public async Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            _logger.LogInformation("Sending message of type {MessageType}", typeof(T).Name);
            await _bus.Send(message);
            _logger.LogInformation("Successfully sent message of type {MessageType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message of type {MessageType}", typeof(T).Name);
            throw;
        }
    }

    public async Task SubscribeAsync<T>(CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            _logger.LogInformation("Subscribing to messages of type {MessageType}", typeof(T).Name);
            await _bus.Subscribe<T>();
            _logger.LogInformation("Successfully subscribed to messages of type {MessageType}", typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to messages of type {MessageType}", typeof(T).Name);
            throw;
        }
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
            _logger.LogInformation("Disposing RebusMessageBus");
            _bus?.Dispose();
        }
        _disposed = true;
    }
} 