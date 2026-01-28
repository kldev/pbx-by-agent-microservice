using App.Shared.Web.BaseModel;
using App.Shared.Web.Security;
using Common.Toolkit.ResultPattern;
using Identity.Api.Features.Structure.Model;

namespace Identity.Api.Features.Structure;

public interface IStructureService
{
    Task<Result<StructureResponse>> CreateAsync(PortalAuthInfo auth, CreateStructureRequest request);
    Task<Result<StructureResponse>> GetByIdAsync(PortalAuthInfo auth, int id);
    Task<Result<List<StructureResponse>>> GetAllAsync(PortalAuthInfo auth, bool? isActive = null);
    Task<Result<PagedResult<StructureResponse>>> GetListAsync(PortalAuthInfo auth, StructureListFilter filter);
    Task<Result<StructureResponse>> UpdateAsync(PortalAuthInfo auth, int id, UpdateStructureRequest request);
}
