using App.Bps.Enum;
using Identity.Data.Entities;

namespace Identity.Api.Seed;

/// <summary>
/// Seed data for Structure dictionary.
/// </summary>
public static class StructureData
{
    public static class StructureIds
    {
        public const int Poland = 1;
        public const int Foreign = 2;
    }

    public static List<StructureDict> GetStructureDict() =>
    [
        new() { Id = StructureIds.Poland, Code = "PL", Name = "Poland", Region = StructureRegion.Poland },
        new() { Id = StructureIds.Foreign, Code = "INT", Name = "International", Region = StructureRegion.Foreign }
    ];
}
