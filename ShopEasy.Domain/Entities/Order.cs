using ShopEasy.Domain.Enums;

namespace ShopEasy.Domain.Entities;

/// <summary>
/// Represents a customer order containing one or more line items.
/// Tracks lifecycle state via <see cref="OrderStatus"/>.
/// </summary>
public class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public decimal TotalAmount { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public Customer Customer { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
