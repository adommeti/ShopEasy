using Microsoft.EntityFrameworkCore;
using ShopEasy.Application.Interfaces;
using ShopEasy.Application.Services;
using ShopEasy.Infrastructure.Data;
using ShopEasy.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// SECTION 1 — DATABASE
// Register ShopDbContext with SQLite. We use AddDbContextFactory
// instead of AddDbContext because the factory pattern works better
// with Blazor Server (where components are long-lived) and allows
// creating short-lived contexts on demand.
// ============================================================

var connectionString = builder.Configuration.GetConnectionString("ShopDb");

builder.Services.AddDbContextFactory<ShopDbContext>(options =>
    options.UseSqlite(connectionString));

// ============================================================
// SECTION 2 — DEPENDENCY INJECTION
// Register application services so controllers can receive them
// via constructor injection. Scoped = one instance per HTTP request.
// ============================================================

builder.Services.AddScoped<IShopDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<ShopDbContext>>().CreateDbContext());

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// ============================================================
// SECTION 3 — WEB API SETUP
// ============================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================================================
// SECTION 4 — CORS
// Permissive policy for development so the Blazor Client and
// any frontend can call the API without cross-origin errors.
// TODO: Restrict CORS origins in production
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ============================================================
// BUILD THE APP
// ============================================================

var app = builder.Build();

// ============================================================
// SECTION 5 — MIDDLEWARE PIPELINE
// Order matters: each middleware runs in the order registered.
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();

// ============================================================
// SECTION 6 — DATABASE INITIALIZATION
// On first run, create the database and seed it with sample data.
// EnsureCreated() builds the schema from our entity configurations.
// Then we check if the Products table is empty and insert seed data.
// ============================================================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ShopDbContext>();

    context.Database.EnsureCreated();

    if (!context.Products.Any())
    {
        context.Products.AddRange(SeedData.GetProducts());
        context.Customers.AddRange(SeedData.GetCustomers());
        context.SaveChanges();
    }
}

// ============================================================
// RUN
// ============================================================

await app.RunAsync();
