namespace DataSource.Data.Entities;

/// <summary>
/// View entity for user subordinates - used by RCP for supervisor filtering.
/// </summary>
public class VwUserSubordinate : ViewEntityBase
{
    /// <summary>
    /// ID of the user's direct supervisor (null = no supervisor, e.g., CEO)
    /// </summary>
    public long? SupervisorId { get; set; }
}
