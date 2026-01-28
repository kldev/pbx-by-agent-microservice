namespace App.Bps.Enum;

/// <summary>
/// System roles defining user access levels and responsibilities.
/// Users can have multiple roles assigned.
/// </summary>
public enum AppRole
{
    /// <summary>
    /// Full system access - administrators
    /// </summary>
    Root = 1,

    /// <summary>
    /// Operations team - manage system configuration
    /// </summary>
    Ops = 2,

    /// <summary>
    /// Admin - can manage users and basic settings
    /// </summary>
    Admin = 3,

    /// <summary>
    /// Support team - handle customer issues
    /// </summary>
    Support = 4,

    /// <summary>
    /// Standard user role
    /// </summary>
    User = 5
}
