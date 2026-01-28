using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using CdrService.Api.Features.CallDirections.Model;

namespace CdrService.Api.Features.CallDirections;

public static class Endpoint
{
    private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops];

    public static void MapCallDirectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/call-directions")
            .WithTags("CallDirections")
            .RequireAuthorization();

        // POST /api/call-directions/list
        group.MapPost("/list", GetList)
            .WithName("GetCallDirectionsList")
            .WithSummary("Lista kierunków połączeń")
            .Produces<IEnumerable<CallDirectionResponse>>()
            .Produces<ApiErrorResponse>(400);

        // GET /api/call-directions/{gid}
        group.MapGet("/{gid}", GetByGid)
            .WithName("GetCallDirectionByGid")
            .WithSummary("Pobierz kierunek połączenia")
            .Produces<CallDirectionResponse>()
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        ICallDirectionService service)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetListAsync(auth),
            AllRoles);
    }

    private static async Task<IResult> GetByGid(
        ClaimsPrincipal user,
        ICallDirectionService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByGidAsync(auth, gid),
            AllRoles);
    }
}
