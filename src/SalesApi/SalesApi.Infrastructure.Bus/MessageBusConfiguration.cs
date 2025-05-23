using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rebus.Config;
using Rebus.Retry.Simple;
using Rebus.Serialization.Json;
using Rebus.Routing.TypeBased;
using SalesApi.Domain.Interfaces;
using SalesApi.Domain.Messages;
using SalesApi.Infrastructure.Bus.Consumers;
using SalesApi.Infrastructure.Bus.Publishers;

namespace SalesApi.Infrastructure.Bus;

public static class MessageBusConfiguration
{
    public static IServiceCollection AddMessageBusPublisher(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMQ")
            ?? throw new InvalidOperationException("RabbitMQ connection string is not configured");
        var queueName = configuration["MessageBus:QueueName"]
            ?? throw new InvalidOperationException("MessageBus queue name is not configured");

        services.AddRebus(configure => configure
            .Transport(t => t.UseRabbitMq(connectionString, queueName))
            .Serialization(s => s.UseSystemTextJson())
            .Logging(l => l.Console())
            .Options(o => o.RetryStrategy(maxDeliveryAttempts: 3))
            .Routing(r => r.TypeBased()
                .Map<CreateProductMessage>(queueName)
                .Map<CreateSaleMessage>(queueName)));

        // Register publishers only
        services.AddScoped<IProductPublisher, ProductPublisher>();
        services.AddScoped<ISalePublisher, SalePublisher>();

        services.AddSingleton<IMessageBus>(sp => new RebusMessageBus(
            connectionString,
            queueName,
            sp.GetRequiredService<ILogger<RebusMessageBus>>()));

        return services;
    }

    public static IServiceCollection AddMessageBusSubscriber(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMQ")
            ?? throw new InvalidOperationException("RabbitMQ connection string is not configured");
        var queueName = configuration["MessageBus:QueueName"]
            ?? throw new InvalidOperationException("MessageBus queue name is not configured");

        services.AutoRegisterHandlersFromAssemblyOf<ProductMessageConsumer>();
        services.AddRebus(configure => configure
            .Transport(t => t.UseRabbitMq(connectionString, queueName))
            .Serialization(s => s.UseSystemTextJson())
            .Logging(l => l.Console())
            .Options(o => o.RetryStrategy(maxDeliveryAttempts: 3))
            .Routing(r => r.TypeBased()
                .Map<CreateProductMessage>(queueName)
                .Map<CreateSaleMessage>(queueName)
            )
        );

        // Register publishers as well (optional, if subscribers also need to publish)
        services.AddScoped<IProductPublisher, ProductPublisher>();
        services.AddScoped<ISalePublisher, SalePublisher>();

        services.AddSingleton<IMessageBus>(sp => new RebusMessageBus(
            connectionString,
            queueName,
            sp.GetRequiredService<ILogger<RebusMessageBus>>()));

        return services;
    }

    public static async Task StartMessageBusSubscriptionsAsync(this IServiceProvider serviceProvider)
    {
        var bus = serviceProvider.GetRequiredService<IMessageBus>();
        await bus.SubscribeAsync<CreateProductMessage>();
        await bus.SubscribeAsync<CreateSaleMessage>();
    }
} 