using App.Bps.Enum;
using App.Shared.Web.BaseModel;

namespace Identity.Api.Features.AppUsers.Model;

public class AppUserListFilter : PagedRequest
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public Department? Department { get; set; }
    public int? StructureId { get; set; }
    public long? TeamId { get; set; }
}
