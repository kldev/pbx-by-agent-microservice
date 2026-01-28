using System.IO;
using System.Text.Json;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Identity.Api.Features.Auth.Model;

namespace Identity.Api.Features.Auth;

public static class Endpoint
{
    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        // Internal endpoint - called only by Gateway, hidden from swagger
        group.MapPost("/validate-login", ValidateLogin)
            .WithName("ValidateLogin")
            .Produces<ValidateLoginResponse>()
            .Produces<ApiErrorResponse>(400)
            .AllowAnonymous()
            .ExcludeFromDescription();

        group.MapPost("/me", GetMe)
            .WithName("GetMe")
            .Produces<MeResponse>()
            .Produces<ApiErrorResponse>(401);

        group.MapPost("/jd-prefill", GetJdPrefill)
            .WithName("GetJdPrefill")
            .Produces<JdPrefillResponse>()
            .Produces<ApiErrorResponse>(401);
    }

    private static async Task<IResult> ValidateLogin(
        HttpContext httpContext,
        IAuthService service)
    {
        using var reader = new StreamReader(httpContext.Request.Body);
        var body = await reader.ReadToEndAsync();

        LoginRequest? request;
        try
        {
            request = JsonSerializer.Deserialize<LoginRequest>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch
        {
            return Results.BadRequest(new ApiErrorResponse { Code = "INVALID_JSON", Message = "Invalid request body" });
        }

        if (request == null || string.IsNullOrEmpty(request.Email))
        {
            return Results.BadRequest(new ApiErrorResponse { Code = "MISSING_EMAIL", Message = "Email is required" });
        }

        var result = await service.ValidateLoginAsync(request);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetMe(
        HttpContext httpContext,
        IAuthService service)
    {
        // Get user info from X-* headers set by Gateway
        var userGid = httpContext.GetUserGid();
        if (string.IsNullOrEmpty(userGid))
        {
            return Results.Unauthorized();
        }

        var result = await service.GetMeAsync(userGid);
        return ResultToApiMapper.ToApiResult(result);
    }

    private static async Task<IResult> GetJdPrefill(
        HttpContext httpContext,
        IAuthService service)
    {
        var userGid = httpContext.GetUserGid();
        if (string.IsNullOrEmpty(userGid))
        {
            return Results.Unauthorized();
        }

        var result = await service.GetJdPrefillAsync(userGid);
        return ResultToApiMapper.ToApiResult(result);
    }
}
