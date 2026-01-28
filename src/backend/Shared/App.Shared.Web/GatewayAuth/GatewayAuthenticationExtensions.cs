using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace App.Shared.Web.GatewayAuth;

/// <summary>
/// Extension methods for configuring Gateway authentication.
/// </summary>
public static class GatewayAuthenticationExtensions
{
    /// <summary>
    /// Adds Gateway authentication that reads user identity from X-User-* headers.
    /// Use this in downstream microservices that trust the Gateway.
    /// </summary>
    public static AuthenticationBuilder AddGatewayAuthentication(this IServiceCollection services)
    {
        return services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = GatewayAuthenticationHandler.SchemeName;
            options.DefaultChallengeScheme = GatewayAuthenticationHandler.SchemeName;
        })
        .AddScheme<GatewayAuthenticationOptions, GatewayAuthenticationHandler>(
            GatewayAuthenticationHandler.SchemeName,
            _ => { });
    }

    /// <summary>
    /// Adds Gateway authentication scheme to an existing authentication builder.
    /// </summary>
    public static AuthenticationBuilder AddGatewayAuthentication(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<GatewayAuthenticationOptions, GatewayAuthenticationHandler>(
            GatewayAuthenticationHandler.SchemeName,
            _ => { });
    }
}
