using System.Security.Claims;
using Yarp.ReverseProxy.Transforms;

namespace Gateway.Api.Transforms;

/// <summary>
/// YARP transform that extracts user claims from JWT and adds them as X-User-* headers.
/// This enables downstream services to trust these headers without validating JWT themselves.
/// </summary>
public class UserHeadersTransform : RequestTransform
{
    public override ValueTask ApplyAsync(RequestTransformContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            // X-User-Gid - unique user identifier
            var gid = user.FindFirst("Gid")?.Value;
            if (!string.IsNullOrEmpty(gid))
            {
                context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Gid", gid);
            }

            // X-User-Id - numeric user ID
            var userId = user.FindFirst("UserId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", userId);
            }

            // X-User-Email
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Email", email);
            }

            // X-User-FirstName
            var firstName = user.FindFirst("FirstName")?.Value;
            if (!string.IsNullOrEmpty(firstName))
            {
                context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-FirstName", firstName);
            }

            // X-User-LastName
            var lastName = user.FindFirst("LastName")?.Value;
            if (!string.IsNullOrEmpty(lastName))
            {
                context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-LastName", lastName);
            }

            // X-User-Roles - all roles as comma-separated list
            var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            if (roles.Count > 0)
            {
                context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Roles", string.Join(",", roles));
            }

            // X-User-SbuId - SBU identifier
            var sbuId = user.FindFirst("SbuId")?.Value;
            if (!string.IsNullOrEmpty(sbuId))
            {
                context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-SbuId", sbuId);
            }

            // X-User-TeamId - Team identifier
            var teamId = user.FindFirst("TeamId")?.Value;
            if (!string.IsNullOrEmpty(teamId))
            {
                context.ProxyRequest.Headers.TryAddWithoutValidation("X-User-TeamId", teamId);
            }
        }

        // Remove the Authorization header so downstream services don't see the JWT
        // (they should trust X-User-* headers instead)
        context.ProxyRequest.Headers.Remove("Authorization");

        return ValueTask.CompletedTask;
    }
}
