using ShopEasy.Application.DTOs;

namespace ShopEasy.Application.Interfaces;

/// <summary>
/// Defines operations for retrieving customer information.
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Finds a single customer by their identifier.
    /// </summary>
    /// <param name="id">The customer identifier.</param>
    /// <returns>The matching customer, or <c>null</c> if not found.</returns>
    Task<CustomerDTO?> GetCustomerByIdAsync(int id);

    /// <summary>
    /// Returns all registered customers.
    /// </summary>
    /// <returns>A list of all customers; empty list if none exist.</returns>
    Task<List<CustomerDTO>> GetAllCustomersAsync();
}
