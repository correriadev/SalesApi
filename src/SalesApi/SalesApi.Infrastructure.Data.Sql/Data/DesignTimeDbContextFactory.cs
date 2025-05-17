using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SalesApi.Infrastructure.Data.Sql;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var projectDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".."));
        var sharedSettingsPath = Path.Combine(projectDir, "shared-settings");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile(Path.Combine(sharedSettingsPath, "sharedSettings.json"), optional: false)
            .AddJsonFile(Path.Combine(sharedSettingsPath, "sharedSettings.Local.json"), optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("SalesApiDb");
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
} 