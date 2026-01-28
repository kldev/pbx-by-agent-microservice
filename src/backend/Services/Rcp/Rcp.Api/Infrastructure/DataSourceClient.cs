using System.Net.Http.Json;

namespace Rcp.Api.Infrastructure;

/// <summary>
/// HTTP client for DataSource service.
/// </summary>
public class DataSourceClient : IDataSourceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DataSourceClient> _logger;

    public DataSourceClient(HttpClient httpClient, ILogger<DataSourceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<long>> GetSubordinateIdsAsync(long supervisorId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/subordinates",
                new SubordinatesRequest(supervisorId));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get subordinates for supervisor {SupervisorId}: {StatusCode}",
                    supervisorId, response.StatusCode);
                return [];
            }

            var result = await response.Content.ReadFromJsonAsync<SubordinatesResponse>();
            return result?.Items.Select(x => x.RecordId).ToList() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subordinates for supervisor {SupervisorId}", supervisorId);
            return [];
        }
    }

    // Request/Response models matching DataSource API
    private record SubordinatesRequest(long SupervisorId);

    private class SubordinatesResponse
    {
        public List<SubordinateItem> Items { get; set; } = [];
        public int TotalCount { get; set; }
    }

    private class SubordinateItem
    {
        public long RecordId { get; set; }
        public string Gid { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string? SubLabel { get; set; }
    }
}
