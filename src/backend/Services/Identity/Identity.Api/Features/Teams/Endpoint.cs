using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using Identity.Api.Features.Teams.Model;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Features.Teams;

public static class Endpoint
{
    private static readonly AppRole[] AllowedRoles = [AppRole.Admin, AppRole.Root];

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/teams")
            .WithTags("Teams")
            .RequireAuthorization();

        group.MapPost("/list", GetList)
            .WithName("GetTeamList")
            .Produces<PagedResult<TeamResponse>>()
            .Produces<ApiErrorResponse>(400);

        group.MapPost("/by-gid", GetByGid)
            .WithName("GetTeam")
            .Produces<TeamResponse>()
            .Produces<ApiErrorResponse>(404);

        group.MapPost("/", Create)
            .WithName("CreateTeam")
            .Produces<TeamResponse>(201)
            .Produces<ApiErrorResponse>(400);

        group.MapPut("/{gid}", Update)
            .WithName("UpdateTeam")
            .Produces<TeamResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        group.MapDelete("/{gid}", Delete)
            .WithName("DeleteTeam")
            .Produces<bool>()
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        ITeamService service,
        [FromBody] TeamListFilter filter)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            filter,
            async (auth, req) => await service.GetListAsync(auth, req),
            AllowedRoles);
    }

    private static async Task<IResult> GetByGid(
        ClaimsPrincipal user,
        ITeamService service,
        [FromBody] GetByGidRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByGidAsync(auth, request.Gid),
            AllowedRoles);
    }

    private static async Task<IResult> Create(
        ClaimsPrincipal user,
        ITeamService service,
        [FromBody] CreateTeamRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.CreateAsync(auth, req),
            AllowedRoles);
    }

    private static async Task<IResult> Update(
        ClaimsPrincipal user,
        ITeamService service,
        string gid,
        [FromBody] UpdateTeamRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.UpdateAsync(auth, gid, req),
            AllowedRoles);
    }

    private static async Task<IResult> Delete(
        ClaimsPrincipal user,
        ITeamService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.DeleteAsync(auth, gid),
            AllowedRoles);
    }
}
