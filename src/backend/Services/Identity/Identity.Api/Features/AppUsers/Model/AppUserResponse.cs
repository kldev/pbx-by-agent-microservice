using App.Bps.Enum;

namespace Identity.Api.Features.AppUsers.Model;

public class AppUserResponse
{
    public long Id { get; set; }
    public string Gid { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Department? Department { get; set; }
    public int StructureId { get; set; }
    public long? TeamId { get; set; }
    public long? SupervisorId { get; set; }
    public string? SupervisorFullName { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
