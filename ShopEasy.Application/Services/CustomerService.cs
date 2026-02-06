using Microsoft.EntityFrameworkCore;
using ShopEasy.Application.DTOs;
using ShopEasy.Application.Interfaces;

namespace ShopEasy.Application.Services;

/// <summary>
/// Handles customer lookup operations. Read-only — customer creation
/// would be handled by a registration service or seeded directly.
/// </summary>
public class CustomerService(IShopDbContext db) : ICustomerService
{
    /// <summary>
    /// Finds a single customer by their ID.
    /// Returns null if no customer exists with that ID.
    /// </summary>
    public async Task<CustomerDTO?> GetCustomerByIdAsync(int id)
    {
        // Look up the customer by primary key
        var customer = await db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        // If no match, return null so the caller can handle "not found"
        if (customer is null)
        {
            return null;
        }

        // Map the entity to a DTO
        var dto = new CustomerDTO(
            customer.CustomerId,
            customer.FullName,
            customer.Email,
            customer.CreatedAt);

        return dto;
    }

    /// <summary>
    /// Returns all registered customers, sorted alphabetically by name.
    /// </summary>
    public async Task<List<CustomerDTO>> GetAllCustomersAsync()
    {
        // Load all customers, ordered by name for consistent display
        var customers = await db.Customers
            .OrderBy(c => c.FullName)
            .AsNoTracking()
            .Select(c => new CustomerDTO(
                c.CustomerId,
                c.FullName,
                c.Email,
                c.CreatedAt))
            .ToListAsync();

        return customers;
    }
}
