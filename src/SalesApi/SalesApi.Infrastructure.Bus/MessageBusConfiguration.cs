using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Rebus.Config;
using Rebus.Retry.Simple;
using SalesApi.Infrastructure.Bus.Handlers;

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
            .Options(o => o.SimpleRetryStrategy(maxDeliveryAttempts: 3)));

        // Register message handlers
        services.AddScoped<IMessageHandler<SaleCreated>, SaleCreatedHandler>();

        services.AddSingleton<IMessageBus>(sp => new RebusMessageBus(connectionString, queueName));

        return services;
    }
} 