# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

PBX (Private Branch Exchange) - a VoIP telephony platform built with .NET 8 microservices architecture. The system manages customers, SIP accounts, gateways, call records (CDR), and billing rates.

**POC/MVP Status**: This project is in Proof of Concept / MVP phase. EF Core migrations can be deleted and recreated as Initial migrations - no need to preserve migration history at this stage.

## Build and Test Commands

```bash
# Build entire solution
dotnet build src/backend/AppPBX.sln

# Run a specific service
dotnet run --project src/backend/Services/Rate/Rate.Api/Rate.Api.csproj

# Run all tests
dotnet test src/backend/AppPBX.sln

# Run tests for a specific service
dotnet test src/backend/Services/Rate/Rate.Api.Tests/Rate.Api.Tests.csproj

# Run a single test
dotnet test --filter "FullyQualifiedName~TariffsEndpointTests.GetByGid_ExistingTariff_ReturnsTariff"

# Run tests with verbose output
dotnet test --logger "console;verbosity=detailed"

# Add EF Core migration (from Data project directory)
dotnet ef migrations add MigrationName --startup-project ../ServiceName.Api/ServiceName.Api.csproj

# Reset migrations (POC/MVP - recreate from scratch)
rm -rf Migrations/
dotnet ef database drop --startup-project ../ServiceName.Api/ServiceName.Api.csproj --force
dotnet ef migrations add Initial --startup-project ../ServiceName.Api/ServiceName.Api.csproj
```

## Architecture

### Microservices Structure

Each service follows a three-project pattern:
- `ServiceName.Api` - ASP.NET Core Minimal API with endpoints, services, and DI
- `ServiceName.Data` - EF Core DbContext, entities, configurations, migrations
- `ServiceName.Api.Tests` - Integration tests with Testcontainers

### Current Services

| Service | Port | Purpose |
|---------|------|---------|
| Gateway | 5000 | YARP reverse proxy, JWT auth, routes to microservices |
| Identity | 5293 | User authentication, validates credentials |
| Rate | 5010 | Tariffs and call rates management |
| Rcp | - | Time entry/registration |
| DataSource | - | Data aggregation |

### Shared Libraries

- `App.BaseData` - Base entity classes (`BaseTable`, `BaseAuditableTable`)
- `App.Shared.Web` - Common web utilities, `GatewayAuthHandler`, `EndpointHelpers`, `PagedResult<T>`
- `App.Shared.Tests` - Test infrastructure (`MySqlTestContainerBase`, `TestAuthHandler`)
- `Common.Toolkit` - Result pattern (`Result<T>`, error types)
- `App.Bps.Enum` - Application enums (`AppRole`, etc.)

### Key Patterns

**Feature-based organization** - Each feature has its own folder:
```
Features/Tariffs/
├── Endpoint.cs           # Minimal API routing
├── ITariffService.cs     # Business logic interface
├── TariffService.cs      # Business logic implementation
├── ITariffDataHandler.cs # Data access interface
├── TariffDataHandler.cs  # EF Core implementation
└── Model/                # DTOs (Request/Response)
```

**Result pattern** - All service methods return `Result<T>` from `Common.Toolkit.ResultPattern`:
```csharp
public async Task<Result<TariffResponse>> CreateAsync(PortalAuthInfo auth, CreateTariffRequest request)
{
    if (invalid) return Result<TariffResponse>.Failure(new ValidationError("Field", "Message"));
    return Result<TariffResponse>.Success(response);
}
```

**Error codes** - Each service defines error codes in `Definitions/ErrorCodes.cs` with `[ErrorMessage]` attribute for translations:
```csharp
public static class ErrorCodes
{
    public static class Tariff
    {
        [ErrorMessage("Taryfa nie została znaleziona")]
        public const string NotFound = "tariff.not_found";
    }
}
// Usage: ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NotFound)
```

**Endpoint authorization** - Use `EndpointHelpers.HandleAuthRequestAsync` with `AppRole[]`:
```csharp
private static readonly AppRole[] AllRoles = [AppRole.FullAccess, AppRole.Ops];

return await EndpointHelpers.HandleAuthRequestAsync(user, filter,
    async (auth, req) => await service.GetListAsync(auth, req), AllRoles);
```

### Authentication Flow

1. Gateway validates JWT tokens and extracts user info
2. Gateway forwards requests with `X-User-*` headers (Id, Gid, Email, Roles, etc.)
3. Microservices use `GatewayAuthHandler` to read headers into `PortalAuthInfo`

### Database

- **Provider**: MySQL 8 with `MySql.EntityFrameworkCore` (not Pomelo)
- **Connection**: `options.UseMySQL(connectionString)`
- **Entities**: Inherit from `BaseAuditableTable` for audit fields and soft delete
- **GID**: 32-character GUID without dashes (`Guid.NewGuid().ToString("N")`) used as public identifier

### Testing

Tests use Testcontainers for real MySQL instances:
```csharp
public class ServiceMySqlTestContainer : MySqlTestContainerBase
{
    protected override async Task InitializeDatabaseAsync()
    {
        // Apply migrations and seed test data
    }
}
```

Use `[Collection("DatabaseCollectionName")]` to share container across test classes.

## Documentation

- `src/docs/developer-guide.md` - Complete development guide with code examples
- `src/docs/authentication-architecture.md` - JWT and Gateway auth details
