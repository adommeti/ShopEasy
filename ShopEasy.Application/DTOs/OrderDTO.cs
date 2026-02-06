namespace ShopEasy.Application.DTOs;

public record OrderDTO(
    int OrderId,
    string CustomerName,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    List<OrderItemDTO> Items);

public record OrderItemDTO(
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);
