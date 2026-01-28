using App.BaseData;
using App.Bps.Enum;

namespace Identity.Data.Entities;

public class AppUser : BaseAuditableTable
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Department? Department { get; set; }
    public int StructureId { get; set; }
    public long? TeamId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Bezpośredni przełożony użytkownika (null = brak przełożonego, np. CEO)
    /// </summary>
    public long? SupervisorId { get; set; }

    // Navigation
    public StructureDict Structure { get; set; } = null!;
    public Team? Team { get; set; }
    public AppUser? Supervisor { get; set; }
    public ICollection<AppUser> Subordinates { get; set; } = new List<AppUser>();
    public ICollection<LoginAuditLog> LoginAuditLogs { get; set; } = new List<LoginAuditLog>();
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    // Role and Permission assignments (many-to-many)
    public ICollection<AppUserRoleAssignment> RoleAssignments { get; set; } = new List<AppUserRoleAssignment>();
    public ICollection<AppUserPermissionAssignment> PermissionAssignments { get; set; } = new List<AppUserPermissionAssignment>();
}
