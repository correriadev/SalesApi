using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SalesApi.Infrastructure.Extensions;
using SalesApi.Infrastructure.Data.Sql.Extensions;
using SalesApi.WebApi.Swagger;

namespace SalesApi.WebApi;

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

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();

        // Add health checks
        builder.Services.AddHealthChecks(builder.Configuration);

        // Add database
        builder.Services.AddDatabase(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwaggerDocumentation();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.UseHealthChecks();

        // Apply migrations
        await app.Services.MigrateDatabaseAsync();

        app.Run();
    }
} 