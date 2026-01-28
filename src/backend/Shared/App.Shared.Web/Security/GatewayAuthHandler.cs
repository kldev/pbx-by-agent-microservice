using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Shared.Web.Security;

public class GatewayAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "GatewayAuth";

    public GatewayAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Read X-User-* headers set by Gateway
        var userGid = Context.GetUserGid();

        // If no X-User-Gid header, user is not authenticated
        if (string.IsNullOrEmpty(userGid))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new("Gid", userGid)
        };

        var email = Context.GetUserEmail();
        if (!string.IsNullOrEmpty(email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email));
        }

        var firstName = Context.GetUserFirstName();
        if (!string.IsNullOrEmpty(firstName))
        {
            claims.Add(new Claim("FirstName", firstName));
        }

        var lastName = Context.GetUserLastName();
        if (!string.IsNullOrEmpty(lastName))
        {
            claims.Add(new Claim("LastName", lastName));
        }

        var roles = Context.GetUserRoles();
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var sbuId = Context.GetUserSbuId();
        if (sbuId.HasValue)
        {
            claims.Add(new Claim("SbuId", sbuId.Value.ToString()));
        }

        var userId = Context.GetUserId();
        if (userId.HasValue)
        {
            claims.Add(new Claim("UserId", userId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
