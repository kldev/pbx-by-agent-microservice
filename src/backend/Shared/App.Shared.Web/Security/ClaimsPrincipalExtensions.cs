using System.Security.Claims;
using App.Bps.Enum;

namespace App.Shared.Web.Security;


public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("UserId") ?? user.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null && long.TryParse(claim.Value, out var id) ? id : 0;
    }

    public static string GetUserGid(this ClaimsPrincipal user)
    {
        return user.FindFirst("Gid")?.Value ?? string.Empty;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }

    public static string GetFirstName(this ClaimsPrincipal user)
    {
        return user.FindFirst("FirstName")?.Value ?? string.Empty;
    }

    public static string GetLastName(this ClaimsPrincipal user)
    {
        return user.FindFirst("LastName")?.Value ?? string.Empty;
    }

    public static List<AppRole> GetRoles(this ClaimsPrincipal user)
    {
        return user.FindAll(ClaimTypes.Role)
            .Select(c => Enum.TryParse<AppRole>(c.Value, out var role) ? (AppRole?)role : null)
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .ToList();
    }

    public static int? GetSbuId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("SbuId");
        return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
    }

    public static long? GetTeamId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("TeamId");
        return claim != null && long.TryParse(claim.Value, out var id) ? id : null;
    }

    public static PortalAuthInfo? GetAuthInfo(this ClaimsPrincipal user)
    {
        var userId = user.GetUserId();
        if (userId == 0) return null;

        return new PortalAuthInfo
        {
            UserId = userId,
            Gid = user.GetUserGid(),
            FirstName = user.GetFirstName(),
            LastName = user.GetLastName(),
            Email = user.GetEmail(),
            Roles = user.GetRoles(),
            SbuId = user.GetSbuId(),
            TeamId = user.GetTeamId()
        };
    }

    public static bool IsAuthorized(this ClaimsPrincipal user, params AppRole[] allowedRoles)
    {
        var userRoles = user.GetRoles();
        if (userRoles.Contains(AppRole.Root)) return true;
        return allowedRoles.Any(r => userRoles.Contains(r));
    }

    public static bool IsUnauthorized(this ClaimsPrincipal user, params AppRole[] allowedRoles)
    {
        return !user.IsAuthorized(allowedRoles);
    }
}