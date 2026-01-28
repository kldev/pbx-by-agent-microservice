namespace DataSource.Api.Models;

/// <summary>
/// Request to get subordinates for a given supervisor.
/// Used by RCP for filtering supervisor views.
/// </summary>
public record SubordinatesRequest(long SupervisorId);
