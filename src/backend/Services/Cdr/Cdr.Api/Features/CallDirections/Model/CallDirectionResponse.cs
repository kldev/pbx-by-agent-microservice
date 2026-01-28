namespace CdrService.Api.Features.CallDirections.Model;

public class CallDirectionResponse
{
    public string Gid { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string NamePL { get; set; } = null!;
    public string NameEN { get; set; } = null!;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}
