namespace ShopEasy.Domain.Enums;

/// <summary>
/// Represents the order lifecycle state machine.
/// This enum defines the sequential states an order progresses through,
/// from initial creation to final resolution (delivered or cancelled).
/// Stored as integer in the database for efficient indexing and querying.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order has been created but not yet confirmed.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Order has been confirmed and is being processed.
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Order has been shipped and is in transit.
    /// </summary>
    Shipped = 2,

    /// <summary>
    /// Order has been successfully delivered to the customer.
    /// </summary>
    Delivered = 3,

    /// <summary>
    /// Order has been cancelled and will not be fulfilled.
    /// </summary>
    Cancelled = 4
}
