using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Identity.Api.Features.AppUsers.Model;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Features.AppUsers;

public static class Endpoint
{
    // Only Root can manage users (create, list, update, delete)
    private static readonly AppRole[] RootOnly = [AppRole.Root];

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/app-users")
            .WithTags("AppUsers")
            .RequireAuthorization();

        // Root only - list users
        group.MapPost("/list", GetList)
            .WithName("GetAppUserList")
            .Produces<PagedResult<AppUserResponse>>()
            .Produces<ApiErrorResponse>(400);

        // Any logged-in user can get profile by GID
        group.MapPost("/by-gid", GetByGid)
            .WithName("GetAppUser")
            .Produces<AppUserResponse>()
            .Produces<ApiErrorResponse>(404);

        // Root only - create user
        group.MapPost("/", Create)
            .WithName("CreateAppUser")
            .Produces<AppUserResponse>(201)
            .Produces<ApiErrorResponse>(400);

        // Root only - update user
        group.MapPut("/{gid}", Update)
            .WithName("UpdateAppUser")
            .Produces<AppUserResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        // Root OR self - change password
        group.MapPut("/{gid}/password", ChangePassword)
            .WithName("ChangeAppUserPassword")
            .Produces<bool>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        // Root only - delete user
        group.MapDelete("/{gid}", Delete)
            .WithName("DeleteAppUser")
            .Produces<bool>()
            .Produces<ApiErrorResponse>(404);

        // Root only - set supervisor
        group.MapPut("/{gid}/supervisor", SetSupervisor)
            .WithName("SetAppUserSupervisor")
            .Produces<AppUserResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        IAppUserService service,
        [FromBody] AppUserListFilter filter)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            filter,
            async (auth, req) => await service.GetListAsync(auth, req),
            RootOnly);
    }

    /// <summary>
    /// Any logged-in user can get a user profile by GID.
    /// </summary>
    private static async Task<IResult> GetByGid(
        ClaimsPrincipal user,
        IAppUserService service,
        [FromBody] GetByGidRequest request)
    {
        var authInfo = user.GetAuthInfo();
        if (authInfo == null)
            return Results.Unauthorized();

        var result = await service.GetByGidAsync(authInfo, request.Gid);
        return result.ToApiResult();
    }

    private static async Task<IResult> Create(
        ClaimsPrincipal user,
        IAppUserService service,
        [FromBody] CreateAppUserRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.CreateAsync(auth, req),
            RootOnly);
    }

    private static async Task<IResult> Update(
        ClaimsPrincipal user,
        IAppUserService service,
        string gid,
        [FromBody] UpdateAppUserRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.UpdateAsync(auth, gid, req),
            RootOnly);
    }

    /// <summary>
    /// Change password: Root can change any user's password, or user can change their own.
    /// </summary>
    private static async Task<IResult> ChangePassword(
        ClaimsPrincipal user,
        IAppUserService service,
        string gid,
        [FromBody] ChangePasswordRequest request)
    {
        var authInfo = user.GetAuthInfo();
        if (authInfo == null)
            return Results.Unauthorized();

        // Root can change any password, or user can change their own
        bool canChange = authInfo.IsRoot || authInfo.Gid == gid;
        if (!canChange)
            return Results.Forbid();

        var result = await service.ChangePasswordAsync(authInfo, gid, request);
        return result.ToApiResult();
    }

    private static async Task<IResult> Delete(
        ClaimsPrincipal user,
        IAppUserService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.DeleteAsync(auth, gid),
            RootOnly);
    }

    private static async Task<IResult> SetSupervisor(
        ClaimsPrincipal user,
        IAppUserService service,
        string gid,
        [FromBody] SetSupervisorRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.SetSupervisorAsync(auth, gid, req),
            RootOnly);
    }
}
