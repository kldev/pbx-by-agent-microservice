using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using Microsoft.AspNetCore.Mvc;
using RateService.Api.Features.DestinationGroups.Model;

namespace RateService.Api.Features.DestinationGroups;

public static class Endpoint
{
    private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops];

    public static void MapDestinationGroupEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/destination-groups")
            .WithTags("DestinationGroups")
            .RequireAuthorization();

        // GET /api/destination-groups
        group.MapGet("/", GetAll)
            .WithName("GetDestinationGroups")
            .WithSummary("Lista grup destynacji")
            .Produces<IEnumerable<DestinationGroupResponse>>();

        // GET /api/destination-groups/{id}
        group.MapGet("/{id:int}", GetById)
            .WithName("GetDestinationGroupById")
            .WithSummary("Pobierz grupę destynacji")
            .Produces<DestinationGroupResponse>()
            .Produces<ApiErrorResponse>(404);

        // POST /api/destination-groups
        group.MapPost("/", Create)
            .WithName("CreateDestinationGroup")
            .WithSummary("Utwórz grupę destynacji")
            .Produces<DestinationGroupResponse>()
            .Produces<ApiErrorResponse>(400);

        // PUT /api/destination-groups/{id}
        group.MapPut("/{id:int}", Update)
            .WithName("UpdateDestinationGroup")
            .WithSummary("Aktualizuj grupę destynacji")
            .Produces<DestinationGroupResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetAll(
        ClaimsPrincipal user,
        IDestinationGroupService service)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetAllAsync(),
            AllRoles);
    }

    private static async Task<IResult> GetById(
        ClaimsPrincipal user,
        IDestinationGroupService service,
        int id)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByIdAsync(id),
            AllRoles);
    }

    private static async Task<IResult> Create(
        ClaimsPrincipal user,
        IDestinationGroupService service,
        [FromBody] CreateDestinationGroupRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.CreateAsync(req),
            AllRoles);
    }

    private static async Task<IResult> Update(
        ClaimsPrincipal user,
        IDestinationGroupService service,
        int id,
        [FromBody] CreateDestinationGroupRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.UpdateAsync(id, req),
            AllRoles);
    }
}
