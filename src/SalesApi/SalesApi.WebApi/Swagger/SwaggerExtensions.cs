using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Text.Json.Serialization;

namespace SalesApi.WebApi.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = "SalesApi",
                    Version = description.ApiVersion.ToString(),
                    Description = "Sales API"
                });
            }

            // Configure schema IDs
            c.CustomSchemaIds(type =>
            {
                var schemaName = type.GetCustomAttributes(typeof(JsonSchemaNameAttribute), false)
                    .OfType<JsonSchemaNameAttribute>()
                    .FirstOrDefault()?.Name;

                return schemaName ?? type.Name;
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"SalesApi {description.GroupName.ToUpperInvariant()}");
            }

            c.RoutePrefix = string.Empty;
            c.DocumentTitle = "SalesApi Documentation";
        });

        return app;
    }
} 