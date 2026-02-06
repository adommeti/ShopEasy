using Microsoft.AspNetCore.Mvc;
using ShopEasy.Application.DTOs;
using ShopEasy.Application.Interfaces;

namespace ShopEasy.WebApi.Controllers;

/// <summary>
/// REST API endpoints for creating, querying, and managing orders.
/// Handles the full order lifecycle including status transitions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// POST api/orders
    /// Creates a new order from the supplied line items.
    /// Returns 201 Created with a Location header pointing to the new order.
    /// Returns 404 if the customer or any product doesn't exist.
    /// Returns 400 if stock is insufficient.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDTO dto)
    {
        try
        {
            var order = await orderService.CreateOrderAsync(dto);

            // Return HTTP 201 with a Location header like:
            //   Location: /api/orders/42
            // This is REST best practice — tells the client where to find the new resource.
            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
        }
        catch (KeyNotFoundException ex)
        {
            // Customer or product not found
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // Insufficient stock or other business rule violation
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// GET api/orders/{id}
    /// Returns a single order with all its line items.
    /// Returns 404 if the order doesn't exist.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await orderService.GetOrderByIdAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    /// <summary>
    /// GET api/orders
    /// Returns all orders in the system, newest first.
    /// This is the admin/dashboard endpoint.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// GET api/orders/customer/{customerId}
    /// Returns all orders placed by a specific customer, newest first.
    /// Returns an empty list if the customer has no orders.
    /// </summary>
    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var orders = await orderService.GetOrdersByCustomerAsync(customerId);
        return Ok(orders);
    }

    /// <summary>
    /// PATCH api/orders/{id}/status
    /// Advances an order to a new status following the lifecycle state machine.
    /// Returns 404 if the order doesn't exist.
    /// Returns 400 if the status transition is not allowed (e.g. Delivered → Pending).
    /// </summary>
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusUpdateDTO dto)
    {
        try
        {
            var order = await orderService.UpdateOrderStatusAsync(id, dto.NewStatus);
            return Ok(order);
        }
        catch (KeyNotFoundException ex)
        {
            // Order not found
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // Invalid status transition
            return BadRequest(ex.Message);
        }
    }
}
