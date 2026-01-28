using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Shared.Tests.Infrastructure;

/// <summary>
/// Mock authentication handler for integration tests.
/// Automatically authenticates all requests with configurable claims.
/// Claims are compatible with ClaimsPrincipalExtensions.GetAuthInfo().
/// </summary>
public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    public const string AuthenticationScheme = "TestScheme";

    public TestAuthHandler(
        IOptionsMonitor<TestAuthHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            // Required claims for ClaimsPrincipalExtensions.GetAuthInfo()
            new("UserId", Options.UserId.ToString()),
            new(ClaimTypes.NameIdentifier, Options.UserId.ToString()),
            new("Gid", Options.Gid),
            new("FirstName", Options.FirstName),
            new("LastName", Options.LastName),
            new(ClaimTypes.Email, Options.Email),
        };

        // Add roles (as AppRole enum names for GetRoles() parsing)
        foreach (var role in Options.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add custom claims
        foreach (var claim in Options.CustomClaims)
        {
            claims.Add(new Claim(claim.Key, claim.Value));
        }

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public long UserId { get; set; } = TestFixtureIds.Ids.TestUser1;
    public string Gid { get; set; } = TestFixtureIds.Gids.TestUser1;
    public string FirstName { get; set; } = "Test";
    public string LastName { get; set; } = "User";
    public string Email { get; set; } = "test@example.com";

    /// <summary>
    /// Roles as AppRole enum names: "Admin", "SalesPerson", "HrPoland", "Root", etc.
    /// Default is Admin which has access to most operations.
    /// </summary>
    public string[] Roles { get; set; } = ["Admin"];

    public Dictionary<string, string> CustomClaims { get; set; } = new();
}
