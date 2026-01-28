using App.Bps.Enum;

namespace Identity.Api.Features.Structure.Model;

public class CreateStructureRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public StructureRegion Region { get; set; }
}
