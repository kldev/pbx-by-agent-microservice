using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Shared.Web.GatewayAuth;

/// <summary>
/// Authentication handler that creates a ClaimsPrincipal from X-User-* headers set by the Gateway.
/// Use this in downstream microservices that trust the Gateway.
/// </summary>
public class GatewayAuthenticationHandler : AuthenticationHandler<GatewayAuthenticationOptions>
{
    public const string SchemeName = "Gateway";

    public GatewayAuthenticationHandler(
        IOptionsMonitor<GatewayAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check for X-User-Gid header (required for authentication)
        if (!Request.Headers.TryGetValue("X-User-Gid", out var userGid) || string.IsNullOrEmpty(userGid))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new("Gid", userGid!)
        };

        // X-User-Id
        if (Request.Headers.TryGetValue("X-User-Id", out var userId) && !string.IsNullOrEmpty(userId))
        {
            claims.Add(new Claim("UserId", userId!));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId!));
        }

        // X-User-Email
        if (Request.Headers.TryGetValue("X-User-Email", out var email) && !string.IsNullOrEmpty(email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email!));
        }

        // X-User-FirstName
        if (Request.Headers.TryGetValue("X-User-FirstName", out var firstName) && !string.IsNullOrEmpty(firstName))
        {
            claims.Add(new Claim("FirstName", firstName!));
        }

        // X-User-LastName
        if (Request.Headers.TryGetValue("X-User-LastName", out var lastName) && !string.IsNullOrEmpty(lastName))
        {
            claims.Add(new Claim("LastName", lastName!));
        }

        // X-User-Roles (comma-separated)
        if (Request.Headers.TryGetValue("X-User-Roles", out var roles) && !string.IsNullOrEmpty(roles))
        {
            var roleList = roles.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var role in roleList)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
            }
        }

        // X-User-SbuId
        if (Request.Headers.TryGetValue("X-User-SbuId", out var sbuId) && !string.IsNullOrEmpty(sbuId))
        {
            claims.Add(new Claim("SbuId", sbuId!));
        }

        // X-User-TeamId
        if (Request.Headers.TryGetValue("X-User-TeamId", out var teamId) && !string.IsNullOrEmpty(teamId))
        {
            claims.Add(new Claim("TeamId", teamId!));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
