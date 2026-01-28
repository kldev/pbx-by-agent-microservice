using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using Identity.Api.Features.AppUsers.Model;
using Identity.Api.Infrastructure;
using Identity.Data;
using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Features.AppUsers;

public class AppUserService : IAppUserService
{
    private readonly IAppUserDataHandler _dataHandler;
    private readonly IdentityDbContext _context;

    public AppUserService(IAppUserDataHandler dataHandler, IdentityDbContext context)
    {
        _dataHandler = dataHandler;
        _context = context;
    }

    public async Task<Result<AppUserResponse>> CreateAsync(PortalAuthInfo auth, CreateAppUserRequest request)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
            validationErrors.Add("Imię jest wymagane");
        else if (request.FirstName.Length > 100)
            validationErrors.Add("Imię nie może mieć więcej niż 100 znaków");

        if (string.IsNullOrWhiteSpace(request.LastName))
            validationErrors.Add("Nazwisko jest wymagane");
        else if (request.LastName.Length > 100)
            validationErrors.Add("Nazwisko nie może mieć więcej niż 100 znaków");

        if (string.IsNullOrWhiteSpace(request.Email))
            validationErrors.Add("Email jest wymagany");

        if (string.IsNullOrWhiteSpace(request.Password))
            validationErrors.Add("Hasło jest wymagane");
        else if (request.Password.Length < 8)
            validationErrors.Add("Hasło musi mieć co najmniej 8 znaków");

        if (validationErrors.Count > 0)
            return Result<AppUserResponse>.Failure(new ValidationError(validationErrors));

        var existingEmail = await _dataHandler.GetByEmailAsync(request.Email);
        if (existingEmail != null)
            return Result<AppUserResponse>.Failure(new BusinessLogicError("appuser.email_exists", "Podany email jest już zajęty"));

