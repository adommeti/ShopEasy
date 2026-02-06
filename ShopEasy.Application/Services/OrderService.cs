using Microsoft.EntityFrameworkCore;
using ShopEasy.Application.DTOs;
using ShopEasy.Application.Interfaces;
using ShopEasy.Domain.Entities;
using ShopEasy.Domain.Enums;

namespace ShopEasy.Application.Services;

/// <summary>
/// Handles the full order lifecycle — creation, status transitions, and queries.
/// Contains the state machine rules that govern which status transitions are allowed.
/// </summary>
public class OrderService(IShopDbContext db) : IOrderService
{
    // ──────────────────────────────────────────────────────────────
    // STATE MACHINE
    // Maps each OrderStatus to the list of statuses it can transition to.
    // Terminal states (Delivered, Cancelled) map to empty lists.
    // This is like a Python dict:  {Pending: [Confirmed, Cancelled], ...}
    // ──────────────────────────────────────────────────────────────
    private static readonly Dictionary<OrderStatus, List<OrderStatus>> AllowedTransitions = new()
    {
        { OrderStatus.Pending,   [OrderStatus.Confirmed, OrderStatus.Cancelled] },
        { OrderStatus.Confirmed, [OrderStatus.Shipped, OrderStatus.Cancelled] },
        { OrderStatus.Shipped,   [OrderStatus.Delivered] },
        { OrderStatus.Delivered, [] },
        { OrderStatus.Cancelled, [] },
    };

    // ──────────────────────────────────────────────────────────────
    // PURE LOGIC — no database, easy to unit test
    // ──────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public bool CanTransition(string currentStatus, string newStatus)
    {
        // Try to parse the string into an OrderStatus enum value.
        // Enum.TryParse is like Python's try/except on Enum(value) — returns false
        // instead of throwing if the string doesn't match any enum member.
        var currentIsValid = Enum.TryParse<OrderStatus>(currentStatus, ignoreCase: true, out var current);
        var newIsValid = Enum.TryParse<OrderStatus>(newStatus, ignoreCase: true, out var target);

        // If either string couldn't be parsed, the transition is invalid
        if (!currentIsValid || !newIsValid)
        {
            return false;
        }

        // Look up the allowed transitions for the current state
        var allowed = AllowedTransitions[current];

        // Check if the target state is in the allowed list
        return allowed.Contains(target);
    }

    // ──────────────────────────────────────────────────────────────
    // COMMANDS (write operations that change state)
    // ──────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<OrderDTO> CreateOrderAsync(CreateOrderDTO dto)
    {
        // Step 1: Collect all the product IDs the customer wants to order
        var requestedProductIds = dto.Items
            .Select(item => item.ProductId)
            .ToList();

        // Step 2: Load those products from the database in a single query
        // (much faster than loading one at a time in a loop)
        var products = await db.Products
            .Where(p => requestedProductIds.Contains(p.ProductId))
            .ToListAsync();

        // Step 3: Make sure every requested product actually exists
        var foundProductIds = products.Select(p => p.ProductId).ToHashSet();
        var missingProductIds = requestedProductIds.Where(id => !foundProductIds.Contains(id)).ToList();

        if (missingProductIds.Count > 0)
        {
            var missingList = string.Join(", ", missingProductIds);
            throw new KeyNotFoundException($"Products not found: {missingList}");
        }

        // Step 4: Build a lookup dictionary so we can quickly find each product by ID
        // This is like Python's {p.id: p for p in products}
        var productLookup = products.ToDictionary(p => p.ProductId);

        // Step 5: Create the Order entity with initial Pending status
        var order = new Order
        {
            CustomerId = dto.CustomerId,
            ShippingAddress = dto.ShippingAddress,
            Notes = dto.Notes,
            Status = OrderStatus.Pending,
        };

        // Step 6: Create an OrderItem for each line in the request
        var totalAmount = 0m; // running total (decimal literal)

        foreach (var itemDto in dto.Items)
        {
            var product = productLookup[itemDto.ProductId];

            // Snapshot the current price — this is THE price for this order,
            // even if the product price changes tomorrow
            var unitPrice = product.Price;

            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = unitPrice,
            };

            order.Items.Add(orderItem);

            // Accumulate the total
            totalAmount += itemDto.Quantity * unitPrice;

            // Step 7: Reduce stock on the product
            product.StockQuantity -= itemDto.Quantity;
        }

