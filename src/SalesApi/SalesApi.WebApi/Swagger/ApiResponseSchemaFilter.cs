using Microsoft.OpenApi.Models;
using SalesApi.ViewModel.V1.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SalesApi.WebApi.Swagger;

public class ApiResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsGenericType && context.Type.GetGenericTypeDefinition() == typeof(ApiResponse<>))
        {
            var genericType = context.Type.GetGenericArguments()[0];
            var genericSchema = context.SchemaGenerator.GenerateSchema(genericType, context.SchemaRepository);

            schema.Properties.Clear();
            schema.Properties.Add("data", genericSchema);
            schema.Properties.Add("status", new OpenApiSchema { Type = "string" });
            schema.Properties.Add("message", new OpenApiSchema { Type = "string" });
        }
    }
} 