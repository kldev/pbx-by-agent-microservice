using App.Bps.Enum;

namespace App.Shared.Web.Security;

public class PortalAuthInfo
{
    public long UserId { get; init; }
    public string Gid { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public long? CompanyId { get; init; }
    public int? SbuId { get; init; }
    public long? TeamId { get; init; }
    public List<AppRole> Roles { get; init; } = [];

    public string FullName => $"{FirstName} {LastName}";
    public bool HasRole(AppRole role) => Roles.Contains(role);
    public bool IsRoot => HasRole(AppRole.Root);
}