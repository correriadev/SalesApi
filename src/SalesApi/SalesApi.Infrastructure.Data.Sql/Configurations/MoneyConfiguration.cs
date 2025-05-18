using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SalesApi.Domain.Entities;
using SalesApi.Domain.ValueObjects;

namespace SalesApi.Infrastructure.Data.Sql.Configurations;

public static class MoneyConfiguration
{
    public static void ConfigureMoney(this ModelBuilder modelBuilder)
    {
        var converter = new ValueConverter<Money, decimal>(
            v => v.ToDecimal(),
            v => Money.FromDecimal(v));

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasConversion(converter);

        modelBuilder.Entity<SaleItem>()
            .Property(s => s.UnitPrice)
            .HasConversion(converter);

        modelBuilder.Entity<SaleItem>()
            .Property(s => s.Discount)
            .HasConversion(converter);

        modelBuilder.Entity<SaleItem>()
            .Property(s => s.Total)
            .HasConversion(converter);

        modelBuilder.Entity<Sale>()
            .Property(s => s.TotalAmount)
            .HasConversion(converter);
    }
} 