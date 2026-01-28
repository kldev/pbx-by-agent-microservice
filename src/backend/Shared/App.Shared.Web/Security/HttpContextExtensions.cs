using Microsoft.AspNetCore.Http;

namespace App.Shared.Web.Security;

public static class HttpContextExtensions
{
    public const string UserIdHeader = "X-User-Id";
    public const string UserGidHeader = "X-User-Gid";
    public const string UserEmailHeader = "X-User-Email";
    public const string UserFirstNameHeader = "X-User-FirstName";
    public const string UserLastNameHeader = "X-User-LastName";
    public const string UserRolesHeader = "X-User-Roles";
    public const string UserSbuIdHeader = "X-User-SbuId";

    public static string? GetUserGid(this HttpContext context)
    {
        return context.Request.Headers.TryGetValue(UserGidHeader, out var value)
            ? value.ToString()
            : null;
    }

    public static string? GetUserEmail(this HttpContext context)
    {
        return context.Request.Headers.TryGetValue(UserEmailHeader, out var value)
            ? value.ToString()
            : null;
    }

    public static string? GetUserFirstName(this HttpContext context)
    {
        return context.Request.Headers.TryGetValue(UserFirstNameHeader, out var value)
            ? value.ToString()
            : null;
    }

    public static string? GetUserLastName(this HttpContext context)
    {
        return context.Request.Headers.TryGetValue(UserLastNameHeader, out var value)
            ? value.ToString()
            : null;
    }

    public static List<string> GetUserRoles(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(UserRolesHeader, out var value) && !string.IsNullOrEmpty(value))
        {
            return value.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .ToList();
        }
        return [];
    }

    public static int? GetUserSbuId(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(UserSbuIdHeader, out var value)
            && int.TryParse(value, out var sbuId))
        {
            return sbuId;
        }
        return null;
    }

    public static long? GetUserId(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(UserIdHeader, out var value)
            && long.TryParse(value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
