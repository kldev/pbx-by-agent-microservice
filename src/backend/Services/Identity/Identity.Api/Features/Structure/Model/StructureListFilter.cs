using App.Bps.Enum;
using App.Shared.Web.BaseModel;

namespace Identity.Api.Features.Structure.Model;

public class StructureListFilter : PagedRequest
{
    public string? Search { get; set; }
    public StructureRegion? Region { get; set; }
    public bool? IsActive { get; set; }
}
