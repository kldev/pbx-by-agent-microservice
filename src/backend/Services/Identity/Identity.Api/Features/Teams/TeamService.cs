using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using Identity.Api.Features.Teams.Model;
using Identity.Data.Entities;

namespace Identity.Api.Features.Teams;

public class TeamService : ITeamService
{
    private readonly ITeamDataHandler _dataHandler;

    public TeamService(ITeamDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<TeamResponse>> CreateAsync(PortalAuthInfo auth, CreateTeamRequest request)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Code))
            validationErrors.Add("Kod zespołu jest wymagany");
        else if (request.Code.Length > 20)
            validationErrors.Add("Kod zespołu nie może mieć więcej niż 20 znaków");

        if (string.IsNullOrWhiteSpace(request.Name))
            validationErrors.Add("Nazwa zespołu jest wymagana");
        else if (request.Name.Length > 100)
            validationErrors.Add("Nazwa zespołu nie może mieć więcej niż 100 znaków");

        if (validationErrors.Count > 0)
            return Result<TeamResponse>.Failure(new ValidationError(validationErrors));

        var existingCode = await _dataHandler.GetByCodeAsync(request.Code);
        if (existingCode != null)
            return Result<TeamResponse>.Failure(new BusinessLogicError("team.code_exists", "Zespół o podanym kodzie już istnieje"));

        var entity = new Team
        {
            StructureId = request.StructureId,
            Code = request.Code,
            Name = request.Name,
            IsActive = true
        };

        await _dataHandler.CreateAsync(entity);

        var created = await _dataHandler.GetByGidAsync(entity.Gid);
        return Result<TeamResponse>.Success(MapToResponse(created!));
    }

    public async Task<Result<TeamResponse>> GetByGidAsync(PortalAuthInfo auth, string gid)
    {
        var entity = await _dataHandler.GetByGidAsync(gid);
        if (entity == null)
            return Result<TeamResponse>.Failure(new NotFoundError("team.not_found", "Zespół nie został znaleziony"));

        return Result<TeamResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<PagedResult<TeamResponse>>> GetListAsync(PortalAuthInfo auth, TeamListFilter filter)
    {
        var (items, total) = await _dataHandler.GetPagedAsync(filter);

        var result = new PagedResult<TeamResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = total,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Result<PagedResult<TeamResponse>>.Success(result);
    }

    public async Task<Result<TeamResponse>> UpdateAsync(PortalAuthInfo auth, string gid, UpdateTeamRequest request)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Code))
            validationErrors.Add("Kod zespołu jest wymagany");
        else if (request.Code.Length > 20)
            validationErrors.Add("Kod zespołu nie może mieć więcej niż 20 znaków");

        if (string.IsNullOrWhiteSpace(request.Name))
            validationErrors.Add("Nazwa zespołu jest wymagana");
        else if (request.Name.Length > 100)
            validationErrors.Add("Nazwa zespołu nie może mieć więcej niż 100 znaków");

        if (validationErrors.Count > 0)
            return Result<TeamResponse>.Failure(new ValidationError(validationErrors));

        var entity = await _dataHandler.GetByGidAsync(gid);
        if (entity == null)
            return Result<TeamResponse>.Failure(new NotFoundError("team.not_found", "Zespół nie został znaleziony"));

        if (entity.Code != request.Code)
        {
            var existingCode = await _dataHandler.GetByCodeAsync(request.Code);
            if (existingCode != null)
                return Result<TeamResponse>.Failure(new BusinessLogicError("team.code_exists", "Zespół o podanym kodzie już istnieje"));
        }

        entity.StructureId = request.StructureId;
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.IsActive = request.IsActive;

        await _dataHandler.UpdateAsync(entity);

        var updated = await _dataHandler.GetByGidAsync(gid);
        return Result<TeamResponse>.Success(MapToResponse(updated!));
    }

    public async Task<Result<bool>> DeleteAsync(PortalAuthInfo auth, string gid)
    {
        var entity = await _dataHandler.GetByGidAsync(gid);
        if (entity == null)
            return Result<bool>.Failure(new NotFoundError("team.not_found", "Zespół nie został znaleziony"));

        var hasEmployees = await _dataHandler.HasEmployeesAsync(entity.Id);
        if (hasEmployees)
            return Result<bool>.Failure(new BusinessLogicError("team.has_employees", "Nie można usunąć zespołu z przypisanymi pracownikami"));

        await _dataHandler.SoftDeleteAsync(entity);
        return Result<bool>.Success(true);
    }

    private static TeamResponse MapToResponse(Team entity) => new()
    {
        Id = entity.Id,
        Gid = entity.Gid,
        StructureId = entity.StructureId,
        SbuName = entity.Structure?.Name,
        Code = entity.Code,
        Name = entity.Name,
        Type = entity.Type,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt,
        ModifiedAt = entity.ModifiedAt
    };
}
