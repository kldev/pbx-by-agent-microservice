using App.Bps.Enum;
using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using Identity.Api.Features.Structure.Model;
using Identity.Data.Entities;

namespace Identity.Api.Features.Structure;

public class StructureService : IStructureService
{
    private readonly IStructureDataHandler _dataHandler;

    public StructureService(IStructureDataHandler dataHandler)
    {
        _dataHandler = dataHandler;
    }

    public async Task<Result<StructureResponse>> CreateAsync(PortalAuthInfo auth, CreateStructureRequest request)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Code))
            validationErrors.Add("Kod Structure jest wymagany");
        else if (request.Code.Length > 20)
            validationErrors.Add("Kod Structure nie moze miec wiecej niz 20 znakow");

        if (string.IsNullOrWhiteSpace(request.Name))
            validationErrors.Add("Nazwa Structure jest wymagana");
        else if (request.Name.Length > 100)
            validationErrors.Add("Nazwa Structure nie moze miec wiecej niz 100 znakow");

        if (validationErrors.Count > 0)
            return Result<StructureResponse>.Failure(new ValidationError(validationErrors));

        var existingCode = await _dataHandler.GetByCodeAsync(request.Code);
        if (existingCode != null)
            return Result<StructureResponse>.Failure(new BusinessLogicError("structure.code_exists", "Structure o podanym kodzie juz istnieje"));

        var entity = new StructureDict
        {
            Code = request.Code,
            Name = request.Name,
            Region = request.Region,
            IsActive = true
        };

        await _dataHandler.CreateAsync(entity);
        return Result<StructureResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<StructureResponse>> GetByIdAsync(PortalAuthInfo auth, int id)
    {
        var entity = await _dataHandler.GetByIdAsync(id);
        if (entity == null)
            return Result<StructureResponse>.Failure(new NotFoundError("structure.not_found", "Structure nie zostalo znalezione"));

        return Result<StructureResponse>.Success(MapToResponse(entity));
    }

    public async Task<Result<List<StructureResponse>>> GetAllAsync(PortalAuthInfo auth, bool? isActive = null)
    {
        var items = await _dataHandler.GetAllAsync(isActive);
        return Result<List<StructureResponse>>.Success(items.Select(MapToResponse).ToList());
    }

    public async Task<Result<PagedResult<StructureResponse>>> GetListAsync(PortalAuthInfo auth, StructureListFilter filter)
    {
        var (items, total) = await _dataHandler.GetPagedAsync(filter);

        var result = new PagedResult<StructureResponse>
        {
            Items = items.Select(MapToResponse),
            TotalCount = total,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };

        return Result<PagedResult<StructureResponse>>.Success(result);
    }

    public async Task<Result<StructureResponse>> UpdateAsync(PortalAuthInfo auth, int id, UpdateStructureRequest request)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Code))
            validationErrors.Add("Kod Structure jest wymagany");
        else if (request.Code.Length > 20)
            validationErrors.Add("Kod Structure nie moze miec wiecej niz 20 znakow");

        if (string.IsNullOrWhiteSpace(request.Name))
            validationErrors.Add("Nazwa Structure jest wymagana");
        else if (request.Name.Length > 100)
            validationErrors.Add("Nazwa Structure nie moze miec wiecej niz 100 znakow");

        if (validationErrors.Count > 0)
            return Result<StructureResponse>.Failure(new ValidationError(validationErrors));

        var entity = await _dataHandler.GetByIdAsync(id);
        if (entity == null)
            return Result<StructureResponse>.Failure(new NotFoundError("structure.not_found", "Structure nie zostalo znalezione"));

        if (entity.Code != request.Code)
        {
            var existingCode = await _dataHandler.GetByCodeAsync(request.Code);
            if (existingCode != null)
                return Result<StructureResponse>.Failure(new BusinessLogicError("structure.code_exists", "Structure o podanym kodzie juz istnieje"));
        }

        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.Region = request.Region;
        entity.IsActive = request.IsActive;

        await _dataHandler.UpdateAsync(entity);
        return Result<StructureResponse>.Success(MapToResponse(entity));
    }

    private static StructureResponse MapToResponse(StructureDict entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        Region = entity.Region,
        IsActive = entity.IsActive
    };
}
