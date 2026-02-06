using ShopEasy.Application.DTOs;

namespace ShopEasy.Application.Interfaces;

/// <summary>
/// Defines operations for browsing and retrieving products from the catalog.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Returns all active products in the catalog.
    /// </summary>
    /// <returns>A list of all products; empty list if none exist.</returns>
    Task<List<ProductDTO>> GetAllProductsAsync();

    /// <summary>
    /// Finds a single product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>The matching product, or <c>null</c> if not found.</returns>
    Task<ProductDTO?> GetProductByIdAsync(int id);

    /// <summary>
    /// Returns all products belonging to the specified category.
    /// </summary>
    /// <param name="category">The category name (case-insensitive).</param>
    /// <returns>A list of matching products; empty list if none match.</returns>
    Task<List<ProductDTO>> GetProductsByCategoryAsync(string category);

    /// <summary>
    /// Returns the distinct set of category names across all active products.
    /// </summary>
    /// <returns>A sorted list of unique category names.</returns>
    Task<List<string>> GetCategoriesAsync();
}
