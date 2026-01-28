namespace RateService.Api.Features.DestinationGroups.Model;

public class CreateDestinationGroupRequest
{
    public string Name { get; set; } = null!;
    public string NamePL { get; set; } = null!;
    public string? NameEN { get; set; }
    public string? Description { get; set; }
}
