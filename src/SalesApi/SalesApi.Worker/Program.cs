using Microsoft.Extensions.Hosting;
using SalesApi.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using SalesApi.Worker;

namespace SalesApi.Worker;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add shared settings
        var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
        builder.Configuration
            .SetBasePath(basePath)
            .AddJsonFile("sharedSettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"sharedSettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Add health checks
        builder.Services.AddHealthChecks(builder.Configuration);

        // Add worker service
        builder.Services.AddHostedService<Worker>();

        var app = builder.Build();

        // Configure health checks
        app.UseRouting();
        app.UseHealthChecks();

        await app.RunAsync();
    }
}
