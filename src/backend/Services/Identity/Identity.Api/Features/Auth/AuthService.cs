using Common.Toolkit.ResultPattern;
using Identity.Api.Features.AppUsers;
using Identity.Api.Features.Auth.Model;
using Identity.Api.Infrastructure;

namespace Identity.Api.Features.Auth;

public class AuthService : IAuthService
{
    private readonly IAppUserDataHandler _userDataHandler;
    private readonly ILogger<AuthService> _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public AuthService(
        IAppUserDataHandler userDataHandler,
        ILogger<AuthService> logger)
    {
        _userDataHandler = userDataHandler;
        _logger = logger;
    }

    public async Task<ValidateLoginResponse> ValidateLoginAsync(LoginRequest request)
    {
        var user = await _userDataHandler.GetByEmailAsync(request.Email);

        if (user == null)
        {
            _logger.LogWarning("Login validation failed: user not found for email {Email}", request.Email);
            return new ValidateLoginResponse(
                IsValid: false, UserId: null, Gid: null, Email: null, FirstName: null, LastName: null,
                Roles: [], StructureId: null, ErrorCode: "AUTH_INVALID_CREDENTIALS", ErrorMessage: "Invalid email or password");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login validation failed: user {Email} is inactive", request.Email);
            return new ValidateLoginResponse(
                IsValid: false, UserId: null, Gid: null, Email: null, FirstName: null, LastName: null,
                Roles: [], StructureId: null, ErrorCode: "AUTH_USER_INACTIVE", ErrorMessage: "User account is inactive");
        }

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login validation failed: invalid password for {Email}", request.Email);
            return new ValidateLoginResponse(
                IsValid: false, UserId: null, Gid: null, Email: null, FirstName: null, LastName: null,
                Roles: [], StructureId: null, ErrorCode: "AUTH_INVALID_CREDENTIALS", ErrorMessage: "Invalid email or password");
        }

        _logger.LogInformation("User {Email} validated successfully", request.Email);

        // Get roles from RoleAssignments (many-to-many)
        var roles = user.RoleAssignments
            .Select(ra => ra.Role.ToAppRole())
            .ToList();

        return new ValidateLoginResponse(
            IsValid: true,
            UserId: user.Id,
            Gid: user.Gid,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Roles: roles,
            StructureId: user.StructureId,
            ErrorCode: null,
            ErrorMessage: null);
    }

    public async Task<Result<MeResponse>> GetMeAsync(string userGid)
    {
        var user = await _userDataHandler.GetByGidWithRolesAsync(userGid);

        if (user == null)
        {
            return Result<MeResponse>.Failure(new NotFoundError("AUTH_USER_NOT_FOUND", "User not found"));
        }

        var roles = user.RoleAssignments
            .Select(ra => ra.Role.ToAppRole())
            .ToList();

        return Result<MeResponse>.Success(new MeResponse(
            Gid: user.Gid,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Roles: roles,
            IsActive: user.IsActive,
            StructureId: user.StructureId,
            TeamId: user.TeamId
        ));
    }

    public async Task<Result<JdPrefillResponse>> GetJdPrefillAsync(string userGid)
    {
        var user = await _userDataHandler.GetByGidWithSbuAndTeamAsync(userGid);

        if (user == null)
        {
            return Result<JdPrefillResponse>.Failure(new NotFoundError("AUTH_USER_NOT_FOUND", "User not found"));
        }

        return Result<JdPrefillResponse>.Success(new JdPrefillResponse(
            SalesId: user.Id,
            SalesGid: user.Gid,
            SalesName: $"{user.FirstName} {user.LastName}",
            StructureId: user.StructureId,
            SbuName: user.Structure?.Name ?? "",
            TeamId: user.TeamId,
            TeamGid: user.Team?.Gid,
            TeamName: user.Team?.Name
        ));
    }
}
