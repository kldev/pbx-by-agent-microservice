namespace Identity.Data.Entities;

/// <summary>
/// Join table for many-to-many relationship between AppUser and AppPermissionEntity.
/// A user can have multiple direct permissions (in addition to role-based permissions).
/// </summary>
public class AppUserPermissionAssignment
{
    /// <summary>
    /// User ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Permission ID
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// When the permission was granted to the user
    /// </summary>
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Who granted the permission (user ID or system)
    /// </summary>
    public long? GrantedBy { get; set; }

    /// <summary>
    /// Navigation to user
    /// </summary>
    public AppUser User { get; set; } = null!;

    /// <summary>
    /// Navigation to permission
    /// </summary>
    public AppPermissionEntity Permission { get; set; } = null!;
}
