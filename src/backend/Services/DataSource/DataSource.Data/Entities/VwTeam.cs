using App.Bps.Enum;

namespace DataSource.Data.Entities;

public class VwTeam : ViewEntityBase
{
    public string? SbuGid { get; set; }
    public int SbuId { get; set; }
    public TeamType Type { get; set; }
}
