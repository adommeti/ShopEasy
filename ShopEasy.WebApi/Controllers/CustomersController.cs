using Microsoft.AspNetCore.Mvc;
using ShopEasy.Application.Interfaces;

namespace ShopEasy.WebApi.Controllers;

/// <summary>
/// REST API endpoints for retrieving customer information.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController(ICustomerService customerService) : ControllerBase
{
    /// <summary>
    /// GET api/customers
    /// Returns all registered customers sorted by name.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await customerService.GetAllCustomersAsync();
        return Ok(customers);
    }

    /// <summary>
    /// GET api/customers/{id}
    /// Returns a single customer by their ID.
    /// Returns 404 if the customer doesn't exist.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await customerService.GetCustomerByIdAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }
}
