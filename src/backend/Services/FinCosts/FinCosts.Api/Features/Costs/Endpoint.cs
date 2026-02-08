using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using FinCosts.Api.Features.Costs.Model;
using Microsoft.AspNetCore.Mvc;

namespace FinCosts.Api.Features.Costs;

public static class Endpoint
{
    private static readonly AppRole[] ReadRoles = [AppRole.Admin, AppRole.Root, AppRole.Support];

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/costs").WithTags("Costs").RequireAuthorization();

        group.MapPost("/document-types", GetDocumentTypes).WithName("GetDocumentTypes")
            .Produces<List<DocumentTypeResponse>>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        group.MapPost("/currency-types", GetCurrencyTypes).WithName("GetCurrencyTypes")
            .Produces<List<CurrencyTypeResponse>>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        group.MapPost("/vat-rate-types", GetVatRateTypes).WithName("GetVatRateTypes")
            .Produces<List<VatRateTypeResponse>>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);

        group.MapPost("/documents/list", GetDocumentEntries).WithName("GetDocumentEntries")
            .Produces<PagedResult<DocumentEntryResponse>>()
            .Produces<ApiErrorResponse>(400);
    }

    private static async Task<IResult> GetDocumentTypes(
        ClaimsPrincipal user,
        ICostsService service)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user, new{},
            async (auth, req) => await service.GetDocumentTypes(auth),
            ReadRoles);
    }

    private static async Task<IResult> GetCurrencyTypes(
        ClaimsPrincipal user,
        ICostsService service)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user, new{},
            async (auth, req) => await service.GetCurrencyTypes(auth),
            ReadRoles);
    }

    private static async Task<IResult> GetVatRateTypes(
        ClaimsPrincipal user,
        ICostsService service)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user, new{},
            async (auth, req) => await service.GetVatRateTypes(auth),
            ReadRoles);
    }

    private static async Task<IResult> GetDocumentEntries(
        ClaimsPrincipal user,
        ICostsService service,
        [FromBody] DocumentEntryListFilter filter)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            filter,
            async (auth, req) => await service.GetDocumentEntries(auth, req),
            ReadRoles);
    }
}