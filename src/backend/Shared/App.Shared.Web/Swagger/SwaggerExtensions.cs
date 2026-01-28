using Microsoft.Extensions.DependencyInjection;

namespace App.Shared.Web.Swagger;

/// <summary>
/// Extension methods for configuring Swagger with proper enum handling.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger generation with enum values serialized as strings.
    /// </summary>
    public static IServiceCollection AddSwaggerWithEnumStrings(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<EnumSchemaFilter>();
        });

        return services;
    }
}
