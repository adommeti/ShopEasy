using ShopEasy.Domain.Entities;

namespace ShopEasy.Infrastructure.Seed;

/// <summary>
/// Provides static seed data for development and testing.
/// These are the initial rows inserted into the database on first run.
/// </summary>
public static class SeedData
{
    private static readonly DateTime SeedDate = new(2026, 1, 1);

    public static List<Product> GetProducts() =>
    [
        new Product
        {
            ProductId = 1,
            Name = "Wireless Mouse",
            Description = "Ergonomic wireless mouse with adjustable DPI and silent clicks",
            Price = 29.99m,
            StockQuantity = 150,
            Category = "Electronics",
            IsActive = true,
            CreatedAt = SeedDate,
        },
        new Product
        {
            ProductId = 2,
            Name = "Mechanical Keyboard",
            Description = "Full-size mechanical keyboard with Cherry MX Brown switches and RGB backlight",
            Price = 79.99m,
            StockQuantity = 75,
            Category = "Electronics",
            IsActive = true,
            CreatedAt = SeedDate,
        },
        new Product
        {
            ProductId = 3,
            Name = "USB-C Hub",
            Description = "7-in-1 USB-C hub with HDMI, USB 3.0, SD card reader, and power delivery",
            Price = 45.00m,
            StockQuantity = 200,
            Category = "Accessories",
            IsActive = true,
            CreatedAt = SeedDate,
        },
        new Product
        {
            ProductId = 4,
            Name = "Monitor Stand",
            Description = "Adjustable aluminum monitor stand with cable management and storage drawer",
            Price = 34.99m,
            StockQuantity = 60,
            Category = "Accessories",
            IsActive = true,
            CreatedAt = SeedDate,
        },
        new Product
        {
            ProductId = 5,
            Name = "Webcam HD",
            Description = "1080p HD webcam with built-in noise-cancelling microphone and auto-focus",
            Price = 54.99m,
            StockQuantity = 90,
            Category = "Electronics",
            IsActive = true,
            CreatedAt = SeedDate,
        },
        new Product
        {
            ProductId = 6,
            Name = ".NET in Action",
            Description = "Comprehensive programming guide covering modern .NET development patterns",
            Price = 49.99m,
            StockQuantity = 30,
            Category = "Books",
            IsActive = true,
            CreatedAt = SeedDate,
        },
    ];

    public static List<Customer> GetCustomers() =>
    [
        new Customer
        {
            CustomerId = 1,
            FullName = "Alice Johnson",
            Email = "alice@example.com",
            CreatedAt = SeedDate,
        },
        new Customer
        {
            CustomerId = 2,
            FullName = "Bob Smith",
            Email = "bob@example.com",
            CreatedAt = SeedDate,
        },
        new Customer
        {
            CustomerId = 3,
            FullName = "Carol Davis",
            Email = "carol@example.com",
            CreatedAt = SeedDate,
        },
    ];
}
