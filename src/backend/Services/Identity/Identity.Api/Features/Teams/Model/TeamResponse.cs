using App.Bps.Enum;

namespace Identity.Api.Features.Teams.Model;

public class TeamResponse
{
    public long Id { get; set; }
    public string Gid { get; set; } = null!;
    public int StructureId { get; set; }
    public string? SbuName { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public TeamType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
