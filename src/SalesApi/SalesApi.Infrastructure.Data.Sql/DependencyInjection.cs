using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesApi.Domain.Repositories;
using SalesApi.Infrastructure.Data.Sql.Repositories;

namespace SalesApi.Infrastructure.Data.Sql;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDataSql(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SalesApiDb"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();

        return services;
    }
} 