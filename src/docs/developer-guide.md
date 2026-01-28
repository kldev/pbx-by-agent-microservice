# Przewodnik Dewelopera - PBX Microservices

Kompletny przewodnik po tworzeniu, kodowaniu, testowaniu i uruchamianiu mikroserwisów w projekcie PBX.

> **POC/MVP Notice**: Ten projekt jest w fazie Proof of Concept / MVP. Migracje EF Core mogą być usuwane i tworzone od nowa jako Initial migration. Nie ma potrzeby zachowywania historii migracji na tym etapie.

---

## Spis treści

1. [Architektura projektu](#1-architektura-projektu)
2. [Tworzenie nowego mikroserwisu](#2-tworzenie-nowego-mikroserwisu)
3. [Wzorce kodowania](#3-wzorce-kodowania)
4. [Tworzenie Feature (CRUD)](#4-tworzenie-feature-crud)
5. [Testowanie](#5-testowanie)
6. [Seedowanie danych](#6-seedowanie-danych)
7. [Integracja z Gateway](#7-integracja-z-gateway)
8. [Migracje bazy danych](#8-migracje-bazy-danych)
9. [Uruchamianie projektu](#9-uruchamianie-projektu)
10. [Checklist nowego mikroserwisu](#10-checklist-nowego-mikroserwisu)

---

## 1. Architektura projektu

### 1.1 Struktura katalogów

```
src/
├── backend/
│   ├── AppPBX.sln              # Główny plik solution
│   ├── Services/               # Mikroserwisy
│   │   ├── Gateway/            # YARP API Gateway
│   │   ├── Identity/           # Autentykacja
│   │   ├── RateService/        # Taryfy i stawki (port 5010)
│   │   ├── Rcp/                # Rejestracja czasu pracy
│   │   └── DataSource/         # Agregacja danych
│   ├── Shared/                 # Wspólne biblioteki
│   │   ├── App.BaseData/       # Klasy bazowe encji
│   │   ├── App.Shared.Web/     # Helpers dla API
│   │   ├── App.Shared.Tests/   # Infrastruktura testów
│   │   ├── Common.Toolkit/     # Result pattern, utilities
│   │   ├── App.Bps.Enum/       # Enumy aplikacji (AppRole, etc.)
│   │   └── Seed.Shared/        # Wspólne seedowanie
│   └── Tools/                  # Narzędzia
├── docs/                       # Dokumentacja
└── front/                      # Frontend (placeholder)
```

### 1.2 Technologie

| Komponent | Technologia |
|-----------|-------------|
| Framework | .NET 8.0 |
| ORM | Entity Framework Core 8.0.11 |
| Baza danych | MySQL 8 (MySql.EntityFrameworkCore 8.0.8) |
| API Gateway | YARP |
| Autentykacja | JWT Bearer (przez Gateway) |
| Testy | xUnit 2.9.3 + Testcontainers.MySql 4.9.0 |

### 1.3 Porty serwisów (development)

| Serwis | Port | Prefix API |
|--------|------|------------|
| Gateway | 5000 | `/api/*` |
| Identity | 5293 | `/api/identity/*` |
| RateService | 5010 | `/api/rates/*` |
| Rcp | - | `/api/rcp/*` |
| DataSource | - | `/api/datasource/*` |

---

## 2. Tworzenie nowego mikroserwisu

### 2.1 Struktura nowego serwisu

Każdy mikroserwis składa się z trzech projektów:

```
src/backend/Services/NazwaSerwisu/
├── NazwaSerwisu.Api/            # Projekt API (.NET Web)
│   ├── Program.cs               # Konfiguracja startup
│   ├── Infrastructure/          # DI, rozszerzenia
│   │   ├── DependencyInjection.cs
│   │   ├── EndpointExtensions.cs
│   │   └── DatabaseExtensions.cs
│   ├── Features/                # Feature-based organization
│   ├── Seed/                    # Seedowanie danych
│   └── Definitions/             # Kody błędów
├── NazwaSerwisu.Data/           # Projekt biblioteki
│   ├── NazwaSerwisDbContext.cs  # DbContext
│   ├── Entities/                # Modele domenowe
│   ├── Configurations/          # Fluent API mapping
│   └── Migrations/              # Migracje EF Core
└── NazwaSerwisu.Api.Tests/      # Projekt testów
    ├── Infrastructure/          # Test fixtures
    └── [Feature]Tests.cs        # Testy per feature
```

### 2.2 Tworzenie projektów

```bash
# 1. Utwórz katalog serwisu
mkdir -p src/backend/Services/NowyServis

# 2. Utwórz projekt API
cd src/backend/Services/NowyServis
dotnet new web -n NowyServis.Api

# 3. Utwórz projekt Data (biblioteka)
dotnet new classlib -n NowyServis.Data

# 4. Utwórz projekt testów
dotnet new xunit -n NowyServis.Api.Tests

# 5. Dodaj referencje
cd NowyServis.Api
dotnet add reference ../NowyServis.Data/NowyServis.Data.csproj
dotnet add reference ../../../Shared/App.BaseData/App.BaseData.csproj
dotnet add reference ../../../Shared/App.Shared.Web/App.Shared.Web.csproj

cd ../NowyServis.Api.Tests
dotnet add reference ../NowyServis.Api/NowyServis.Api.csproj
dotnet add reference ../../../Shared/App.Shared.Tests/App.Shared.Tests.csproj
```

### 2.3 Pakiety NuGet

**NowyServis.Api.csproj:**
```xml
<ItemGroup>
  <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.11" />
  <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.13" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.11">
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
  <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
</ItemGroup>
```

**NowyServis.Data.csproj:**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="8.0.11" />
  <PackageReference Include="MySql.EntityFrameworkCore" Version="8.0.8" />
</ItemGroup>

<ItemGroup>
  <ProjectReference Include="..\..\..\Shared\App.BaseData\App.BaseData.csproj" />
  <ProjectReference Include="..\..\..\Shared\App.Bps.Enum\App.Bps.Enum.csproj" />
</ItemGroup>
```

**NowyServis.Api.Tests.csproj:**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
  <PackageReference Include="xunit" Version="2.9.3" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.5.3">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
  <PackageReference Include="coverlet.collector" Version="6.0.4">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
  <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.11" />
  <PackageReference Include="Testcontainers.MySql" Version="4.9.0" />
</ItemGroup>
```

### 2.4 Program.cs - szablon

```csharp
using System.Text.Json.Serialization;
using App.Shared.Web.Middleware;
using App.Shared.Web.Security;
using App.Shared.Web.Swagger;
using NowyServis.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON to serialize enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add services to the container (with enum strings in Swagger)
builder.Services.AddSwaggerWithEnumStrings();

// Gateway authentication (reads X-User-* headers)
builder.Services.AddAuthentication(GatewayAuthHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, GatewayAuthHandler>(GatewayAuthHandler.SchemeName, null);
builder.Services.AddAuthorization();

// Application services (DbContext, features)
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Database migration on startup
await app.InitializeDatabaseAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLogging();

// Map endpoints
app.MapAppEndpoints();

app.Run();

// Required for WebApplicationFactory in tests
public partial class Program { }
```

### 2.5 appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=pbx_nowyservis;User=root;Password=password;CharSet=utf8mb4;"
  },
  "Seed": {
    "RunOnStartup": false,
    "IncludeShowcaseData": false
  }
}
```

**appsettings.Development.json:**
```json
{
  "Seed": {
    "RunOnStartup": true,
    "IncludeShowcaseData": true
  }
}
```

---

## 3. Wzorce kodowania

### 3.1 Feature-Based Organization

Każda feature (np. Tariffs, Rates) ma własny katalog:

```
Features/
└── Tariffs/
    ├── Endpoint.cs              # Routing (Minimal API)
    ├── ITariffService.cs        # Interfejs business logic
    ├── TariffService.cs         # Implementacja business logic
    ├── ITariffDataHandler.cs    # Interfejs dostępu do danych
    ├── TariffDataHandler.cs     # Implementacja EF Core
    └── Model/
        ├── TariffResponse.cs        # DTO odpowiedzi
        ├── CreateTariffRequest.cs   # Request tworzenia
        ├── UpdateTariffRequest.cs   # Request aktualizacji
        └── TariffListFilter.cs      # Filtr paginacji
```

### 3.2 Result Pattern

Wszystkie operacje biznesowe zwracają `Result<T>` z `Common.Toolkit.ResultPattern`:

```csharp
using Common.Toolkit.ResultPattern;

// W serwisie
public async Task<Result<TariffResponse>> CreateAsync(
    PortalAuthInfo auth,
    CreateTariffRequest request)
{
    // Walidacja
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Result<TariffResponse>.Failure(
            new ValidationError(ErrorCodes.Tariff.NameRequired, "Nazwa jest wymagana"));
    }

    // Sprawdzenie duplikatu
    if (await _dataHandler.ExistsByNameAsync(request.Name))
    {
        return Result<TariffResponse>.Failure(
            new BusinessLogicError("NAME_EXISTS", "Taryfa o tej nazwie już istnieje"));
    }

    // Tworzenie
    var tariff = new Tariff
    {
        Name = request.Name,
        // ...
        CreatedByUserId = auth.UserId
    };

    await _dataHandler.CreateAsync(tariff);
    return Result<TariffResponse>.Success(MapToResponse(tariff));
}
```

**Typy błędów:**

| Typ | HTTP Status | Użycie |
|-----|-------------|--------|
| `ValidationError` | 400 | Błędy walidacji pól |
| `NotFoundError` | 404 | Zasób nie znaleziony |
| `BusinessLogicError` | 400 | Błędy logiki biznesowej |
| `ForbiddenError` | 403 | Brak uprawnień |
| `GenericError` | 500 | Błędy ogólne |

### 3.3 Kody błędów (ErrorCodes)

Każdy serwis definiuje swoje kody błędów w `Definitions/ErrorCodes.cs`. Kody błędów używają atrybutu `[ErrorMessage]` do tłumaczeń.

**Struktura katalogów:**
```
NowyServis.Api/
└── Definitions/
    ├── ErrorCodes.cs       # Definicje kodów błędów
    └── ErrorCodeHelper.cs  # Helper do pobierania tłumaczeń
```

**ErrorCodes.cs - definicja kodów:**
```csharp
using App.Shared.Web;

namespace NowyServis.Api.Definitions;

public static class ErrorCodes
{
    public static class Tariff
    {
        [ErrorMessage("Taryfa nie została znaleziona")]
        public const string NotFound = "tariff.not_found";

        [ErrorMessage("Nazwa taryfy jest wymagana")]
        public const string NameRequired = "tariff.name_required";

        [ErrorMessage("Taryfa o tej nazwie już istnieje")]
        public const string NameExists = "tariff.name_exists";

        [ErrorMessage("Interwał naliczania musi być większy od zera")]
        public const string InvalidBillingIncrement = "tariff.invalid_billing_increment";

        [ErrorMessage("Nie można usunąć domyślnej taryfy")]
        public const string CannotDeleteDefault = "tariff.cannot_delete_default";
    }

    public static class Rate
    {
        [ErrorMessage("Stawka nie została znaleziona")]
        public const string NotFound = "rate.not_found";

        [ErrorMessage("Prefiks jest wymagany")]
        public const string PrefixRequired = "rate.prefix_required";
    }
}
```

**ErrorCodeHelper.cs - pobieranie tłumaczeń:**
```csharp
using System.Reflection;
using App.Shared.Web;

namespace NowyServis.Api.Definitions;

public static class ErrorCodeHelper
{
    private static readonly Dictionary<string, string> Messages = new();

    static ErrorCodeHelper()
    {
        LoadMessages(typeof(ErrorCodes));
    }

    private static void LoadMessages(Type type)
    {
        foreach (var nestedType in type.GetNestedTypes())
        {
            foreach (var field in nestedType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    var code = field.GetValue(null) as string;
                    var attr = field.GetCustomAttribute<ErrorMessageAttribute>();
                    if (code != null && attr != null)
                    {
                        Messages[code] = attr.Polish;
                    }
                }
            }
        }
    }

    public static string GetMessage(string code)
    {
        return Messages.TryGetValue(code, out var message) ? message : code;
    }
}
```

**Użycie w serwisie:**
```csharp
// Zwracanie błędu z kodem i przetłumaczonym komunikatem
return Result<TariffResponse>.Failure(
    new NotFoundError(
        ErrorCodes.Tariff.NotFound,
        ErrorCodeHelper.GetMessage(ErrorCodes.Tariff.NotFound)));

// Lub krócej (jeśli message = code)
return Result<TariffResponse>.Failure(
    new ValidationError(ErrorCodes.Tariff.NameRequired, "Nazwa jest wymagana"));
```

**Konwencja nazewnictwa kodów:**
- Format: `{encja}.{akcja}` (snake_case)
- Przykłady: `tariff.not_found`, `rate.prefix_required`, `destination_group.name_exists`

### 3.4 Klasy bazowe encji

**BaseTable** - podstawowa encja:
```csharp
namespace App.BaseData;

public abstract class BaseTable
{
    public long Id { get; set; }                              // Auto-increment PK
    public string Gid { get; set; } = Guid.NewGuid().ToString("N");  // 32-char GUID dla API
}
```

**BaseAuditableTable** - encja z audytem:
```csharp
namespace App.BaseData;

public abstract class BaseAuditableTable : BaseTable
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long? CreatedByUserId { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public long? ModifiedByUserId { get; set; }
    public bool IsDeleted { get; set; }            // Soft delete
    public DateTime? DeletedAt { get; set; }
}
```

### 3.5 PortalAuthInfo

Dane użytkownika dostępne w każdym request (z `App.Shared.Web.Security`):

```csharp
public class PortalAuthInfo
{
    public long UserId { get; init; }
    public string Gid { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public long? CompanyId { get; init; }
    public int? SbuId { get; init; }
    public long? TeamId { get; init; }
    public List<AppRole> Roles { get; init; } = [];

    public string FullName => $"{FirstName} {LastName}";
    public bool HasRole(AppRole role) => Roles.Contains(role);
    public bool IsRoot => HasRole(AppRole.Root);
}
```

### 3.6 Role systemowe (AppRole)

System definiuje 5 ról użytkowników w enumie `AppRole` (w `App.Bps.Enum`):

| Rola | Wartość | Opis |
|------|---------|------|
| `Root` | 1 | Pełny dostęp systemowy - administratorzy |
| `Ops` | 2 | Operations team - zarządzanie konfiguracją systemu |
| `Admin` | 3 | Administrator - zarządzanie użytkownikami i podstawowymi ustawieniami |
| `Support` | 4 | Support team - obsługa zgłoszeń klientów |
| `User` | 5 | Standardowy użytkownik |

**Hierarchia uprawnień** (od najwyższych):
```
Root > Ops > Admin > Support > User
```

#### Używanie ról jako stringów

Klasa `Roles` w `App.Bps.Enum` dostarcza stałe stringowe do użycia w atrybutach i testach:

```csharp
using App.Bps.Enum;

// Stałe stringowe (do atrybutów, testów)
public static class Roles
{
    public const string Root = "Root";
    public const string Ops = "Ops";
    public const string Admin = "Admin";
    public const string Support = "Support";
    public const string User = "User";

    public static readonly string[] All = [Root, Ops, Admin, Support, User];
}
```

#### Używanie ról w endpointach

```csharp
// W endpoincie - definiuj dozwolone role
private static readonly AppRole[] AdminRoles = [AppRole.Root, AppRole.Admin];
private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops, AppRole.Admin];

// Użycie w handlerze
return await EndpointHelpers.HandleAuthRequestAsync(
    user, filter,
    async (auth, req) => await service.GetListAsync(auth, req),
    AdminRoles);  // Tylko Root i Admin
```

#### Używanie ról w testach

W testach używaj klasy `Roles` zamiast hardkodowanych stringów:

```csharp
using App.Bps.Enum;

// DOBRZE - używaj Roles.*
using var factory = new IdentityApplicationFactory(
    _container.ConnectionString,
    options => options.Roles = [Roles.Root]);

// DOBRZE - wiele ról
options.Roles = [Roles.Admin, Roles.User];

// ŹLE - nie hardkoduj stringów
options.Roles = ["Root"];  // Unikaj!
options.Roles = ["Admin"]; // Unikaj!
```

**Przykład testu autoryzacji:**
```csharp
[Theory]
[InlineData(Roles.Root)]
[InlineData(Roles.Admin)]
public async Task AdminEndpoint_AllowedForAdminRoles(string role)
{
    using var factory = new IdentityApplicationFactory(
        _container.ConnectionString,
        options => options.Roles = [role]);
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/endpoint", request);

    Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
}

[Theory]
[InlineData(Roles.Support)]
[InlineData(Roles.User)]
public async Task AdminEndpoint_ForbiddenForNonAdmin(string role)
{
    using var factory = new IdentityApplicationFactory(
        _container.ConnectionString,
        options => options.Roles = [role]);
    using var client = factory.CreateClient();

    var response = await client.PostAsJsonAsync("/api/endpoint", request);

    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}
```

#### Extension methods (AppRoleExtensions)

```csharp
using App.Bps.Enum;

// Konwersja AppRole -> string
AppRole.Root.ToRoleName()  // "Root"

// Parsowanie string -> AppRole
AppRoleExtensions.Parse("Root")       // AppRole.Root
AppRoleExtensions.TryParse("admin", out var role)  // true, role = AppRole.Admin

// Walidacja
AppRoleExtensions.IsValidRole("Root")    // true
AppRoleExtensions.IsValidRole("Unknown") // false
```

### 3.7 Paginacja

Użyj `PagedResult<T>` z `App.Shared.Web.BaseModel`:

```csharp
// Filter - dziedzicz z PagedRequest dla wspólnych pól
public class TariffListFilter : PagedRequest
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}

// Response - już zdefiniowany w App.Shared.Web.BaseModel
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

---

## 4. Tworzenie Feature (CRUD)

### 4.1 Krok 1: Encja (Data layer)

```csharp
// NowyServis.Data/Entities/Tariff.cs
using App.BaseData;

namespace NowyServis.Data.Entities;

public class Tariff : BaseAuditableTable
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CurrencyCode { get; set; } = "PLN";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
    public DateTime? ValidTo { get; set; }
    public int BillingIncrement { get; set; } = 60;
    public int MinimumDuration { get; set; } = 0;
    public decimal ConnectionFee { get; set; } = 0;

    // Navigation
    public ICollection<Rate> Rates { get; set; } = new List<Rate>();
}
```

### 4.2 Krok 2: DbContext

```csharp
// NowyServis.Data/NowyServisDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace NowyServis.Data;

public class NowyServisDbContext : DbContext
{
    public DbSet<Tariff> Tariffs => Set<Tariff>();
    public DbSet<Rate> Rates => Set<Rate>();

    public NowyServisDbContext(DbContextOptions<NowyServisDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NowyServisDbContext).Assembly);
    }
}
```

### 4.3 Krok 3: Konfiguracja EF Core

```csharp
// NowyServis.Data/Configurations/TariffConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NowyServis.Data.Configurations;

public class TariffConfiguration : IEntityTypeConfiguration<Tariff>
{
    public void Configure(EntityTypeBuilder<Tariff> builder)
    {
        builder.ToTable("tariffs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Gid).HasColumnName("gid").HasMaxLength(32);

        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
        builder.Property(x => x.IsDefault).HasColumnName("is_default");
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.ValidFrom).HasColumnName("valid_from");
        builder.Property(x => x.ValidTo).HasColumnName("valid_to");
        builder.Property(x => x.BillingIncrement).HasColumnName("billing_increment");
        builder.Property(x => x.MinimumDuration).HasColumnName("minimum_duration");
        builder.Property(x => x.ConnectionFee).HasColumnName("connection_fee").HasPrecision(18, 4);

        // Audit fields
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(x => x.ModifiedAt).HasColumnName("modified_at");
        builder.Property(x => x.ModifiedByUserId).HasColumnName("modified_by_user_id");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Index
        builder.HasIndex(x => x.Gid).IsUnique();
        builder.HasIndex(x => x.Name);
    }
}
```

### 4.4 Krok 4: Modele (DTOs)

```csharp
// NowyServis.Api/Features/Tariffs/Model/TariffResponse.cs
public class TariffResponse
{
    public string Gid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CurrencyCode { get; set; } = null!;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public int BillingIncrement { get; set; }
    public int MinimumDuration { get; set; }
    public decimal ConnectionFee { get; set; }
    public int RatesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// CreateTariffRequest.cs
public class CreateTariffRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CurrencyCode { get; set; } = "PLN";
    public bool IsDefault { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public int BillingIncrement { get; set; } = 60;
    public int MinimumDuration { get; set; } = 0;
    public decimal ConnectionFee { get; set; } = 0;
}

// UpdateTariffRequest.cs
public class UpdateTariffRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CurrencyCode { get; set; } = "PLN";
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public int BillingIncrement { get; set; } = 60;
    public int MinimumDuration { get; set; } = 0;
    public decimal ConnectionFee { get; set; } = 0;
}

// TariffListFilter.cs
public class TariffListFilter
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}
```

### 4.5 Krok 5: DataHandler

```csharp
// ITariffDataHandler.cs
public interface ITariffDataHandler
{
    Task<(IEnumerable<Tariff> Items, int TotalCount)> GetPagedAsync(TariffListFilter filter);
    Task<Tariff?> GetByGidAsync(string gid);
    Task<Tariff?> GetByGidWithRatesAsync(string gid);
    Task<bool> ExistsByNameAsync(string name, long? excludeId = null);
    Task CreateAsync(Tariff tariff);
    Task UpdateAsync(Tariff tariff);
    Task DeleteAsync(Tariff tariff);
    Task ClearDefaultFlagAsync(long? excludeId = null);
}

// TariffDataHandler.cs
public class TariffDataHandler : ITariffDataHandler
{
    private readonly NowyServisDbContext _context;

    public TariffDataHandler(NowyServisDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Tariff> Items, int TotalCount)> GetPagedAsync(TariffListFilter filter)
    {
        var query = _context.Tariffs
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        // Filtrowanie
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            query = query.Where(t =>
                t.Name.ToLower().Contains(search) ||
                (t.Description != null && t.Description.ToLower().Contains(search)));
        }

        if (filter.IsActive.HasValue)
            query = query.Where(t => t.IsActive == filter.IsActive);

        // Liczenie
        var totalCount = await query.CountAsync();

        // Paginacja
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Tariff?> GetByGidAsync(string gid)
    {
        return await _context.Tariffs
            .FirstOrDefaultAsync(t => t.Gid == gid && !t.IsDeleted);
    }

    public async Task<Tariff?> GetByGidWithRatesAsync(string gid)
    {
        return await _context.Tariffs
            .Include(t => t.Rates.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(t => t.Gid == gid && !t.IsDeleted);
    }

    public async Task<bool> ExistsByNameAsync(string name, long? excludeId = null)
    {
        var query = _context.Tariffs.Where(t => t.Name == name && !t.IsDeleted);
        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId);
        return await query.AnyAsync();
    }

    public async Task CreateAsync(Tariff tariff)
    {
        _context.Tariffs.Add(tariff);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Tariff tariff)
    {
        _context.Tariffs.Update(tariff);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tariff tariff)
    {
        tariff.IsDeleted = true;
        tariff.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task ClearDefaultFlagAsync(long? excludeId = null)
    {
        var query = _context.Tariffs.Where(t => t.IsDefault && !t.IsDeleted);
        if (excludeId.HasValue)
            query = query.Where(t => t.Id != excludeId);

        var tariffs = await query.ToListAsync();
        foreach (var t in tariffs)
            t.IsDefault = false;

        await _context.SaveChangesAsync();
    }
}
```

### 4.6 Krok 6: Service

```csharp
// ITariffService.cs
using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;

public interface ITariffService
{
    Task<Result<PagedResult<TariffResponse>>> GetListAsync(PortalAuthInfo auth, TariffListFilter filter);
    Task<Result<TariffResponse>> GetByGidAsync(PortalAuthInfo auth, string gid);
    Task<Result<TariffResponse>> CreateAsync(PortalAuthInfo auth, CreateTariffRequest request);
    Task<Result<TariffResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateTariffRequest request);
    Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid);
}

// TariffService.cs
public class TariffService : ITariffService
{
    private readonly ITariffDataHandler _dataHandler;

    public TariffService(ITariffDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<PagedResult<TariffResponse>>> GetListAsync(
        PortalAuthInfo auth, TariffListFilter filter)
    {
        var (items, totalCount) = await _dataHandler.GetPagedAsync(filter);

        var result = new PagedResult<TariffResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Result<PagedResult<TariffResponse>>.Success(result);
    }

    public async Task<Result<TariffResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var tariff = await _dataHandler.GetByGidAsync(gid);
        if (tariff == null)
            return Result<TariffResponse>.Failure(
                new NotFoundError("TARIFF_NOT_FOUND", $"Taryfa {gid} nie istnieje"));

        return Result<TariffResponse>.Success(MapToResponse(tariff));
    }

    public async Task<Result<TariffResponse>> CreateAsync(
        PortalAuthInfo auth, CreateTariffRequest request)
    {
        // Walidacja
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<TariffResponse>.Failure(
                new ValidationError("Name", "Nazwa jest wymagana"));

        if (request.BillingIncrement <= 0)
            return Result<TariffResponse>.Failure(
                new ValidationError("BillingIncrement", "Interwał musi być większy od 0"));

        // Sprawdź duplikat
        if (await _dataHandler.ExistsByNameAsync(request.Name))
            return Result<TariffResponse>.Failure(
                new BusinessLogicError("NAME_EXISTS", "Taryfa o tej nazwie już istnieje"));

        // Jeśli ustawiamy jako domyślną, wyczyść flagę u innych
        if (request.IsDefault)
            await _dataHandler.ClearDefaultFlagAsync();

        // Utwórz
        var tariff = new Tariff
        {
            Name = request.Name,
            Description = request.Description,
            CurrencyCode = request.CurrencyCode,
            IsDefault = request.IsDefault,
            ValidFrom = request.ValidFrom ?? DateTime.UtcNow,
            ValidTo = request.ValidTo,
            BillingIncrement = request.BillingIncrement,
            MinimumDuration = request.MinimumDuration,
            ConnectionFee = request.ConnectionFee,
            CreatedByUserId = auth.UserId
        };

        await _dataHandler.CreateAsync(tariff);
        return Result<TariffResponse>.Success(MapToResponse(tariff));
    }

    public async Task<Result<TariffResponse>> UpdateAsync(
        PortalAuthInfo auth, string gid, UpdateTariffRequest request)
    {
        var tariff = await _dataHandler.GetByGidAsync(gid);
        if (tariff == null)
            return Result<TariffResponse>.Failure(
                new NotFoundError("TARIFF_NOT_FOUND", $"Taryfa {gid} nie istnieje"));

        // Walidacja
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<TariffResponse>.Failure(
                new ValidationError("Name", "Nazwa jest wymagana"));

        // Sprawdź duplikat (z wykluczeniem siebie)
        if (await _dataHandler.ExistsByNameAsync(request.Name, tariff.Id))
            return Result<TariffResponse>.Failure(
                new BusinessLogicError("NAME_EXISTS", "Taryfa o tej nazwie już istnieje"));

        // Jeśli ustawiamy jako domyślną, wyczyść flagę u innych
        if (request.IsDefault && !tariff.IsDefault)
            await _dataHandler.ClearDefaultFlagAsync(tariff.Id);

        // Aktualizuj
        tariff.Name = request.Name;
        tariff.Description = request.Description;
        tariff.CurrencyCode = request.CurrencyCode;
        tariff.IsDefault = request.IsDefault;
        tariff.IsActive = request.IsActive;
        tariff.ValidFrom = request.ValidFrom ?? tariff.ValidFrom;
        tariff.ValidTo = request.ValidTo;
        tariff.BillingIncrement = request.BillingIncrement;
        tariff.MinimumDuration = request.MinimumDuration;
        tariff.ConnectionFee = request.ConnectionFee;
        tariff.ModifiedByUserId = auth.UserId;

        await _dataHandler.UpdateAsync(tariff);
        return Result<TariffResponse>.Success(MapToResponse(tariff));
    }

    public async Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid)
    {
        var tariff = await _dataHandler.GetByGidAsync(gid);
        if (tariff == null)
            return Result<bool>.Failure(
                new NotFoundError("TARIFF_NOT_FOUND", $"Taryfa {gid} nie istnieje"));

        if (tariff.IsDefault)
            return Result<bool>.Failure(
                new BusinessLogicError("CANNOT_DELETE_DEFAULT", "Nie można usunąć domyślnej taryfy"));

        await _dataHandler.DeleteAsync(tariff);
        return Result<bool>.Success(true);
    }

    private static TariffResponse MapToResponse(Tariff tariff) => new()
    {
        Gid = tariff.Gid,
        Name = tariff.Name,
        Description = tariff.Description,
        CurrencyCode = tariff.CurrencyCode,
        IsDefault = tariff.IsDefault,
        IsActive = tariff.IsActive,
        ValidFrom = tariff.ValidFrom,
        ValidTo = tariff.ValidTo,
        BillingIncrement = tariff.BillingIncrement,
        MinimumDuration = tariff.MinimumDuration,
        ConnectionFee = tariff.ConnectionFee,
        RatesCount = tariff.Rates?.Count ?? 0,
        CreatedAt = tariff.CreatedAt
    };
}
```

### 4.7 Krok 7: Endpoint

```csharp
// Endpoint.cs
using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using Microsoft.AspNetCore.Mvc;

namespace NowyServis.Api.Features.Tariffs;

public static class Endpoint
{
    private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops];

    public static void MapTariffEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/nowyservis/tariffs")
            .WithTags("Tariffs")
            .RequireAuthorization();

        // POST /api/nowyservis/tariffs/list
        group.MapPost("/list", GetList)
            .WithName("GetTariffsList")
            .WithSummary("Lista taryf")
            .Produces<PagedResult<TariffResponse>>()
            .Produces<ApiErrorResponse>(400);

        // GET /api/nowyservis/tariffs/{gid}
        group.MapGet("/{gid}", GetByGid)
            .WithName("GetTariffByGid")
            .WithSummary("Pobierz taryfę")
            .Produces<TariffResponse>()
            .Produces<ApiErrorResponse>(404);

        // POST /api/nowyservis/tariffs
        group.MapPost("/", Create)
            .WithName("CreateTariff")
            .WithSummary("Utwórz taryfę")
            .Produces<TariffResponse>()
            .Produces<ApiErrorResponse>(400);

        // PUT /api/nowyservis/tariffs/{gid}
        group.MapPut("/{gid}", Update)
            .WithName("UpdateTariff")
            .WithSummary("Aktualizuj taryfę")
            .Produces<TariffResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        // DELETE /api/nowyservis/tariffs/{gid}
        group.MapDelete("/{gid}", Delete)
            .WithName("DeleteTariff")
            .WithSummary("Usuń taryfę")
            .Produces<bool>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        ITariffService service,
        [FromBody] TariffListFilter filter)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            filter,
            async (auth, req) => await service.GetListAsync(auth, req),
            AllRoles);
    }

    private static async Task<IResult> GetByGid(
        ClaimsPrincipal user,
        ITariffService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByGidAsync(auth, gid),
            AllRoles);
    }

    private static async Task<IResult> Create(
        ClaimsPrincipal user,
        ITariffService service,
        [FromBody] CreateTariffRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.CreateAsync(auth, req),
            AllRoles);
    }

    private static async Task<IResult> Update(
        ClaimsPrincipal user,
        ITariffService service,
        string gid,
        [FromBody] UpdateTariffRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.UpdateAsync(auth, gid, req),
            AllRoles);
    }

    private static async Task<IResult> Delete(
        ClaimsPrincipal user,
        ITariffService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.DeleteAsync(auth, gid),
            AllRoles);
    }
}
```

### 4.8 Krok 8: Rejestracja DI

```csharp
// Infrastructure/DependencyInjection.cs
using Microsoft.EntityFrameworkCore;

namespace NowyServis.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<NowyServisDbContext>(options =>
        {
            options.UseMySQL(connectionString!);
        });

        // Tariffs
        services.AddScoped<ITariffDataHandler, TariffDataHandler>();
        services.AddScoped<ITariffService, TariffService>();

        // Seed
        services.AddScoped<ISeedService, SeedService>();

        return services;
    }
}

// Infrastructure/EndpointExtensions.cs
namespace NowyServis.Api.Infrastructure;

public static class EndpointExtensions
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapTariffEndpoints();

        // Health check
        app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
    }
}
```

---

## 5. Testowanie

### 5.1 Struktura testów

```
NowyServis.Api.Tests/
├── Infrastructure/
│   ├── NowyServisMySqlTestContainer.cs  # Dziedzczy z MySqlTestContainerBase
│   ├── NowyServisDatabaseCollection.cs  # xUnit Collection
│   ├── NowyServisApplicationFactory.cs  # WebApplicationFactory
│   └── NowyServisTestDataSeeder.cs      # Dane testowe
├── TariffsEndpointTests.cs
└── HealthEndpointTests.cs
```

### 5.2 MySQL Test Container

Dziedzicz z `MySqlTestContainerBase` z `App.Shared.Tests.Infrastructure`:

```csharp
// Infrastructure/NowyServisMySqlTestContainer.cs
using App.Shared.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NowyServis.Data;

namespace NowyServis.Api.Tests.Infrastructure;

public class NowyServisMySqlTestContainer : MySqlTestContainerBase
{
    protected override async Task InitializeDatabaseAsync()
    {
        var options = new DbContextOptionsBuilder<NowyServisDbContext>()
            .UseMySQL(ConnectionString)
            .Options;

        await using var context = new NowyServisDbContext(options);

        // Apply migrations
        await context.Database.MigrateAsync();

        // Seed test data
        NowyServisTestDataSeeder.SeedTestData(context);
        await context.SaveChangesAsync();
    }
}
```

### 5.3 Database Collection

```csharp
// Infrastructure/NowyServisDatabaseCollection.cs
using Xunit;

namespace NowyServis.Api.Tests.Infrastructure;

[CollectionDefinition(Name)]
public class NowyServisDatabaseCollection : ICollectionFixture<NowyServisMySqlTestContainer>
{
    public const string Name = "NowyServisDatabase";
}
```

### 5.4 Application Factory

```csharp
// Infrastructure/NowyServisApplicationFactory.cs
using App.Shared.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NowyServis.Data;

namespace NowyServis.Api.Tests.Infrastructure;

public class NowyServisApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Action<TestAuthHandlerOptions>? _configureAuth;

    public NowyServisApplicationFactory(string connectionString, Action<TestAuthHandlerOptions>? configureAuth = null)
    {
        _connectionString = connectionString;
        _configureAuth = configureAuth;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<NowyServisDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Add test DbContext with MySQL container connection string
            services.AddDbContext<NowyServisDbContext>(options =>
            {
                options.UseMySQL(_connectionString);
            });

            // Configure test authentication
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                })
                .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme,
                    options =>
                    {
                        _configureAuth?.Invoke(options);
                    });
        });

        builder.UseEnvironment("Testing");
    }

    public async Task SeedTestDataAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NowyServisDbContext>();

        await context.Database.EnsureCreatedAsync();
        await NowyServisTestDataSeeder.SeedAsync(context);
    }
}
```

### 5.5 Test Data Seeder

```csharp
// Infrastructure/NowyServisTestDataSeeder.cs
namespace NowyServis.Api.Tests.Infrastructure;

public static class NowyServisTestDataSeeder
{
    public static readonly string TestTariffGid = "test0001000100010001000100010001";

    public static void SeedTestData(NowyServisDbContext context)
    {
        // Test tariff
        var tariff = new Tariff
        {
            Gid = TestTariffGid,
            Name = "Test Tariff",
            CurrencyCode = "PLN",
            IsActive = true,
            BillingIncrement = 60
        };
        context.Tariffs.Add(tariff);
    }

    public static async Task SeedAsync(NowyServisDbContext context)
    {
        SeedTestData(context);
        await context.SaveChangesAsync();
    }
}
```

### 5.6 Przykładowy test

```csharp
// TariffsEndpointTests.cs
using System.Net;
using System.Net.Http.Json;
using App.Bps.Enum;
using App.Shared.Tests.Infrastructure;
using NowyServis.Api.Features.Tariffs.Model;
using NowyServis.Api.Tests.Infrastructure;
using Xunit;

namespace NowyServis.Api.Tests;

[Collection(NowyServisDatabaseCollection.Name)]
public class TariffsEndpointTests : IAsyncLifetime
{
    private readonly NowyServisApplicationFactory _factory;
    private readonly HttpClient _client;

    public TariffsEndpointTests(NowyServisMySqlTestContainer container)
    {
        _factory = new NowyServisApplicationFactory(
            container.ConnectionString,
            options =>
            {
                // Configure test user with Root role
                options.Roles = [Roles.Root];
            });
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.SeedTestDataAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetList_ReturnsTariffs()
    {
        // Arrange
        var filter = new TariffListFilter { PageNumber = 1, PageSize = 10 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/nowyservis/tariffs/list", filter);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetByGid_ExistingTariff_ReturnsTariff()
    {
        // Act
        var response = await _client.GetAsync(
            $"/api/nowyservis/tariffs/{NowyServisTestDataSeeder.TestTariffGid}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tariff = await response.Content.ReadFromJsonAsync<TariffResponse>();
        Assert.NotNull(tariff);
        Assert.Equal("Test Tariff", tariff.Name);
    }

    [Fact]
    public async Task GetByGid_NonExistingTariff_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/nowyservis/tariffs/non-existing-gid");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedTariff()
    {
        // Arrange
        var request = new CreateTariffRequest
        {
            Name = $"New Tariff {Guid.NewGuid():N}",
            Description = "New tariff description",
            CurrencyCode = "EUR",
            BillingIncrement = 30
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/nowyservis/tariffs", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tariff = await response.Content.ReadFromJsonAsync<TariffResponse>();
        Assert.NotNull(tariff);
        Assert.Contains("New Tariff", tariff.Name);
        Assert.Equal("EUR", tariff.CurrencyCode);
        Assert.NotEmpty(tariff.Gid);
    }

    [Fact]
    public async Task Create_EmptyName_Returns400()
    {
        // Arrange
        var request = new CreateTariffRequest
        {
            Name = "",
            CurrencyCode = "PLN"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/nowyservis/tariffs", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
```

### 5.7 Uruchamianie testów

```bash
# Wszystkie testy
cd src/backend/Services/NowyServis/NowyServis.Api.Tests
dotnet test

# Z verbose output
dotnet test --logger "console;verbosity=detailed"

# Konkretna klasa testów
dotnet test --filter "FullyQualifiedName~TariffsEndpointTests"

# Konkretny test
dotnet test --filter "FullyQualifiedName~TariffsEndpointTests.GetByGid_ExistingTariff_ReturnsTariff"
```

---

## 6. Seedowanie danych

### 6.1 Struktura seedowania

```
NowyServis.Api/
└── Seed/
    ├── ISeedService.cs          # Interfejs (lub użyj z App.Shared.Web)
    ├── SeedService.cs           # Główny serwis seedowania
    ├── ShowcaseData.cs          # Dane przykładowe
    └── FixedGuids.cs            # Stałe GUIDy (dla powtarzalności)
```

### 6.2 Fixed GUIDs

```csharp
// Seed/FixedGuids.cs
namespace NowyServis.Api.Seed;

public static class FixedGuids
{
    // Tariffs
    public static readonly string DefaultTariff = "t1111111111111111111111111111111";
    public static readonly string PremiumTariff = "t2222222222222222222222222222222";
}
```

### 6.3 Showcase Data

```csharp
// Seed/ShowcaseData.cs
namespace NowyServis.Api.Seed;

public static class ShowcaseData
{
    public static List<Tariff> GetTariffs() =>
    [
        new Tariff
        {
            Gid = FixedGuids.DefaultTariff,
            Name = "Standard PL",
            Description = "Domyślna taryfa dla Polski",
            CurrencyCode = "PLN",
            IsDefault = true,
            IsActive = true,
            BillingIncrement = 60,
            MinimumDuration = 0,
            ConnectionFee = 0
        },
        new Tariff
        {
            Gid = FixedGuids.PremiumTariff,
            Name = "Premium EU",
            Description = "Taryfa premium dla krajów EU",
            CurrencyCode = "EUR",
            IsDefault = false,
            IsActive = true,
            BillingIncrement = 30,
            MinimumDuration = 30,
            ConnectionFee = 0.10m
        }
    ];
}
```

### 6.4 Seed Service

```csharp
// Seed/SeedService.cs
using App.Shared.Web;
using Microsoft.EntityFrameworkCore;

namespace NowyServis.Api.Seed;

public class SeedService : ISeedService
{
    private readonly NowyServisDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SeedService> _logger;

    public SeedService(
        NowyServisDbContext context,
        IConfiguration configuration,
        ILogger<SeedService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        var seedConfig = _configuration.GetSection("Seed").Get<SeedSettings>();

        if (seedConfig?.RunOnStartup != true)
        {
            _logger.LogInformation("Seed disabled in configuration");
            return;
        }

        _logger.LogInformation("Starting database seed...");

        await SeedTariffsAsync(seedConfig.IncludeShowcaseData);

        _logger.LogInformation("Database seed completed");
    }

    private async Task SeedTariffsAsync(bool includeShowcase)
    {
        if (!includeShowcase) return;

        var tariffs = ShowcaseData.GetTariffs();

        foreach (var tariff in tariffs)
        {
            var exists = await _context.Tariffs
                .AnyAsync(t => t.Gid == tariff.Gid);

            if (!exists)
            {
                _context.Tariffs.Add(tariff);
                _logger.LogInformation("Seeded tariff: {Name}", tariff.Name);
            }
        }

        await _context.SaveChangesAsync();
    }
}
```

### 6.5 Database Extensions

```csharp
// Infrastructure/DatabaseExtensions.cs
using Microsoft.EntityFrameworkCore;

namespace NowyServis.Api.Infrastructure;

public static class DatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NowyServisDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // Migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                await context.Database.MigrateAsync();
            }

            // Seed
            var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
            await seedService.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error initializing database");
            throw;
        }
    }
}
```

---

## 7. Integracja z Gateway

### 7.1 Konfiguracja YARP

Dodaj routing do nowego serwisu w Gateway (`src/backend/Services/Gateway/Gateway.Api/appsettings.json`):

```json
{
  "ReverseProxy": {
    "Routes": {
      "nowyservis-route": {
        "ClusterId": "nowyservis-cluster",
        "AuthorizationPolicy": "default",
        "Match": {
          "Path": "/api/nowyservis/{**catch-all}"
        },
        "Transforms": [
          { "PathPattern": "/api/{**catch-all}" }
        ]
      }
    },
    "Clusters": {
      "nowyservis-cluster": {
        "Destinations": {
          "nowyservis-destination": {
            "Address": "http://nowyservis:8080"
          }
        }
      }
    }
  }
}
```

### 7.2 Przekazywanie autentykacji

Gateway waliduje JWT i przekazuje dane użytkownika w headerach X-User-*:

```
X-User-Id: 123
X-User-Gid: abc123def456
X-User-FirstName: Jan
X-User-LastName: Kowalski
X-User-Email: jan@example.com
X-User-Roles: Root,Ops
```

Mikroserwis odczytuje te headers przez `GatewayAuthHandler` z `App.Shared.Web.Security`.

### 7.3 Konfiguracja Swagger Aggregation

Gateway agreguje dokumentację Swagger ze wszystkich mikroserwisów. Aby nowy serwis był widoczny w Swagger UI Gateway, musisz wykonać **dwa kroki**:

#### Krok 1: Dodaj serwis do SwaggerAggregator

W pliku `src/backend/Services/Gateway/Gateway.Api/Swagger/SwaggerAggregator.cs` dodaj wpis do tablicy `Services`:

```csharp
private static readonly ServiceSwaggerInfo[] Services =
[
    new("Identity", "/api/identity", "identity-cluster"),
    new("DataSource", "/api/datasource", "datasource-cluster"),
    new("Rcp", "/api/rcp", "rcp-cluster"),
    new("Rate", "/api/rate", "rate-cluster"),
    // Dodaj nowy serwis:
    new("NowyServis", "/api/nowyservis", "nowyservis-cluster"),
];
```

Parametry `ServiceSwaggerInfo`:
- `Name` - nazwa serwisu (używana w URL `/api-docs/{name}/swagger.json`)
- `Prefix` - prefix ścieżek API (np. `/api/nowyservis`)
- `ClusterKey` - klucz clustra z konfiguracji YARP (musi pasować do `appsettings.json`)

#### Krok 2: Dodaj endpoint w Program.cs (opcjonalnie)

Jeśli chcesz mieć dedykowany endpoint w dropdown Swagger UI, dodaj go w `Program.cs` Gateway:

```csharp
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway");
    c.SwaggerEndpoint("/api-docs/identity/swagger.json", "Identity API");
    c.SwaggerEndpoint("/api-docs/rate/swagger.json", "Rate API");
    // Dodaj nowy serwis:
    c.SwaggerEndpoint("/api-docs/nowyservis/swagger.json", "NowyServis API");
});
```

#### Jak to działa

1. Gateway udostępnia endpoint `/api-docs/{service}/swagger.json`
2. `SwaggerAggregator` pobiera Swagger z mikroserwisu (`http://nowyservis:8080/swagger/v1/swagger.json`)
3. Transformuje ścieżki (np. `/api/tariffs` → `/api/nowyservis/tariffs`)
4. Zwraca zmodyfikowany dokument OpenAPI

#### Troubleshooting

**Swagger nie pokazuje nowego serwisu:**
1. Sprawdź czy serwis jest dodany do `SwaggerAggregator.Services[]`
2. Sprawdź czy cluster jest zdefiniowany w `appsettings.json` (sekcja Routes i Clusters)
3. Sprawdź czy mikroserwis ma włączony Swagger (`app.UseSwagger()`)
4. Sprawdź logi Gateway - powinny być informacje o pobieraniu swagger.json

**Przykład kompletnej konfiguracji dla nowego serwisu:**

`appsettings.json`:
```json
{
  "ReverseProxy": {
    "Routes": {
      "nowyservis-route": {
        "ClusterId": "nowyservis-cluster",
        "AuthorizationPolicy": "default",
        "Match": { "Path": "/api/nowyservis/{**catch-all}" },
        "Transforms": [{ "PathPattern": "/api/{**catch-all}" }]
      }
    },
    "Clusters": {
      "nowyservis-cluster": {
        "Destinations": {
          "nowyservis-destination": {
            "Address": "http://nowyservis:8080"
          }
        }
      }
    }
  }
}
```

`SwaggerAggregator.cs`:
```csharp
new("NowyServis", "/api/nowyservis", "nowyservis-cluster"),
```

`Program.cs`:
```csharp
c.SwaggerEndpoint("/api-docs/nowyservis/swagger.json", "NowyServis API");
```

---

## 8. Migracje bazy danych

> **POC/MVP**: W fazie POC/MVP migracje mogą być usuwane i tworzone od nowa. Jeśli schemat bazy wymaga znaczących zmian, można usunąć folder `Migrations/` i utworzyć nową Initial migration.

### 8.1 Tworzenie migracji

```bash
cd src/backend/Services/NowyServis/NowyServis.Data

# Dodaj nową migrację
dotnet ef migrations add NazwaMigracji \
  --startup-project ../NowyServis.Api/NowyServis.Api.csproj

# Zastosuj migracje
dotnet ef database update \
  --startup-project ../NowyServis.Api/NowyServis.Api.csproj
```

### 8.2 Reset migracji (POC/MVP)

W fazie POC można zresetować migracje i stworzyć od nowa:

```bash
cd src/backend/Services/NowyServis/NowyServis.Data

# 1. Usuń folder Migrations
rm -rf Migrations/

# 2. Usuń bazę danych (opcjonalnie)
dotnet ef database drop --startup-project ../NowyServis.Api/NowyServis.Api.csproj --force

# 3. Utwórz nową Initial migration
dotnet ef migrations add Initial \
  --startup-project ../NowyServis.Api/NowyServis.Api.csproj

# 4. Zastosuj migrację
dotnet ef database update \
  --startup-project ../NowyServis.Api/NowyServis.Api.csproj
```

### 8.3 Automatyczne migracje przy starcie

Migracje są aplikowane automatycznie w `InitializeDatabaseAsync()` - patrz sekcja 6.5.

### 8.4 Rollback migracji

```bash
# Cofnij do konkretnej migracji
dotnet ef database update NazwaPoprzedniejMigracji \
  --startup-project ../NowyServis.Api/NowyServis.Api.csproj

# Usuń ostatnią migrację (jeśli nie została zastosowana)
dotnet ef migrations remove \
  --startup-project ../NowyServis.Api/NowyServis.Api.csproj
```

---

## 9. Uruchamianie projektu

### 9.1 Wymagania

- .NET 8 SDK (8.0.406+)
- MySQL 8.0+
- Docker (dla Testcontainers)

### 9.2 Uruchomienie MySQL

```bash
# Docker
docker run -d \
  --name pbx-mysql \
  -e MYSQL_ROOT_PASSWORD=password \
  -e MYSQL_DATABASE=pbx_rates \
  -p 3306:3306 \
  mysql:8.0
```

### 9.3 Uruchomienie serwisów

```bash
# Terminal 1 - Gateway
cd src/backend/Services/Gateway/Gateway.Api
dotnet run

# Terminal 2 - Identity
cd src/backend/Services/Identity/Identity.Api
dotnet run

# Terminal 3 - RateService
cd src/backend/Services/RateService/Rate.Api
dotnet run
```

### 9.4 Adresy

| Komponent | URL |
|-----------|-----|
| Gateway | http://localhost:5000 |
| Gateway Swagger | http://localhost:5000/swagger |
| Identity Swagger | http://localhost:5293/swagger |
| RateService Swagger | http://localhost:5010/swagger |

---

## 10. Checklist nowego mikroserwisu

### Projekty
- [ ] Utworzono `NazwaSerwisu.Api` (web)
- [ ] Utworzono `NazwaSerwisu.Data` (classlib)
- [ ] Utworzono `NazwaSerwisu.Api.Tests` (xunit)
- [ ] Dodano referencje między projektami
- [ ] Dodano pakiety NuGet

### Konfiguracja
- [ ] `Program.cs` z pełną konfiguracją
- [ ] `appsettings.json` z connection string
- [ ] `appsettings.Development.json` z seed config
- [ ] `Infrastructure/DependencyInjection.cs`
- [ ] `Infrastructure/EndpointExtensions.cs`
- [ ] `Infrastructure/DatabaseExtensions.cs`

### Baza danych
- [ ] `DbContext`
- [ ] Encje w `Entities/`
- [ ] Konfiguracje w `Configurations/`
- [ ] Pierwsza migracja

### Seedowanie
- [ ] `Seed/FixedGuids.cs`
- [ ] `Seed/ShowcaseData.cs`
- [ ] `Seed/SeedService.cs`

### Features
- [ ] Każda feature w osobnym katalogu
- [ ] `Endpoint.cs` - routing
- [ ] `I{Feature}Service.cs` - interfejs
- [ ] `{Feature}Service.cs` - implementacja
- [ ] `I{Feature}DataHandler.cs` - interfejs
- [ ] `{Feature}DataHandler.cs` - implementacja
- [ ] `Model/` - DTOs

### Testy
- [ ] `Infrastructure/MySqlTestContainer.cs` (dziedzicz z MySqlTestContainerBase)
- [ ] `Infrastructure/DatabaseCollection.cs`
- [ ] `Infrastructure/ApplicationFactory.cs`
- [ ] `Infrastructure/TestDataSeeder.cs`
- [ ] Testy dla każdej feature

### Integracja z Gateway
- [ ] Routing YARP w Gateway `appsettings.json` (Routes + Clusters)
- [ ] Wpis w `SwaggerAggregator.Services[]` dla agregacji Swagger
- [ ] Endpoint w `Program.cs` Swagger UI (opcjonalnie)
- [ ] Dodano do solution (`dotnet sln add`)

---

## Kontakt

W razie pytań lub problemów:
- Sprawdź istniejące serwisy (RateService, Rcp) jako wzorzec
- Przejrzyj `src/docs/plan-projekt.md` dla specyfikacji domeny PBX
- Sprawdź `src/docs/authentication-architecture.md` dla szczegółów autentykacji
