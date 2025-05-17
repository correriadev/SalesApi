using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.NpgSql;
using HealthChecks.MongoDb;
using System.Text.Json;
using System.Text;

namespace SalesApi.Infrastructure.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
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