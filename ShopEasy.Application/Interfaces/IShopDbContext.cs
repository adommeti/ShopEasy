using ShopEasy.Domain.Entities;

namespace ShopEasy.Application.Interfaces;

/// <summary>
/// Abstraction over the database context so the Application layer
/// never depends on Infrastructure directly. The concrete EF Core
/// DbContext in Infrastructure will implement this interface.
/// </summary>
public interface IShopDbContext
{
    IQueryable<Product> Products { get; }
    IQueryable<Customer> Customers { get; }
    IQueryable<Order> Orders { get; }
    IQueryable<OrderItem> OrderItems { get; }

    /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins tracking a new entity so it will be inserted on the next SaveChanges call.
    /// </summary>
    void Add<T>(T entity) where T : class;

    /// <summary>
    /// Finds an entity by its primary key(s). Returns null if not found.
    /// </summary>
    ValueTask<T?> FindAsync<T>(params object[] keyValues) where T : class;
}
