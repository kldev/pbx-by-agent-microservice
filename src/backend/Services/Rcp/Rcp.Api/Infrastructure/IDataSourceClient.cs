namespace Rcp.Api.Infrastructure;

/// <summary>
/// Client for communication with DataSource service.
/// Used to get subordinates for supervisor filtering.
/// </summary>
public interface IDataSourceClient
{
    /// <summary>
    /// Get list of subordinate user IDs for a given supervisor.
    /// </summary>
    Task<List<long>> GetSubordinateIdsAsync(long supervisorId);
}
