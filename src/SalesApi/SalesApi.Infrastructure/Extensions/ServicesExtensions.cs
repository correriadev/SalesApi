using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace SalesApi.Infrastructure.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        // Register RabbitMQ connection
        services.AddSingleton<IConnection>(sp =>
        {
            var uri = new Uri(configuration.GetConnectionString("RabbitMq")!);
            var factory = new ConnectionFactory { Uri = uri };
            return factory.CreateConnectionAsync().Result;
        });

        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("SalesApiDb")!,
                name: "postgres",
                tags: new[] { "database", "postgres" },
                timeout: TimeSpan.FromSeconds(configuration.GetValue<int>("HealthChecks:Database:Timeout"))
            )
            .AddMongoDb(
                configuration.GetConnectionString("MongoDb")!,
                name: "mongodb",
                tags: new[] { "database", "mongodb" },
                timeout: TimeSpan.FromSeconds(configuration.GetValue<int>("HealthChecks:MongoDb:Timeout"))
            )
            .AddRabbitMQ(
                factory: sp => sp.GetRequiredService<IConnection>(),
                name: "rabbitmq",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "message-bus", "rabbitmq" },
                timeout: TimeSpan.FromSeconds(configuration.GetValue<int>("HealthChecks:RabbitMq:Timeout"))
            );

        return services;
    }

    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(x => new
                        {
                            name = x.Key,
                            status = x.Value.Status.ToString(),
                            description = x.Value.Description,
                            duration = x.Value.Duration
                        })
                    };
                    var json = JsonSerializer.Serialize(response);
                    var bytes = Encoding.UTF8.GetBytes(json);
                    await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
                }
            });
        });

        return app;
    }
} 