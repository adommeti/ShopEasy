namespace ShopEasy.Application.DTOs;

public record ProductDTO(
    int ProductId,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string Category);
