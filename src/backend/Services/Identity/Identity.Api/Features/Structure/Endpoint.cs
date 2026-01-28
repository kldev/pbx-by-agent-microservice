using System.Security.Claims;
using App.Bps.Enum;
using App.Shared.Web.ApiResultPattern;
using App.Shared.Web.BaseModel;
using Identity.Api.Features.Structure.Model;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Features.Structure;

public static class Endpoint
{
    private static readonly AppRole[] AdminRoles = [AppRole.Admin, AppRole.Root];
    private static readonly AppRole[] ReadRoles = [AppRole.Admin, AppRole.Root, AppRole.Ops];

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/structure")
            .WithTags("Structure")
            .RequireAuthorization();

        // Read endpoints - available to more roles
        group.MapPost("/list", GetList)
            .WithName("GetStructureList")
            .Produces<PagedResult<StructureResponse>>()
            .Produces<ApiErrorResponse>(400);

        group.MapPost("/all", GetAll)
            .WithName("GetAllStructure")
            .Produces<List<StructureResponse>>();

        group.MapPost("/by-id", GetById)
            .WithName("GetStructure")
            .Produces<StructureResponse>()
            .Produces<ApiErrorResponse>(404);

        // Admin only endpoints
        group.MapPost("/", Create)
            .WithName("CreateStructure")
            .Produces<StructureResponse>(201)
            .Produces<ApiErrorResponse>(400);

        group.MapPut("/{id:int}", Update)
            .WithName("UpdateStructure")
            .Produces<StructureResponse>()
            .Produces<ApiErrorResponse>(400)
            .Produces<ApiErrorResponse>(404);
    }

    private static async Task<IResult> GetList(
        ClaimsPrincipal user,
        IStructureService service,
        [FromBody] StructureListFilter filter)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            filter,
            async (auth, req) => await service.GetListAsync(auth, req),
            ReadRoles);
    }

    private static async Task<IResult> GetAll(
        ClaimsPrincipal user,
        IStructureService service,
        [FromBody] GetAllStructureRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetAllAsync(auth, request.IsActive),
            ReadRoles);
    }

    private static async Task<IResult> GetById(
        ClaimsPrincipal user,
        IStructureService service,
        [FromBody] GetStructureByIdRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            async auth => await service.GetByIdAsync(auth, request.Id),
            ReadRoles);
    }

    private static async Task<IResult> Create(
        ClaimsPrincipal user,
        IStructureService service,
        [FromBody] CreateStructureRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.CreateAsync(auth, req),
            AdminRoles);
    }

    private static async Task<IResult> Update(
        ClaimsPrincipal user,
        IStructureService service,
        int id,
        [FromBody] UpdateStructureRequest request)
    {
        return await EndpointHelpers.HandleAuthRequestAsync(
            user,
            request,
            async (auth, req) => await service.UpdateAsync(auth, id, req),
            AdminRoles);
    }
}
