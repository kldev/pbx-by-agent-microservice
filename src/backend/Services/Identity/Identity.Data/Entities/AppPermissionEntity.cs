using App.Bps.Enum;

namespace Identity.Data.Entities;

/// <summary>
/// Dictionary table for system permissions. ID is not auto-incremented - it maps to AppPermission enum values.
/// </summary>
public class AppPermissionEntity
{
    /// <summary>
    /// Permission ID - matches AppPermission enum value (no auto-increment)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Permission name/code
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Permission description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Permission category for grouping (e.g., Invoice, Order, Customer)
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Whether the permission is active in the system
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Whether this is a special permission that is not granted to Root role
    /// </summary>
    public bool IsSpecial { get; set; }

    /// <summary>
    /// Users who have this permission
    /// </summary>
    public ICollection<AppUserPermissionAssignment> UserAssignments { get; set; } = new List<AppUserPermissionAssignment>();

    /// <summary>
    /// Maps entity to AppPermission enum
    /// </summary>
    public AppPermission ToAppPermission() => (AppPermission)Id;

    /// <summary>
    /// Creates entity from AppPermission enum
    /// </summary>
    public static AppPermissionEntity FromAppPermission(AppPermission permission, string? description = null, string? category = null, bool isSpecial = false) => new()
    {
        Id = (int)permission,
        Name = permission.ToString(),
        Description = description,
        Category = category,
        IsActive = true,
        IsSpecial = isSpecial
    };
}
