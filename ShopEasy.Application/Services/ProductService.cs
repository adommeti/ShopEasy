using Microsoft.EntityFrameworkCore;
using ShopEasy.Application.DTOs;
using ShopEasy.Application.Interfaces;

namespace ShopEasy.Application.Services;

/// <summary>
/// Handles all product catalog operations — browsing, searching, and filtering.
/// This service is read-only; product creation/editing would go in a separate admin service.
/// </summary>
public class ProductService(IShopDbContext db) : IProductService
{
    /// <summary>
    /// Fetches every active product from the database, sorted alphabetically by name.
    /// We use AsNoTracking() because we're only reading data, not modifying it.
    /// This tells EF Core to skip change-tracking overhead, making the query faster.
    /// </summary>
    public async Task<List<ProductDTO>> GetAllProductsAsync()
    {
        // Start with the Products table, only include active (non-deleted) products
        var activeProducts = db.Products
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .AsNoTracking();

        // Project each Product entity into a lightweight DTO
        // This is like a SELECT with specific columns — we only send what the API needs
        var productDtos = await activeProducts
            .Select(p => new ProductDTO(
                p.ProductId,
                p.Name,
                p.Description,
                p.Price,
                p.StockQuantity,
                p.Category))
            .ToListAsync();

        return productDtos;
    }

    /// <summary>
    /// Looks up a single product by its ID.
    /// Returns null if the product doesn't exist or has been soft-deleted.
    /// </summary>
    public async Task<ProductDTO?> GetProductByIdAsync(int id)
    {
        // Try to find the product by primary key
        var product = await db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == id && p.IsActive);

        // If no matching product was found, return null to signal "not found"
        if (product is null)
        {
            return null;
        }

        // Map the entity to a DTO before returning
        var dto = new ProductDTO(
            product.ProductId,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.Category);

        return dto;
    }

    /// <summary>
    /// Finds all active products in a given category.
    /// The comparison is case-insensitive so "Electronics" matches "electronics".
    /// </summary>
    public async Task<List<ProductDTO>> GetProductsByCategoryAsync(string category)
    {
        // Filter by category (case-insensitive) AND only active products
        var matchingProducts = db.Products
            .Where(p => p.Category.ToLower() == category.ToLower() && p.IsActive)
            .OrderBy(p => p.Name)
            .AsNoTracking();

        // Map each entity to a DTO
        var productDtos = await matchingProducts
            .Select(p => new ProductDTO(
                p.ProductId,
                p.Name,
                p.Description,
                p.Price,
                p.StockQuantity,
                p.Category))
            .ToListAsync();

        return productDtos;
    }

    /// <summary>
    /// Returns a sorted list of all unique category names from active products.
    /// Useful for building a category filter dropdown in the UI.
    /// </summary>
    public async Task<List<string>> GetCategoriesAsync()
    {
        // Pull distinct categories from all active products
        var categories = await db.Products
            .Where(p => p.IsActive)
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .AsNoTracking()
            .ToListAsync();

        return categories;
    }
}
