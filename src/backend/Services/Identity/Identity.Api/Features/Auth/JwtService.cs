using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Data.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Features.Auth;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("UserId", user.Id.ToString()),
            new("Gid", user.Gid),
            new(ClaimTypes.Email, user.Email),
            new("FirstName", user.FirstName),
            new("LastName", user.LastName)
        };

        // Add role claims for all assigned roles
        if (user.RoleAssignments != null)
        {
            foreach (var roleAssignment in user.RoleAssignments)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleAssignment.Role.Name));
            }
        }

        // Add StructureId (always present) and TeamId (if available)
        claims.Add(new Claim("StructureId", user.StructureId.ToString()));
        if (user.TeamId.HasValue)
            claims.Add(new Claim("TeamId", user.TeamId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: GetExpiration(),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetExpiration()
    {
        return DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes);
    }
}
