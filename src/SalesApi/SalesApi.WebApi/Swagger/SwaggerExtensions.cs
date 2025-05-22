using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Text.Json.Serialization;
using SalesApi.WebApi.Swagger;

namespace SalesApi.WebApi.Swagger;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sales API",
                Version = "v1",
                Description = "API for managing sales and products"
            });

            c.SchemaFilter<ApiResponseSchemaFilter>();

            // Configure schema IDs to handle nested and generic types
            c.CustomSchemaIds(type =>
            {
                string GetTypeName(Type t)
                {
                    if (t.IsGenericType)
                    {
                        var genericArguments = t.GetGenericArguments()
                            .Select(GetTypeName);
                        return $"{t.Name.Split('`')[0]}Of{string.Join("And", genericArguments)}";
                    }
                    
                    if (t.IsNested)
                    {
                        return $"{t.DeclaringType?.Name}{t.Name}";
                    }
                    
                    return t.Name;
                }

                return GetTypeName(type);
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sales API v1");
        });

        return app;
    }
} 