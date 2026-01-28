using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gateway.Api.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Api.Auth;

public interface IJwtService
{
    string GenerateToken(ValidateLoginResponse userInfo);
    DateTime GetExpiration();
}

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateToken(ValidateLoginResponse userInfo)
    {
        var claims = new List<Claim>
        {
            new("UserId", userInfo.UserId?.ToString() ?? string.Empty),
            new("Gid", userInfo.Gid ?? string.Empty),
            new(ClaimTypes.Email, userInfo.Email ?? string.Empty),
            new("FirstName", userInfo.FirstName ?? string.Empty),
            new("LastName", userInfo.LastName ?? string.Empty),
            new("SbuId", userInfo.SbuId?.ToString() ?? string.Empty)
        };

        // Add multiple role claims - one for each role the user has
        if (userInfo.Roles != null)
        {
            foreach (var role in userInfo.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

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
