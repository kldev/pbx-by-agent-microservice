using System.Security.Claims;
using AnswerRule.Api.Features.Rules.Model;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using Microsoft.AspNetCore.Mvc;

namespace AnswerRule.Api.Features.Rules;

public static class Endpoint
{
    private static readonly AppRole[] AllRoles = [AppRole.Root, AppRole.Ops, AppRole.Admin, AppRole.User];
    private static readonly AppRole[] AdminRoles = [AppRole.Root, AppRole.Ops, AppRole.Admin];
    private static readonly AppRole[] InternalRoles = [AppRole.Root, AppRole.Ops];

    public static void MapRuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rules")
            .WithTags("Answering Rules")
            .RequireAuthorization();

        // POST /api/rules/list
        group.MapPost("/list", GetList)
            .WithName("GetRulesList")
            .WithSummary("Lista reguł odpowiadania")
            .Produces<PagedResult<AnsweringRuleResponse>>()
            .Produces<ApiErrorResponse>(400);

        // GET /api/rules/{gid}
        group.MapGet("/{gid}", GetByGid)
            .WithName("GetRuleByGid")
            .WithSummary("Pobierz regułę")
            .Produces<AnsweringRuleDetailResponse>()
            .Produces<ApiErrorResponse>(404);

        // POST /api/rules
        group.MapPost("/", Create)
            .WithName("CreateRule")
            .WithSummary("Utwórz regułę")
            .Produces<AnsweringRuleDetailResponse>()
            .Produces<ApiErrorResponse>(400);

        // PUT /api/rules/{gid}
        group.MapPut("/{gid}", Update)
            .WithName("UpdateRule")
            .WithSummary("Aktualizuj regułę")
            .Produces<AnsweringRuleDetailResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        // DELETE /api/rules/{gid}
        group.MapDelete("/{gid}", Delete)
            .WithName("DeleteRule")
            .WithSummary("Usuń regułę")
            .Produces<bool>()
            .Produces<ApiErrorResponse>(404);

        // PATCH /api/rules/{gid}/toggle
        group.MapPatch("/{gid}/toggle", Toggle)
            .WithName("ToggleRule")
            .WithSummary("Włącz/wyłącz regułę")
            .Produces<AnsweringRuleDetailResponse>()
            .Produces<ApiErrorResponse>(404);

        // POST /api/check - for routing engine
        app.MapPost("/api/check", CheckActiveRule)
            .WithTags("Answering Rules")
            .WithName("CheckActiveRule")
            .WithSummary("Sprawdź aktywną regułę dla konta SIP")
            .RequireAuthorization()
            .Produces<CheckRuleResponse>()
            .Produces<ApiErrorResponse>(400);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        IRuleService service,
        [FromBody] AnsweringRuleListFilter filter)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            filter,
            async (auth, req) => await service.GetListAsync(auth, req),
            AllRoles);
    }

    private static async Task<IResult> GetByGid(
        ClaimsPrincipal user,
        IRuleService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByGidAsync(auth, gid),
            AllRoles);
    }

    private static async Task<IResult> Create(
        ClaimsPrincipal user,
        IRuleService service,
        [FromBody] CreateAnsweringRuleRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.CreateAsync(auth, req),
            AdminRoles);
    }

    private static async Task<IResult> Update(
        ClaimsPrincipal user,
        IRuleService service,
        string gid,
        [FromBody] UpdateAnsweringRuleRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.UpdateAsync(auth, gid, req),
            AdminRoles);
    }

    private static async Task<IResult> Delete(
        ClaimsPrincipal user,
        IRuleService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.DeleteAsync(auth, gid),
            AdminRoles);
    }

    private static async Task<IResult> Toggle(
        ClaimsPrincipal user,
        IRuleService service,
        string gid)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.ToggleAsync(auth, gid),
            AdminRoles);
    }

    private static async Task<IResult> CheckActiveRule(
        ClaimsPrincipal user,
        IRuleService service,
        [FromBody] CheckRuleRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.CheckActiveRuleAsync(auth, req),
            InternalRoles);
    }
}
