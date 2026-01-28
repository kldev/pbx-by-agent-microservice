namespace Rcp.Api.Features;

public static class EndpointExtensions
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        // TimeEntry (RCP) endpoints
        TimeEntry.Endpoint.Map(app);
    }
}
