using ShopEasy.Client.Components;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// SERVICES
// ============================================================

// Razor components with interactive Server rendering (SignalR)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient configured to call our WebAPI.
// All Blazor components can inject HttpClient to fetch data.
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5177";

builder.Services.AddHttpClient("ShopEasyApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Register a default HttpClient that components can inject directly
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ShopEasyApi"));

// ============================================================
// BUILD THE APP
// ============================================================

var app = builder.Build();

// ============================================================
// MIDDLEWARE PIPELINE
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
