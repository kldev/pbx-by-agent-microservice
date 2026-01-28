using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using Microsoft.AspNetCore.Http;

namespace App.Shared.Web.ApiResultPattern;

public static class EndpointHelpers
{
    public static async Task<IResult> HandleAuthRequestAsync<TRequest, TResponse>(
        ClaimsPrincipal user,
        TRequest request,
        Func<PortalAuthInfo, TRequest, Task<Result<TResponse>>> handler,
        params AppRole[] allowedRoles)
    {
        if (user.IsUnauthorized(allowedRoles))
            return Results.Forbid();

        var authInfo = user.GetAuthInfo();
        if (authInfo == null)
            return Results.Unauthorized();

        var result = await handler(authInfo, request);
        return result.ToApiResult();
    }

    public static async Task<IResult> HandleAuthRequestAsync<TResponse>(
        ClaimsPrincipal user,
        Func<PortalAuthInfo, Task<Result<TResponse>>> handler,
        params AppRole[] allowedRoles)
    {
        if (user.IsUnauthorized(allowedRoles))
            return Results.Forbid();

        var authInfo = user.GetAuthInfo();
        if (authInfo == null)
            return Results.Unauthorized();

        var result = await handler(authInfo);
        return result.ToApiResult();
    }
}
