using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopEasy.Domain.Entities;

namespace ShopEasy.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core fluent configuration for the OrderItem entity.
/// Ignores the computed LineTotal property so EF Core won't try to
/// create a database column for it.
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.OrderItemId);

        builder.Property(i => i.UnitPrice)
            .HasPrecision(10, 2);

        builder.Property(i => i.Quantity)
            .IsRequired();

        // LineTotal is a computed property (Quantity * UnitPrice) that exists
        // only in C# — tell EF Core to ignore it so it doesn't create a column
        builder.Ignore(i => i.LineTotal);

        // Order relationship (the other side is configured in OrderConfiguration,
        // but being explicit here keeps each config self-contained)
        builder.HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId);

        // Product relationship: Restrict delete so we can't accidentally
        // remove a product that appears in historical orders
        builder.HasOne(i => i.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
