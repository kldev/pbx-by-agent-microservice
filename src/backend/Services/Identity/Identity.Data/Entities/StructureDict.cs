using App.Bps.Enum;

namespace Identity.Data.Entities;

public class StructureDict
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public StructureRegion Region { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<AppUser> Employees { get; set; } = new List<AppUser>();
}
