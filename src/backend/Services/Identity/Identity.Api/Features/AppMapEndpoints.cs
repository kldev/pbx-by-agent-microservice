namespace Identity.Api.Features;

public static class AppMapEndpoints
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        Auth.Endpoint.Map(app);
        Structure.Endpoint.Map(app);
        Teams.Endpoint.Map(app);
        AppUsers.Endpoint.Map(app);
    }
}
