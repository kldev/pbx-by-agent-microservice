namespace Identity.Api.Features.Auth.Model;

public record LoginResponse(
    string Token,
    string Gid,
    string Email,
    string FirstName,
    string LastName,
    List<string> Roles,
    DateTime ExpiresAt
);
