using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using Microsoft.AspNetCore.Mvc;
using CdrService.Api.Features.CallRecords.Model;

namespace CdrService.Api.Features.CallRecords;

public static class Endpoint
{
    private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops];

    public static void MapCallRecordEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/call-records")
            .WithTags("CallRecords")
            .RequireAuthorization();

        // POST /api/call-records/list
        group.MapPost("/list", GetList)
            .WithName("GetCallRecordsList")
            .WithSummary("Lista rekordów CDR")
            .Produces<PagedResult<CallRecordResponse>>()
            .Produces<ApiErrorResponse>(400);

        // GET /api/call-records/{gid}
        group.MapGet("/{gid}", GetByGid)
            .WithName("GetCallRecordByGid")
            .WithSummary("Pobierz rekord CDR")
            .Produces<CallRecordDetailResponse>()
            .Produces<ApiErrorResponse>(404);

        // POST /api/call-records
        group.MapPost("/", Create)
            .WithName("CreateCallRecord")
            .WithSummary("Utwórz rekord CDR")
            .Produces<CallRecordResponse>()
            .Produces<ApiErrorResponse>(400);

        // PUT /api/call-records/{gid}
        group.MapPut("/{gid}", Update)
            .WithName("UpdateCallRecord")
            .WithSummary("Aktualizuj rekord CDR")
            .Produces<CallRecordResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        // DELETE /api/call-records/{gid}
        group.MapDelete("/{gid}", Delete)
            .WithName("DeleteCallRecord")
            .WithSummary("Usuń rekord CDR")
            .Produces<bool>()
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        ICallRecordService service,
        [FromBody] CallRecordListFilter filter)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            filter,
            async (auth, req) => await service.GetListAsync(auth, req),
            AllRoles);
    }

    private static async Task<IResult> GetByGid(
        ClaimsPrincipal user,
        ICallRecordService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByGidAsync(auth, gid),
            AllRoles);
    }

    private static async Task<IResult> Create(
        ClaimsPrincipal user,
        ICallRecordService service,
        [FromBody] CreateCallRecordRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.CreateAsync(auth, req),
            AllRoles);
    }

    private static async Task<IResult> Update(
        ClaimsPrincipal user,
        ICallRecordService service,
        string gid,
        [FromBody] UpdateCallRecordRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.UpdateAsync(auth, gid, req),
            AllRoles);
    }

    private static async Task<IResult> Delete(
        ClaimsPrincipal user,
        ICallRecordService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.DeleteAsync(auth, gid),
            AllRoles);
    }
}