        order.TotalAmount = totalAmount;

        // Step 8: Persist everything to the database
        db.Add(order);
        await db.SaveChangesAsync();

        // Step 9: Return the newly created order as a DTO
        // We need to load the customer name for the DTO, so fetch it
        var customer = await db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId);

        var customerName = customer?.FullName ?? "Unknown";

        return MapToDto(order, customerName);
    }

    /// <inheritdoc />
    public async Task<OrderDTO> UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        // Step 1: Find the order
        var order = await db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        if (order is null)
        {
            throw new KeyNotFoundException($"Order {orderId} not found");
        }

        // Step 2: Validate the transition using our state machine
        var currentStatusName = order.Status.ToString();
        var transitionAllowed = CanTransition(currentStatusName, newStatus);

        if (!transitionAllowed)
        {
            throw new InvalidOperationException(
                $"Cannot transition from {currentStatusName} to {newStatus}");
        }

        // Step 3: Apply the new status
        // We already validated the string parses, so this is safe
        order.Status = Enum.Parse<OrderStatus>(newStatus, ignoreCase: true);

        // Step 4: Save and return
        await db.SaveChangesAsync();

        return MapToDto(order, order.Customer.FullName);
    }

    // ──────────────────────────────────────────────────────────────
    // QUERIES (read-only operations)
    // ──────────────────────────────────────────────────────────────

    /// <inheritdoc />
    public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
    {
        // Load the order with all related data in a single query.
        // Include = eager loading, like SQLAlchemy's joinedload().
        // ThenInclude chains into nested relationships.
        var order = await db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == orderId);

        // If the order doesn't exist, return null
        if (order is null)
        {
            return null;
        }

        return MapToDto(order, order.Customer.FullName);
    }

    /// <inheritdoc />
    public async Task<List<OrderDTO>> GetOrdersByCustomerAsync(int customerId)
    {
        // Load all orders for this customer, including line items and products
        var orders = await db.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();

        // Map each order entity to a DTO
        var orderDtos = orders
            .Select(o => MapToDto(o, o.Customer.FullName))
            .ToList();

        return orderDtos;
    }

    /// <inheritdoc />
    public async Task<List<OrderDTO>> GetAllOrdersAsync()
    {
        var orders = await db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .AsNoTracking()
            .ToListAsync();

        var orderDtos = orders
            .Select(o => MapToDto(o, o.Customer.FullName))
            .ToList();

        return orderDtos;
    }

    // ──────────────────────────────────────────────────────────────
    // PRIVATE HELPER — single place for entity-to-DTO mapping
    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Converts an Order entity (with loaded navigation properties) into an OrderDTO.
    /// Kept as a private helper so the mapping logic lives in exactly one place.
    /// </summary>
    private static OrderDTO MapToDto(Order order, string customerName)
    {
        // Map each OrderItem to an OrderItemDTO
        var itemDtos = order.Items
            .Select(item => new OrderItemDTO(
                ProductName: item.Product?.Name ?? "Unknown",
                Quantity: item.Quantity,
                UnitPrice: item.UnitPrice,
                LineTotal: item.LineTotal))
            .ToList();

        // Build the parent OrderDTO
        var orderDto = new OrderDTO(
            OrderId: order.OrderId,
            CustomerName: customerName,
            OrderDate: order.OrderDate,
            Status: order.Status.ToString(),
            TotalAmount: order.TotalAmount,
            Items: itemDtos);

        return orderDto;
    }
}
