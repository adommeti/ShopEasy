using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopEasy.Domain.Entities;

namespace ShopEasy.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core fluent configuration for the Order entity.
/// Stores Status as a string in the database for SQL readability,
/// and configures relationships with cascade/restrict delete rules.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.OrderId);

        // Store the enum as its string name ("Pending", "Shipped", etc.)
        // instead of the integer value — much easier to read in raw SQL queries
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(o => o.TotalAmount)
            .HasPrecision(10, 2);

        builder.Property(o => o.ShippingAddress)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(o => o.Notes)
            .HasMaxLength(500);

        // Customer relationship: Restrict delete so we don't accidentally
        // wipe a customer and lose all their order history
        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrderItem relationship: Cascade delete so removing an order
        // automatically removes its line items (they're meaningless alone)
        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
