using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesApi.Domain.Entities;

namespace SalesApi.Infrastructure.Data.Sql.Configurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.ProductId)
            .IsRequired();

        builder.Property(si => si.Quantity)
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.Discount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(si => si.SaleId)
            .IsRequired();
    }
} 