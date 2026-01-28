using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using Microsoft.AspNetCore.Mvc;
using RateService.Api.Features.Tariffs.Model;

namespace RateService.Api.Features.Tariffs;

public static class Endpoint
{
    private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops];

    public static void MapTariffEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tariffs")
            .WithTags("Tariffs")
            .RequireAuthorization();

        // POST /api/rates/tariffs/list
        group.MapPost("/list", GetList)
            .WithName("GetTariffsList")
            .WithSummary("Lista taryf")
            .Produces<PagedResult<TariffResponse>>()
            .Produces<ApiErrorResponse>(400);

        // GET /api/rates/tariffs/{gid}
        group.MapGet("/{gid}", GetByGid)
            .WithName("GetTariffByGid")
            .WithSummary("Pobierz taryfę")
            .Produces<TariffDetailResponse>()
            .Produces<ApiErrorResponse>(404);

        // POST /api/rates/tariffs
        group.MapPost("/", Create)
            .WithName("CreateTariff")
            .WithSummary("Utwórz taryfę")
            .Produces<TariffResponse>()
            .Produces<ApiErrorResponse>(400);

        // PUT /api/rates/tariffs/{gid}
        group.MapPut("/{gid}", Update)
            .WithName("UpdateTariff")
            .WithSummary("Aktualizuj taryfę")
            .Produces<TariffResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        // DELETE /api/rates/tariffs/{gid}
        group.MapDelete("/{gid}", Delete)
            .WithName("DeleteTariff")
            .WithSummary("Usuń taryfę")
            .Produces<bool>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        ITariffService service,
        [FromBody] TariffListFilter filter)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            filter,
            async (auth, req) => await service.GetListAsync(auth, req),
            AllRoles);
    }

    private static async Task<IResult> GetByGid(
        ClaimsPrincipal user,
        ITariffService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByGidAsync(auth, gid),
            AllRoles);
    }

    private static async Task<IResult> Create(
        ClaimsPrincipal user,
        ITariffService service,
        [FromBody] CreateTariffRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.CreateAsync(auth, req),
            AllRoles);
    }

    private static async Task<IResult> Update(
        ClaimsPrincipal user,
        ITariffService service,
        string gid,
        [FromBody] UpdateTariffRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.UpdateAsync(auth, gid, req),
            AllRoles);
    }

    private static async Task<IResult> Delete(
        ClaimsPrincipal user,
        ITariffService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.DeleteAsync(auth, gid),
            AllRoles);
    }
}
