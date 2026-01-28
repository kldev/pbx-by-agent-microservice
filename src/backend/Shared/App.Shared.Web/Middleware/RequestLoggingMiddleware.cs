using App.Shared.Web.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace App.Shared.Web.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;
        var userGid = context.GetUserGid() ?? "anonymous";
        var userEmail = context.GetUserEmail() ?? "-";

        if (path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        _logger.LogInformation("User [{UserGid}] ({Email}) -> {Method} {Path}",
            userGid, userEmail, method, path);

        await _next(context);
    }
}
