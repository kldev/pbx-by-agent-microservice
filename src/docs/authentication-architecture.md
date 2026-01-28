# Authentication Architecture

## Overview

System wykorzystuje architekturę gdzie **Gateway jest jedynym punktem odpowiedzialnym za tworzenie i walidację tokenów JWT**. Mikrousługi (Identity, JdJobs, JdSales) nie walidują JWT samodzielnie - zamiast tego ufają nagłówkom X-User-* przekazywanym przez Gateway.

```
┌─────────┐     JWT Token      ┌─────────┐    X-User-* Headers    ┌──────────────┐
│  Client │ ◄───────────────── │ Gateway │ ─────────────────────► │ Microservice │
│         │ ──────────────────►│         │                        │              │
└─────────┘   Login Request    └─────────┘                        └──────────────┘
                                    │
                                    │ Validate credentials
                                    ▼
                              ┌──────────┐
                              │ Identity │
                              └──────────┘
```

## Flow logowania

### 1. Login Request
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@jdapp.local",
  "password": "Agent666"
}
```

### 2. Gateway przetwarza login

Gateway (`Gateway.Api/Auth/AuthEndpoint.cs`):
1. Odbiera request od klienta
2. Wywołuje Identity service przez HTTP client
3. Identity waliduje credentials i zwraca dane użytkownika (bez JWT)
4. Gateway tworzy token JWT z danymi użytkownika
5. Zwraca token do klienta

```csharp
// Gateway wywołuje Identity
var validationResult = await identityClient.ValidateLoginAsync(request);

if (validationResult.IsValid)
{
    // Gateway tworzy JWT
    var token = jwtService.GenerateToken(validationResult);
    return Results.Ok(new LoginResponse(Token: token, ...));
}
```

### 3. Identity waliduje credentials

Identity (`Identity.Api/Features/Auth/AuthService.cs`):
- Sprawdza czy użytkownik istnieje
- Weryfikuje hasło (BCrypt)
- Zwraca dane użytkownika bez tworzenia JWT

```csharp
public async Task<ValidateLoginResponse> ValidateLoginAsync(LoginRequest request)
{
    var user = await _userDataHandler.GetByEmailAsync(request.Email);

    if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
        return new ValidateLoginResponse(IsValid: false, ...);

    return new ValidateLoginResponse(
        IsValid: true,
        UserId: user.Id,
        Gid: user.Gid,
        Email: user.Email,
        Role: user.Role.ToString(),
        ...);
}
```

### 4. Response do klienta

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "gid": "usr-admin-001",
  "email": "admin@jdapp.local",
  "firstName": "Admin",
  "lastName": "User",
  "role": "Admin",
  "expiresAt": "2025-12-18T18:59:00Z"
}
```

## Flow autoryzowanych requestów

### 1. Klient wysyła request z JWT

```
GET /api/identity/sbu/all
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### 2. Gateway waliduje JWT i przekazuje request

Gateway (`Gateway.Api/Transforms/UserHeadersTransform.cs`):
1. Waliduje token JWT
2. Wyciąga claims z tokena
3. Dodaje nagłówki X-User-* do request
4. Usuwa nagłówek Authorization
5. Przekazuje request do mikrousługi

```csharp
public override ValueTask ApplyAsync(RequestTransformContext context)
{
    var user = context.HttpContext.User;

    if (user.Identity?.IsAuthenticated == true)
    {
        context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", user.FindFirst("UserId")?.Value);
        context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Gid", user.FindFirst("Gid")?.Value);
        context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Email", user.FindFirst(ClaimTypes.Email)?.Value);
        context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Role", user.FindFirst(ClaimTypes.Role)?.Value);
        // ...
    }

    // Usuń Authorization header - mikrousługi nie potrzebują JWT
    context.ProxyRequest.Headers.Remove("Authorization");
}
```

### 3. Mikrousługa odczytuje X-User-* headers

Mikrousługi używają `GatewayAuthHandler` (`App.Shared.Web/Security/GatewayAuthHandler.cs`):

```csharp
protected override Task<AuthenticateResult> HandleAuthenticateAsync()
{
    var userGid = Context.GetUserGid(); // z X-User-Gid header

    if (string.IsNullOrEmpty(userGid))
        return Task.FromResult(AuthenticateResult.NoResult());

    var claims = new List<Claim>
    {
        new("UserId", Context.GetUserId()?.ToString() ?? ""),
        new("Gid", userGid),
        new(ClaimTypes.Email, Context.GetUserEmail() ?? ""),
        new(ClaimTypes.Role, Context.GetUserRole() ?? ""),
        // ...
    };

    var identity = new ClaimsIdentity(claims, SchemeName);
    var principal = new ClaimsPrincipal(identity);

    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
}
```

## Struktura plików

### Gateway
```
Gateway.Api/
├── Auth/
│   ├── AuthEndpoint.cs      # POST /api/auth/login
│   ├── IdentityClient.cs    # HTTP client do Identity
│   ├── JwtService.cs        # Tworzenie tokenów JWT
│   └── Models.cs            # LoginRequest, LoginResponse, ValidateLoginResponse
├── Transforms/
│   ├── UserHeadersTransform.cs      # Dodaje X-User-* headers
│   └── UserHeadersTransformProvider.cs
└── Configuration/
    └── JwtSettings.cs
