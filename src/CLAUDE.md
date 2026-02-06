# Project: [Your App Name]

## Tech Stack
- Framework: .NET 10 (LTS) / ASP.NET Core 10 / C# 14
- Frontend: Blazor (Interactive Server or WebAssembly — specify your render mode)
- Database: Oracle 19c+ via Oracle.EntityFrameworkCore 10
- ORM: EF Core 10 with Oracle provider
- Deployment: On-premises (IIS / Kestrel behind reverse proxy)
- Auth: Windows Authentication / Active Directory (on-prem)

## C# 14 Features to Use
- Field-backed properties (`field` keyword) for custom accessors
- Extension members (new `extension()` blocks)
- Null-conditional assignment (`?.=`)
- Partial constructors and events
- Use `nameof()` with unbound generics

## Architecture
- Clean Architecture: Domain → Application → Infrastructure → Blazor Server
- CQRS with MediatR for commands/queries
- Domain modeling first — work backwards from the end goal
- Repository pattern with Unit of Work over EF Core

## Blazor Conventions
- Use Interactive Server rendering by default (SSR for initial load)
- All components use .razor with @code blocks (not code-behind unless complex)
- Use CascadingAuthenticationState for auth propagation
- Prefer component composition over inheritance
- Use `@rendermode InteractiveServer` attribute on routable components
- Reusable UI components go in /Components/Shared
- Page components go in /Components/Pages
- Use EditForm with DataAnnotationsValidator for form validation
- Use QuickGrid for data tables

## Oracle / EF Core Conventions
- Connection string stored in appsettings.json, override with environment vars
- Use `UseOracle()` in DbContext configuration
- Use Fluent API for all entity configurations (no data annotations on domain)
- Sequences for ID generation: `UseHiLo("SEQ_NAME")`
- Use Oracle-compatible column types: VARCHAR2, NUMBER, DATE, CLOB
- Stored procedures accessed via `FromSqlRaw` or Dapper for complex queries
- Named query filters for soft delete and multi-tenancy
- Use LeftJoin/RightJoin LINQ operators (new in EF Core 10)

## On-Prem Deployment
- Target: Windows Server + IIS with ASP.NET Core Module
- Publish as framework-dependent deployment (FDD)
- Use web.config transforms for environment-specific config
- Health checks at /health for load balancer probes
- Serilog with structured logging to file + Oracle sink
- HTTPS via internal CA certificates

## Testing
- xUnit for unit tests, NSubstitute for mocking
- bUnit for Blazor component tests
- Integration tests with WebApplicationFactory
- Name tests: MethodName_StateUnderTest_ExpectedBehavior

## AI Instructions
- Start with the simplest solution that works
- Use Plan Mode (Shift+Tab x2) before architectural changes
- Run `dotnet build` after every significant change
- Run `dotnet test` before committing
- Explain trade-offs when multiple approaches exist
- Do NOT use SQL Server syntax — always Oracle-compatible SQL
- Do NOT use Azure-specific services (we are on-prem)

## Oracle EF Core Limitations — DO NOT USE
- HasFilter() on indexes — not supported
- DateOnly / TimeOnly in LINQ queries — not supported
- HasRowsAffectedReturnValue for stored procedures — not supported
- WidthType.PERCENTAGE in tables — use explicit column sizes
- IDENTITY columns — use Oracle SEQUENCES instead
- Empty string comparisons — Oracle treats "" as NULL
- Temporal (historical) data LINQ queries — not supported

## Oracle EF Core 10 Features — DO USE
- LeftJoin / RightJoin LINQ operators
- ExecuteUpdate / ExecuteDelete for bulk operations
- JSON column updates via ExecuteUpdateAsync
- Named query filters for soft delete / multi-tenancy
- Spatial data via Oracle.EntityFrameworkCore.NetTopologySuite (preview)
- Simplified parameter names (no underscore prefix)
- Sensitive data redaction in SQL logging (on by default)