using ShopEasy.Domain.Enums;

namespace ShopEasy.Domain.Entities;

/// <summary>
/// Represents a product available for purchase in the catalog.
/// Supports soft deletion via the <see cref="IsActive"/> flag.
/// </summary>
public class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string Category { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
