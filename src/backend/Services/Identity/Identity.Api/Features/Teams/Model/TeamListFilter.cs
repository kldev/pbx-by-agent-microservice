using App.Bps.Enum;
using App.Shared.Web.BaseModel;

namespace Identity.Api.Features.Teams.Model;

public class TeamListFilter : PagedRequest
{
    public string? Search { get; set; }
    public int? StructureId { get; set; }
    public bool? IsActive { get; set; }
    public TeamType? Type { get; set; }
}
