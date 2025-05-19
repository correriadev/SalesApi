using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SalesApi.Infrastructure.Extensions;
using SalesApi.Infrastructure.Data.Sql.Extensions;
using SalesApi.WebApi.Swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using SalesApi.Application;
using SalesApi.Infrastructure;
using SalesApi.Infrastructure.Data.Sql;
using SalesApi.WebApi.Controllers.V1;
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
 
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerDocumentation();
        }

        app.UseHttpsRedirection();
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