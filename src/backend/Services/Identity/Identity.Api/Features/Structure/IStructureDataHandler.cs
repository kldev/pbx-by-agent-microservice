using Identity.Api.Features.Structure.Model;
using Identity.Data.Entities;

namespace Identity.Api.Features.Structure;

public interface IStructureDataHandler
{
    Task<StructureDict> CreateAsync(StructureDict entity);
    Task<StructureDict?> GetByIdAsync(int id);
    Task<StructureDict?> GetByCodeAsync(string code);
    Task<List<StructureDict>> GetAllAsync(bool? isActive = null);
    Task<(List<StructureDict> Items, int Total)> GetPagedAsync(StructureListFilter filter);
    Task<bool> HasTeamsAsync(int structureId);
    Task UpdateAsync(StructureDict entity);
}
