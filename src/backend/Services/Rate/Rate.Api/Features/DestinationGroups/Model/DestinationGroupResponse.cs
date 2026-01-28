namespace RateService.Api.Features.DestinationGroups.Model;

public class DestinationGroupResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string NamePL { get; set; } = null!;
    public string? NameEN { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
