using ShopEasy.Application.DTOs;

namespace ShopEasy.Application.Interfaces;

/// <summary>
/// Defines operations for creating, querying, and managing orders.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Creates a new order from the supplied line items.
    /// Validates that the customer exists, products are active, and stock is sufficient.
    /// </summary>
    /// <param name="dto">The order creation payload.</param>
    /// <returns>The newly created order with computed totals.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the customer or any product is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when stock is insufficient for any line item.</exception>
    Task<OrderDTO> CreateOrderAsync(CreateOrderDTO dto);

    /// <summary>
    /// Finds a single order by its identifier, including all line items.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <returns>The matching order, or <c>null</c> if not found.</returns>
    Task<OrderDTO?> GetOrderByIdAsync(int orderId);

    /// <summary>
    /// Returns all orders placed by a specific customer.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <returns>A list of orders; empty list if the customer has no orders.</returns>
    Task<List<OrderDTO>> GetOrdersByCustomerAsync(int customerId);

    /// <summary>
    /// Returns all orders in the system.
    /// </summary>
    /// <returns>A list of all orders; empty list if none exist.</returns>
    Task<List<OrderDTO>> GetAllOrdersAsync();

    /// <summary>
    /// Advances an order to a new status following the lifecycle state machine.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="newStatus">The target status name (e.g. "Shipped").</param>
    /// <returns>The updated order.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the order is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the status transition is not allowed.</exception>
    Task<OrderDTO> UpdateOrderStatusAsync(int orderId, string newStatus);

    /// <summary>
    /// Checks whether a transition from <paramref name="currentStatus"/> to
    /// <paramref name="newStatus"/> is permitted by the order lifecycle rules.
    /// Pure logic — no database access.
    /// </summary>
    /// <param name="currentStatus">The current status name.</param>
    /// <param name="newStatus">The desired target status name.</param>
    /// <returns><c>true</c> if the transition is valid; otherwise <c>false</c>.</returns>
    bool CanTransition(string currentStatus, string newStatus);
}
