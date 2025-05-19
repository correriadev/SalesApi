using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SalesApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
} 