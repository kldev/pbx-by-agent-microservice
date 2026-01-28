using App.Shared.Web.BaseModel;
using CdrService.Api.Features.CallRecords;
using CdrService.Api.Features.CallStatuses;
using CdrService.Api.Features.TerminationCauses;
using CdrService.Api.Features.CallDirections;

namespace CdrService.Api.Infrastructure;

public static class EndpointExtensions
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapCallRecordEndpoints();
        app.MapCallStatusEndpoints();
        app.MapTerminationCauseEndpoints();
        app.MapCallDirectionEndpoints();

        // Health check
        app.MapGet("/health", () => new HealthStatusResponse() { Status = "healthy", Service = "CdrService" })
            .ExcludeFromDescription()
            .WithTags("Health")
            .AllowAnonymous();
    }
}
