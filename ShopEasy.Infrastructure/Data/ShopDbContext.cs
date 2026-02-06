using Microsoft.EntityFrameworkCore;
using ShopEasy.Application.Interfaces;
using ShopEasy.Domain.Entities;

namespace ShopEasy.Infrastructure.Data;

/// <summary>
/// EF Core database context for the ShopEasy application.
/// Implements <see cref="IShopDbContext"/> so the Application layer
/// can query the database without depending on Infrastructure directly.
/// </summary>
public class ShopDbContext(DbContextOptions<ShopDbContext> options)
    : DbContext(options), IShopDbContext
{
    // DbSet properties for EF Core migrations and direct access
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    // Explicit interface implementation: IShopDbContext exposes IQueryable<T>,
    // and DbSet<T> implements IQueryable<T>, so we just return the DbSet.
    // C# requires explicit implementation because the return types differ
    // (DbSet<T> vs IQueryable<T>) even though DbSet<T> IS an IQueryable<T>.
    IQueryable<Product> IShopDbContext.Products => Products;
    IQueryable<Customer> IShopDbContext.Customers => Customers;
    IQueryable<Order> IShopDbContext.Orders => Orders;
    IQueryable<OrderItem> IShopDbContext.OrderItems => OrderItems;

    /// <inheritdoc />
    void IShopDbContext.Add<T>(T entity)
    {
        base.Set<T>().Add(entity);
    }

    /// <inheritdoc />
    async ValueTask<T?> IShopDbContext.FindAsync<T>(params object[] keyValues) where T : class
    {
        return await base.Set<T>().FindAsync(keyValues);
    }

    /// <summary>
    /// Auto-discovers all IEntityTypeConfiguration classes in this assembly
    /// and applies them. This is like SQLAlchemy's Base.metadata — it finds
    /// all your model configurations automatically so you don't have to
    /// register each one manually.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShopDbContext).Assembly);
    }
}
