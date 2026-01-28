using App.BaseData;
using App.Bps.Enum;

namespace Identity.Data.Entities;

public class Team : BaseAuditableTable
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int StructureId { get; set; }
    public TeamType Type { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public StructureDict Structure { get; set; } = null!;
    public ICollection<AppUser> Employees { get; set; } = new List<AppUser>();
}
