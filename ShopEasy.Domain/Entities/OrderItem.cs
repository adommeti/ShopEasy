namespace ShopEasy.Domain.Entities;

/// <summary>
/// Represents a single line item within an order.
/// Captures the product, quantity, and price snapshot at the time of purchase.
/// </summary>
public class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit at the time the order was placed.
    /// Snapshotted so the line item is unaffected by future price changes.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Computed total for this line item. Not stored in the database.
    /// </summary>
    public decimal LineTotal => Quantity * UnitPrice;

    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;
}
