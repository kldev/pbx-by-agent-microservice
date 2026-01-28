namespace CdrService.Api.Features.TerminationCauses.Model;

public class TerminationCauseResponse
{
    public string Gid { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int? Q850Code { get; set; }
    public string NamePL { get; set; } = null!;
    public string NameEN { get; set; } = null!;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}
