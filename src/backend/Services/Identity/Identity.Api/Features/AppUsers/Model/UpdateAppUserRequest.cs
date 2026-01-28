using App.Bps.Enum;

namespace Identity.Api.Features.AppUsers.Model;

public class UpdateAppUserRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Department? Department { get; set; }
    public int StructureId { get; set; }
    public long? TeamId { get; set; }
    public long? SupervisorId { get; set; }
    public bool IsActive { get; set; } = true;
    public List<string> Roles { get; set; } = new();
}
