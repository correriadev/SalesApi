using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Gateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOcelot();
        builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

        var app = builder.Build();

        app.UseOcelot().Wait();

        app.Run();
    }
} 