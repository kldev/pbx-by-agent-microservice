namespace Identity.Data.Entities;

/// <summary>
/// Join table for many-to-many relationship between AppUser and AppUserRole.
/// A user can have multiple roles.
/// </summary>
public class AppUserRoleAssignment
{
    /// <summary>
    /// User ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Role ID
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// When the role was assigned to the user
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Who assigned the role (user ID or system)
    /// </summary>
    public long? AssignedBy { get; set; }

    /// <summary>
    /// Navigation to user
    /// </summary>
    public AppUser User { get; set; } = null!;

    /// <summary>
    /// Navigation to role
    /// </summary>
    public AppUserRole Role { get; set; } = null!;
}
