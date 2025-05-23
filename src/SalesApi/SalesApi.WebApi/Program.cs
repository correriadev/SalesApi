using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using SalesApi.Api.Middleware;
using SalesApi.Application;
using SalesApi.Infrastructure;
using SalesApi.Infrastructure.Data.Sql;
using SalesApi.Infrastructure.Data.Sql.Extensions;
using SalesApi.Infrastructure.Extensions;
using SalesApi.Infrastructure.Bus;
using SalesApi.WebApi.Swagger;
using System.Text.Json.Serialization;

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
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // Configure API versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version")
            );
        });

        builder.Services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Add Swagger documentation
        builder.Services.AddSwaggerDocumentation();

        // Add health checks
        builder.Services.AddHealthChecks(builder.Configuration);

        // Add database
        builder.Services.AddDatabase(builder.Configuration);

        // Add Application services (MediatR and AutoMapper)
        builder.Services.AddApplication();

        // Add Infrastructure services
        builder.Services.AddInfrastructure();

        builder.Services.AddInfrastructureDataSql(builder.Configuration);

        // Add Message Bus
        builder.Services.AddMessageBus(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerDocumentation();

            //TODO: drop database
        }

        app.UseHttpsRedirection();
        
        // Add exception handling middleware before routing and authorization
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        app.UseRouting();
        app.UseAuthorization();

        // Use Swagger documentation
        app.UseSwaggerDocumentation();

        app.MapControllers();
        app.UseHealthChecks();

        // Apply migrations
        await app.Services.MigrateDatabaseAsync();

        app.Run();
    }
} 