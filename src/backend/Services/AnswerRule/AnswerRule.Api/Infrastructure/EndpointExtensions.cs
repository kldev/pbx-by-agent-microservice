using AnswerRule.Api.Features.Rules;

namespace AnswerRule.Api.Infrastructure;

public static class EndpointExtensions
{
    public static void MapAppEndpoints(this WebApplication app)
    {
        app.MapRuleEndpoints();

        // Health check
        app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).ExcludeFromDescription();
    }
}
