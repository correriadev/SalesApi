using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Retry.Simple;
using SalesApi.Domain.Interfaces;
using SalesApi.Domain.Messages;
using SalesApi.Infrastructure.Bus.Consumers;
using SalesApi.Infrastructure.Bus.Publishers;

namespace SalesApi.Infrastructure.Bus;

public static class MessageBusConfiguration
{
    public static IServiceCollection AddMessageBus(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMQ") 
            ?? throw new InvalidOperationException("RabbitMQ connection string is not configured");
        
        var queueName = configuration["MessageBus:QueueName"] 
            ?? throw new InvalidOperationException("MessageBus queue name is not configured");

        services.AddRebus(configure => configure
            .Transport(t => t.UseRabbitMq(connectionString, queueName))
            .Options(o => o.RetryStrategy(maxDeliveryAttempts: 3)));

        // Register message handlers
        services.AddScoped<IMessageHandler<CreateProductMessage>, ProductMessageConsumer>();
        services.AddScoped<IMessageHandler<ProductCreatedMessage>, ProductMessageConsumer>();
        services.AddScoped<IMessageHandler<CreateSaleMessage>, SaleMessageConsumer>();
        services.AddScoped<IMessageHandler<SaleCreatedMessage>, SaleMessageConsumer>();

        // Register publishers
        services.AddScoped<IProductPublisher, ProductPublisher>();
        services.AddScoped<ISalePublisher, SalePublisher>();

        services.AddSingleton<IMessageBus>(sp => new RebusMessageBus(connectionString, queueName));

        return services;
    }
} 