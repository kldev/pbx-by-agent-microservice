namespace FinCosts.Api.Features;
public static class EndpointExtensions
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        Costs.Endpoint.Map(app);
    }
}