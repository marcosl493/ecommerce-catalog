using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);
        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.OwnsOne(p => p.Image, image =>
        {
            image.Property(i => i.Path)
                .HasColumnName("ImagePath")
                .HasMaxLength(500)
                .IsRequired(false);
        });

        builder.HasIndex(p => p.Price)
            .HasDatabaseName("idx_products_active_price")
            .HasFilter("\"Active\" = true");

        builder.HasIndex(p => p.Category)
            .HasDatabaseName("idx_products_category");

        builder.HasIndex(p => p.Active)
            .HasDatabaseName("idx_products_status");

        builder.HasIndex(p => new { p.Category, p.Price })
            .HasDatabaseName("idx_products_active_category_price")
            .HasFilter("\"Active\" = true");
    }
}
