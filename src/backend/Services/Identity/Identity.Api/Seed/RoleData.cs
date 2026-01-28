using App.Bps.Enum;
using Identity.Data.Entities;

namespace Identity.Api.Seed;

/// <summary>
/// Seed data for system roles. Roles are synchronized with AppRole enum on every startup.
/// </summary>
public static class RoleData
{
    public static List<AppUserRole> GetRoles()
    {
        return new List<AppUserRole>
        {
            AppUserRole.FromAppRole(AppRole.Root, "Super administrator with full system access"),
            AppUserRole.FromAppRole(AppRole.Ops, "Operations team - manage system configuration"),
            AppUserRole.FromAppRole(AppRole.Admin, "System administrator with user management"),
            AppUserRole.FromAppRole(AppRole.Support, "Support team - handle customer issues"),
            AppUserRole.FromAppRole(AppRole.User, "Standard user role")
        };
    }
}