        var entity = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Department = request.Department,
            StructureId = request.StructureId,
            TeamId = request.TeamId,
            IsActive = true,
            CreatedByUserId = auth.UserId
        };

        await _dataHandler.CreateAsync(entity);

        // Assign roles
        if (request.Roles.Count > 0)
        {
            await AssignRolesToUserAsync(entity.Id, request.Roles, auth.UserId);
        }

        // Reload with roles and supervisor (agregat)
        var created = await _context.EmployeesWithRoles
            .FirstAsync(x => x.Id == entity.Id);

        return Result<AppUserResponse>.Success(MapToResponse(created));
    }

    public async Task<Result<AppUserResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var entity = await _dataHandler.GetByGidWithRolesAsync(gid);
        if (entity == null)
            return Result<AppUserResponse>.Failure(new NotFoundError("appuser.not_found", "Użytkownik nie został znaleziony"));

        return Result<AppUserResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<PagedResult<AppUserResponse>>> GetListAsync(PortalAuthInfo auth, AppUserListFilter filter)
    {
        var (items, total) = await _dataHandler.GetPagedAsync(filter);

        var result = new PagedResult<AppUserResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = total,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Result<PagedResult<AppUserResponse>>.Success(result);
    }

    public async Task<Result<AppUserResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateAppUserRequest request)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
            validationErrors.Add("Imię jest wymagane");
        else if (request.FirstName.Length > 100)
            validationErrors.Add("Imię nie może mieć więcej niż 100 znaków");

        if (string.IsNullOrWhiteSpace(request.LastName))
            validationErrors.Add("Nazwisko jest wymagane");
        else if (request.LastName.Length > 100)
            validationErrors.Add("Nazwisko nie może mieć więcej niż 100 znaków");

        if (string.IsNullOrWhiteSpace(request.Email))
            validationErrors.Add("Email jest wymagany");

        if (validationErrors.Count > 0)
            return Result<AppUserResponse>.Failure(new ValidationError(validationErrors));

        var entity = await _dataHandler.GetByGidAsync(gid);
        if (entity == null)
            return Result<AppUserResponse>.Failure(new NotFoundError("appuser.not_found", "Użytkownik nie został znaleziony"));

        if (entity.Email != request.Email)
        {
            var existingEmail = await _dataHandler.GetByEmailAsync(request.Email);
            if (existingEmail != null)
                return Result<AppUserResponse>.Failure(new BusinessLogicError("appuser.email_exists", "Podany email jest już zajęty"));
        }

        entity.FirstName = request.FirstName;
        entity.LastName = request.LastName;
        entity.Email = request.Email;
        entity.Department = request.Department;
        entity.StructureId = request.StructureId;
        entity.TeamId = request.TeamId;
        entity.SupervisorId = request.SupervisorId;
        entity.IsActive = request.IsActive;
        entity.ModifiedByUserId = auth.UserId;

        await _dataHandler.UpdateAsync(entity);

        // Update roles
        await UpdateUserRolesAsync(entity.Id, request.Roles, auth.UserId);

        // Reload with roles and supervisor (agregat)
        var updated = await _context.EmployeesWithRoles
            .FirstAsync(x => x.Id == entity.Id);

        return Result<AppUserResponse>.Success(MapToResponse(updated));
    }

    public async Task<Result<AppUserResponse>> SetSupervisorAsync(PortalAuthInfo auth, string gid, SetSupervisorRequest request)
    {
        var entity = await _dataHandler.GetByGidAsync(gid);
        if (entity == null)
            return Result<AppUserResponse>.Failure(new NotFoundError("appuser.not_found", "Użytkownik nie został znaleziony"));

        // Validate supervisor exists if provided
        if (request.SupervisorId.HasValue)
        {
            var supervisor = await _dataHandler.GetByIdAsync(request.SupervisorId.Value);
            if (supervisor == null)
                return Result<AppUserResponse>.Failure(new NotFoundError("appuser.supervisor_not_found", "Przełożony nie został znaleziony"));

            // Prevent self-reference
            if (supervisor.Id == entity.Id)
                return Result<AppUserResponse>.Failure(new BusinessLogicError("appuser.self_supervisor", "Użytkownik nie może być swoim własnym przełożonym"));
        }

        entity.SupervisorId = request.SupervisorId;
        entity.ModifiedByUserId = auth.UserId;

        await _dataHandler.UpdateAsync(entity);

        // Reload with roles and supervisor (agregat)
        var updated = await _context.EmployeesWithRoles
            .FirstAsync(x => x.Id == entity.Id);

        return Result<AppUserResponse>.Success(MapToResponse(updated));
    }

    public async Task<Result<bool>> ChangePasswordAsync(PortalAuthInfo auth, string gid, ChangePasswordRequest request)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            validationErrors.Add("Nowe hasło jest wymagane");
        else if (request.NewPassword.Length < 8)
            validationErrors.Add("Nowe hasło musi mieć co najmniej 8 znaków");

        if (validationErrors.Count > 0)
            return Result<bool>.Failure(new ValidationError(validationErrors));

        var entity = await _dataHandler.GetByGidAsync(gid);
        if (entity == null)
            return Result<bool>.Failure(new NotFoundError("appuser.not_found", "Użytkownik nie został znaleziony"));

        entity.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        entity.ModifiedByUserId = auth.UserId;

        await _dataHandler.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid)
    {
        var entity = await _dataHandler.GetByGidAsync(gid);
        if (entity == null)
            return Result<bool>.Failure(new NotFoundError("appuser.not_found", "Użytkownik nie został znaleziony"));

        await _dataHandler.SoftDeleteAsync(entity);
        return Result<bool>.Success(true);
    }

    private static AppUserResponse MapToResponse(AppUser entity) => new()
    {
        Id = entity.Id,
        Gid = entity.Gid,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Email = entity.Email,
        Department = entity.Department,
        StructureId = entity.StructureId,
        TeamId = entity.TeamId,
        SupervisorId = entity.SupervisorId,
        SupervisorFullName = entity.Supervisor != null
            ? $"{entity.Supervisor.FirstName} {entity.Supervisor.LastName}"
            : null,
        IsActive = entity.IsActive,
        Roles = entity.RoleAssignments?.Select(ra => ra.Role.Name).ToList() ?? new List<string>(),
        CreatedAt = entity.CreatedAt,
        ModifiedAt = entity.ModifiedAt
    };

    private async Task AssignRolesToUserAsync(long userId, List<string> roleNames, long assignedBy)
    {
        var roles = await _context.AppUserRoles
            .Where(r => roleNames.Contains(r.Name))
            .ToListAsync();

        foreach (var role in roles)
        {
            _context.AppUserRoleAssignments.Add(new AppUserRoleAssignment
            {
                UserId = userId,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = assignedBy
            });
        }

        await _context.SaveChangesAsync();
    }

    private async Task UpdateUserRolesAsync(long userId, List<string> roleNames, long assignedBy)
    {
        // Remove existing roles
        var existingAssignments = await _context.AppUserRoleAssignments
            .Where(ra => ra.UserId == userId)
            .ToListAsync();

        _context.AppUserRoleAssignments.RemoveRange(existingAssignments);

        // Add new roles
        if (roleNames.Count > 0)
        {
            var roles = await _context.AppUserRoles
                .Where(r => roleNames.Contains(r.Name))
                .ToListAsync();

            foreach (var role in roles)
            {
                _context.AppUserRoleAssignments.Add(new AppUserRoleAssignment
                {
                    UserId = userId,
                    RoleId = role.Id,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = assignedBy
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}
