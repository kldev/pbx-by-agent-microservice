using App.Bps.Enum;

namespace Gateway.Api.Auth;

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string Token,
    string Gid,
    string Email,
    string FirstName,
    string LastName,
    List<AppRole> Roles,
    DateTime ExpiresAt
);

public record ValidateLoginResponse(
    bool IsValid,
    long? UserId,
    string? Gid,
    string? Email,
    string? FirstName,
    string? LastName,
    List<AppRole>? Roles,
    int? SbuId,
    string? ErrorCode,
    string? ErrorMessage
);

public record ApiErrorResponse
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
