using App.Bps.Enum;

namespace Identity.Api.Features.Auth.Model;

public record MeResponse(
    string Gid,
    string Email,
    string FirstName,
    string LastName,
    List<AppRole> Roles,
    bool IsActive,
    int StructureId,
    long? TeamId
);
