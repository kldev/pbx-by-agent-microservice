using App.Bps.Enum;
using Identity.Data.Entities;

namespace Identity.Api.Seed;

/// <summary>
/// Seed data for Structure dictionary.
/// </summary>
public static class StructureData
{
    public static List<StructureDict> GetStructureDict() =>
    [
        new() { Id = 1, Code = "HQ", Name = "Headquarters", Region = StructureRegion.Poland }
    ];
}
