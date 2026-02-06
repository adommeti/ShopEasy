namespace ShopEasy.Application.DTOs;

public record CustomerDTO(
    int CustomerId,
    string FullName,
    string Email,
    DateTime CreatedAt);
