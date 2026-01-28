using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using CdrService.Api.Features.TerminationCauses.Model;

namespace CdrService.Api.Features.TerminationCauses;

public static class Endpoint
{
    private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops];

    public static void MapTerminationCauseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/termination-causes")
            .WithTags("TerminationCauses")
            .RequireAuthorization();

        // POST /api/termination-causes/list
        group.MapPost("/list", GetList)
            .WithName("GetTerminationCausesList")
            .WithSummary("Lista przyczyn zakończenia")
            .Produces<IEnumerable<TerminationCauseResponse>>()
            .Produces<ApiErrorResponse>(400);

        // GET /api/termination-causes/{gid}
        group.MapGet("/{gid}", GetByGid)
            .WithName("GetTerminationCauseByGid")
            .WithSummary("Pobierz przyczynę zakończenia")
            .Produces<TerminationCauseResponse>()
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        ITerminationCauseService service)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetListAsync(auth),
            AllRoles);
    }

    private static async Task<IResult> GetByGid(
        ClaimsPrincipal user,
        ITerminationCauseService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByGidAsync(auth, gid),
            AllRoles);
    }
}
