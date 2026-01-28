namespace Identity.Api.Features.Auth.Model;

public record ValidateLoginResponse(
    bool IsValid,
    long? UserId,
    string? Gid,
    string? Email,
    string? FirstName,
    string? LastName,
    List<string> Roles,
    int? StructureId,
    string? ErrorCode,
    string? ErrorMessage
);
