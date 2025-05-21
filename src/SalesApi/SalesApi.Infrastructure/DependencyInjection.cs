using Microsoft.Extensions.DependencyInjection;
using SalesApi.Infrastructure.Mappings;

namespace SalesApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        return services;
    }
} 