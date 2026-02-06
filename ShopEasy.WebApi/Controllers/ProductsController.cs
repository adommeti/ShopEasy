using Microsoft.AspNetCore.Mvc;
using ShopEasy.Application.Interfaces;

namespace ShopEasy.WebApi.Controllers;

/// <summary>
/// REST API endpoints for browsing the product catalog.
/// All endpoints are read-only — product management would go in a separate admin controller.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    /// <summary>
    /// GET api/products
    /// Returns all active products sorted alphabetically by name.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// GET api/products/{id}
    /// Returns a single product by its ID.
    /// Returns 404 if the product doesn't exist or has been soft-deleted.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await productService.GetProductByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// GET api/products/categories
    /// Returns a sorted list of all distinct category names.
    /// Useful for building filter dropdowns in the UI.
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await productService.GetCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// GET api/products/category/{category}
    /// Returns all active products in the specified category.
    /// The category match is case-insensitive.
    /// </summary>
    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var products = await productService.GetProductsByCategoryAsync(category);
        return Ok(products);
    }
}
