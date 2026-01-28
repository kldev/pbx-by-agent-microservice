using App.Bps.Enum;
using Identity.Data.Entities;

namespace Identity.Api.Seed;

/// <summary>
/// Seed data for system permissions. Permissions are synchronized with AppPermission enum on every startup.
/// </summary>
public static class PermissionData
{
    public static List<AppPermissionEntity> GetPermissions()
    {
        return new List<AppPermissionEntity>
        {
            AppPermissionEntity.FromAppPermission(
                AppPermission.InvoicesBank,
                "Access to invoices and bank operations",
                "Finance",
                isSpecial: true),

            AppPermissionEntity.FromAppPermission(
                AppPermission.SensitiveData,
                "Access to sensitive data",
                "System",
                isSpecial: true)
        };
    }
}
