using Identity.Data.Entities;

namespace Identity.Api.Features.Auth;

public interface IJwtService
{
    string GenerateToken(AppUser user);
    DateTime GetExpiration();
}
