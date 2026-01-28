namespace Identity.Api.Features.Teams.Model;

public class UpdateTeamRequest
{
    public int StructureId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
