using Common.Toolkit.ResultPattern;
using DataSource.Api.Features.DataSource;
using DataSource.Api.Models;

namespace DataSource.Api.Endpoints;

public static class DataSourceEndpoints
{
    public static void MapDataSourceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("DataSource")
            .RequireAuthorization();

        group.MapPost("/suggestions", GetSuggestions)
            .WithName("GetSuggestions")
            .WithDescription("Get suggestions - 25 records sorted by domain logic")
            .Produces<PickerDataResponse>();

        group.MapPost("/search", Search)
            .WithName("SearchData")
            .WithDescription("Search by query phrase")
            .Produces<PickerDataResponse>();

        group.MapPost("/resolve", Resolve)
            .WithName("ResolveData")
            .WithDescription("Resolve GID/ID for Tag/Prefill component")
            .Produces<PickerDataResponse>();

        group.MapPost("/types", GetTypes)
            .WithName("GetDataSourceTypes")
            .WithDescription("Get available data source types")
            .Produces<List<DataSourceTypeInfo>>();

        group.MapPost("/subordinates", GetSubordinates)
            .WithName("GetSubordinates")
            .WithDescription("Get subordinates for a supervisor (used by RCP)")
            .Produces<PickerDataResponse>();
    }

    private static async Task<IResult> GetSuggestions(
        SuggestionsRequest request,
        IDataSourceService service)
    {
        var result = await service.GetSuggestionsAsync(request);
        return result.Match(
            value => Results.Ok(value),
            error => Results.BadRequest(new { error.Code, error.Message })
        );
    }

    private static async Task<IResult> Search(
        SearchRequest request,
        IDataSourceService service)
    {
        var result = await service.SearchAsync(request);
        return result.Match(
            value => Results.Ok(value),
            error => Results.BadRequest(new { error.Code, error.Message })
        );
    }

    private static async Task<IResult> Resolve(
        ResolveRequest request,
        IDataSourceService service)
    {
        var result = await service.ResolveAsync(request);
        return result.Match(
            value => Results.Ok(value),
            error => Results.BadRequest(new { error.Code, error.Message })
        );
    }

    private static IResult GetTypes(
        EmptyRequest request,
        IDataSourceService service)
    {
        var types = service.GetDataSourceTypes();
        return Results.Ok(types);
    }

    private static async Task<IResult> GetSubordinates(
        SubordinatesRequest request,
        IDataSourceService service)
    {
        var result = await service.GetSubordinatesAsync(request);
        return result.Match(
            value => Results.Ok(value),
            error => Results.BadRequest(new { error.Code, error.Message })
        );
    }
}
