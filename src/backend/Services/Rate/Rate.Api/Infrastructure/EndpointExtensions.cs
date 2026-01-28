using App.Shared.Web.BaseModel;
using RateService.Api.Features.DestinationGroups;
using RateService.Api.Features.Rates;
using RateService.Api.Features.Tariffs;

namespace RateService.Api.Infrastructure;

public static class EndpointExtensions
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapTariffEndpoints();
        app.MapRateEndpoints();
        app.MapDestinationGroupEndpoints();

        // Health check
        app.MapGet("/health", () => new HealthStatusResponse(){ Status = "healthy", Service = "RateService" })
            .ExcludeFromDescription()
            .WithTags("Health")
            .AllowAnonymous();
    }
}
