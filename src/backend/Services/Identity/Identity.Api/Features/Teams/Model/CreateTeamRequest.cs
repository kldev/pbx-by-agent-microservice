namespace Identity.Api.Features.Teams.Model;

public class CreateTeamRequest
{
    public int StructureId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