```

### Identity
```
Identity.Api/
├── Features/Auth/
│   ├── AuthService.cs       # Walidacja credentials
│   ├── IAuthService.cs
│   ├── Endpoint.cs          # POST /api/auth/validate-login (ukryty)
│   └── Model/
│       ├── LoginRequest.cs
│       └── ValidateLoginResponse.cs
└── Program.cs               # Używa GatewayAuthHandler
```

### Shared
```
App.Shared.Web/
└── Security/
    ├── GatewayAuthHandler.cs      # Authentication z X-User-* headers
    ├── HttpContextExtensions.cs    # GetUserGid(), GetUserRole(), etc.
    └── ClaimsPrincipalExtensions.cs
```

## Nagłówki X-User-*

| Header | Opis | Przykład |
|--------|------|----------|
| X-User-Id | Numeryczny ID użytkownika | `1` |
| X-User-Gid | Unikalny identyfikator (GID) | `usr-admin-001` |
| X-User-Email | Email użytkownika | `admin@jdapp.local` |
| X-User-FirstName | Imię | `Admin` |
| X-User-LastName | Nazwisko | `User` |
| X-User-Role | Rola użytkownika | `Admin` |
| X-User-StructreId | ID struktury | `1` |

## JWT Claims

Token JWT zawiera następujące claims:

```json
{
  "UserId": "1",
  "Gid": "usr-admin-001",
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": "admin@jdapp.local",
  "FirstName": "Admin",
  "LastName": "User",
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Admin",
  "StructureId": 1,
  "exp": 1766084340,
  "iss": "PbxApp",
  "aud": "PbxApp"
}
```

## Konfiguracja

### Gateway (appsettings.json)
```json
{
  "Jwt": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "JdApp",
    "Audience": "JdApp",
    "ExpirationMinutes": 60
  }
}
```

### Mikrousługi (Program.cs)
```csharp
// Rejestracja GatewayAuthHandler
builder.Services.AddAuthentication(GatewayAuthHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, GatewayAuthHandler>(GatewayAuthHandler.SchemeName, null);
builder.Services.AddAuthorization();

// W pipeline
app.UseAuthentication();
app.UseAuthorization();
```

## Zalety architektury

1. **Centralizacja JWT** - tylko Gateway zna sekret JWT
2. **Uproszczenie mikrousług** - nie muszą walidować JWT
3. **Bezpieczeństwo** - mikrousługi nie są dostępne bezpośrednio z zewnątrz
4. **Elastyczność** - łatwa zmiana mechanizmu autoryzacji w Gateway
5. **Wydajność** - mikrousługi nie wykonują kryptografii JWT

## Testowanie

```bash
# Login
http POST localhost:5000/api/auth/login email=admin@jdapp.local password=Agent666

# Autoryzowane requesty
TOKEN="eyJhbG..."
http GET localhost:5000/api/identity/auth/me "Authorization:Bearer $TOKEN"
http GET localhost:5000/api/identity/structure/all "Authorization:Bearer $TOKEN"
http GET localhost:5000/api/jobs/clients "Authorization:Bearer $TOKEN"
```

## Użytkownicy testowi

| Email | Hasło | Rola |
|-------|-------|------|
| admin@jdapp.local | Agent666 | Admin |
| sales@jdapp.local | Agent666 | Sales |
| recruiter@jdapp.local | Agent666 | Recruiter |
| hr@jdapp.local | Agent666 | HR |
| ops@jdapp.local | Agent666 | Operations |
