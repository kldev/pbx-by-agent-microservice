namespace CdrService.Api.Features.CallStatuses.Model;

public class CallStatusResponse
{
    public string Gid { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string NamePL { get; set; } = null!;
    public string NameEN { get; set; } = null!;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}
