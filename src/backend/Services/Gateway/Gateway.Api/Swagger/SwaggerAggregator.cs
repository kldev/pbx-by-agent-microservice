using System.Text.Json;
using System.Text.Json.Nodes;

namespace Gateway.Api.Swagger;

public class SwaggerAggregator
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SwaggerAggregator> _logger;

    public SwaggerAggregator(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<SwaggerAggregator> logger)
    {
        _httpClient = httpClientFactory.CreateClient("SwaggerClient");
        _configuration = configuration;
        _logger = logger;
    }

    public record ServiceSwaggerInfo(string Name, string Prefix, string ClusterKey);

    private static readonly ServiceSwaggerInfo[] Services =
    [
        new("Identity", "/api/identity", "identity-cluster"),
        new("Jobs", "/api/jobs", "jobs-cluster"),
        new("Sales", "/api/sales", "sales-cluster"),
        new("DataSource", "/api/datasource", "datasource-cluster"),
        new("Rcp", "/api/rcp", "rcp-cluster"),
        new("Rate", "/api/rate", "rate-cluster"),
        new("Projects", "/api/projects", "projects-cluster"),
        new("Cdr", "/api/cdr", "cdr-cluster"),
        new("AnswerRule", "/api/answerrule", "answerrule-cluster"),
        new("Fincosts", "/api/fincosts", "fincosts-cluster")
    ];

    /// <summary>
    /// Gets swagger spec for a single service with paths prefixed for Gateway.
    /// </summary>
    public async Task<JsonObject?> GetServiceSwaggerAsync(string serviceName)
    {
        var service = Services.FirstOrDefault(s =>
            s.Name.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

        if (service == null) return null;

        var baseUrl = GetClusterAddress(service.ClusterKey);
        if (string.IsNullOrEmpty(baseUrl)) return null;

        try
        {
            var swaggerUrl = $"{baseUrl}/swagger/v1/swagger.json";
            var response = await _httpClient.GetStringAsync(swaggerUrl);
            var doc = JsonNode.Parse(response)?.AsObject();

            if (doc == null) return null;

            // Modify paths to include service prefix
            if (doc["paths"] is JsonObject paths)
            {
                var newPaths = new JsonObject();
                foreach (var (path, value) in paths)
                {
                    // /api/sbu/list -> /api/identity/sbu/list
                    var newPath = path.StartsWith("/api/")
                        ? service.Prefix + path[4..] // Remove "/api" and add prefix
                        : service.Prefix + path;

                    newPaths[newPath] = value?.DeepClone();
                }
                doc["paths"] = newPaths;
            }

            // Update info
            if (doc["info"] is JsonObject info)
            {
                info["title"] = $"PBX {service.Name} API (via Gateway)";
            }

            return doc;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch swagger from {Service}", serviceName);
            return null;
        }
    }

    /// <summary>
    /// Gets aggregated swagger spec combining all services.
    /// </summary>
    public async Task<JsonObject> GetAggregatedSwaggerAsync()
    {
        var aggregated = new JsonObject
        {
            ["openapi"] = "3.0.1",
            ["info"] = new JsonObject
            {
                ["title"] = "PBX Gateway API",
                ["description"] = "Aggregated API documentation for all JD microservices",
                ["version"] = "v1"
            },
            ["paths"] = new JsonObject(),
            ["components"] = new JsonObject
            {
                ["schemas"] = new JsonObject()
            }
        };

        var allPaths = aggregated["paths"]!.AsObject();
        var allSchemas = aggregated["components"]!["schemas"]!.AsObject();

        foreach (var service in Services)
        {
            var doc = await GetServiceSwaggerAsync(service.Name);
            if (doc == null) continue;

            // Merge paths
            if (doc["paths"] is JsonObject paths)
            {
                foreach (var (path, value) in paths)
                {
                    allPaths[path] = value?.DeepClone();
                }
            }

            // Merge schemas with service prefix to avoid conflicts
            if (doc["components"]?["schemas"] is JsonObject schemas)
            {
                foreach (var (schemaName, value) in schemas)
                {
                    var prefixedName = $"{service.Name}_{schemaName}";
                    allSchemas[prefixedName] = value?.DeepClone();
                }
            }
        }

        return aggregated;
    }

    private string? GetClusterAddress(string clusterKey)
    {
        return _configuration[$"ReverseProxy:Clusters:{clusterKey}:Destinations:{clusterKey.Replace("-cluster", "-destination")}:Address"];
    }
}
