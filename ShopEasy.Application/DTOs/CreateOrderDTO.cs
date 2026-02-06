namespace ShopEasy.Application.DTOs;

public record CreateOrderDTO(
    int CustomerId,
    string ShippingAddress,
    string? Notes,
    List<CreateOrderItemDTO> Items);

public record CreateOrderItemDTO(
    int ProductId,
    int Quantity);
