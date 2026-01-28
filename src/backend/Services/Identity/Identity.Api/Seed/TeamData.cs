using App.Bps.Enum;
using Identity.Data.Entities;

namespace Identity.Api.Seed;

/// <summary>
/// Seed data for Teams.
/// </summary>
public static class TeamData
{
    public static class FixedGuids
    {
        public static readonly string Development = "11111111-0001-0001-0001-000000000001";
        public static readonly string Support = "11111111-0002-0001-0001-000000000001";
        public static readonly string Operations = "11111111-0003-0001-0001-000000000001";
    }

    /// <summary>
    /// Team IDs for reference in UserData
    /// </summary>
    public static class TeamIds
    {
        public const int Development = 1;
        public const int Support = 2;
        public const int Operations = 3;
    }

    public static List<Team> GetTeams() =>
    [
        new()
        {
            Gid = FixedGuids.Development,
            StructureId = 1,
            Code = "DEV",
            Name = "Development",
            Type = TeamType.IT,
            IsActive = true
        },
        new()
        {
            Gid = FixedGuids.Support,
            StructureId = 1,
            Code = "SUP",
            Name = "Support",
            Type = TeamType.Support,
            IsActive = true
        },
        new()
        {
            Gid = FixedGuids.Operations,
            StructureId = 1,
            Code = "OPS",
            Name = "Operations",
            Type = TeamType.Operations,
            IsActive = true
        }
    ];
}
