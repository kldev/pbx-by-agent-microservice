using App.Bps.Enum;

namespace Identity.Data.Entities;

/// <summary>
/// Dictionary table for system roles. ID is not auto-incremented - it maps to AppRole enum values.
/// </summary>
public class AppUserRole
{
    /// <summary>
    /// Role ID - matches AppRole enum value (no auto-increment)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Role name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Role description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the role is active in the system
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Users assigned to this role
    /// </summary>
    public ICollection<AppUserRoleAssignment> UserAssignments { get; set; } = new List<AppUserRoleAssignment>();

    /// <summary>
    /// Maps entity to AppRole enum
    /// </summary>
    public AppRole ToAppRole() => (AppRole)Id;

    /// <summary>
    /// Creates entity from AppRole enum
    /// </summary>
    public static AppUserRole FromAppRole(AppRole role, string? description = null) => new()
    {
        Id = (int)role,
        Name = role.ToString(),
        Description = description,
        IsActive = true
    };
}
