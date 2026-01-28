using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using CdrService.Api.Features.CallStatuses.Model;

namespace CdrService.Api.Features.CallStatuses;

public static class Endpoint
{
    private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops];

    public static void MapCallStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/call-statuses")
            .WithTags("CallStatuses")
            .RequireAuthorization();

        // POST /api/call-statuses/list
        group.MapPost("/list", GetList)
            .WithName("GetCallStatusesList")
            .WithSummary("Lista statusów połączeń")
            .Produces<IEnumerable<CallStatusResponse>>()
            .Produces<ApiErrorResponse>(400);

        // GET /api/call-statuses/{gid}
        group.MapGet("/{gid}", GetByGid)
            .WithName("GetCallStatusByGid")
            .WithSummary("Pobierz status połączenia")
            .Produces<CallStatusResponse>()
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        ICallStatusService service)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetListAsync(auth),
            AllRoles);
    }

    private static async Task<IResult> GetByGid(
        ClaimsPrincipal user,
        ICallStatusService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByGidAsync(auth, gid),
            AllRoles);
    }
}
