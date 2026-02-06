using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopEasy.Domain.Entities;

namespace ShopEasy.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core fluent configuration for the Product entity.
/// Defines column constraints, defaults, and precision — equivalent
/// to SQLAlchemy's Column(String(100), nullable=False) style definitions.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.ProductId);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .HasPrecision(10, 2);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.Property(p => p.StockQuantity)
            .HasDefaultValue(0);
    }
}
