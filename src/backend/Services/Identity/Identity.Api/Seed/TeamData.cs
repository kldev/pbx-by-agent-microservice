using App.Bps.Enum;
using Identity.Data.Entities;
using Seed.Shared;

namespace Identity.Api.Seed;

/// <summary>
/// Seed data for Teams.
/// Uses shared GUIDs from Seed.Shared for cross-service consistency.
/// </summary>
public static class TeamData
{
    /// <summary>
    /// Team IDs for reference in UserData
    /// </summary>
    public static class TeamIds
    {
        // Poland
        public const int WolfPack = 1;
        public const int TopGunners = 2;

        // Foreign
        public const int MadMen = 3;
        public const int VictorySquad = 4;
        public const int PitchPerfect = 5;
    }

    public static List<Team> GetTeams() =>
    [
        // Poland teams
        new()
        {
            Id = TeamIds.WolfPack,
            Gid = SeedGuids.Teams.WolfPackGid,
            StructureId = StructureData.StructureIds.Poland,
            Code = "WOLF",
            Name = SeedGuids.Teams.WolfPackName,
            Type = TeamType.Sales,
            IsActive = true
        },
        new()
        {
            Id = TeamIds.TopGunners,
            Gid = SeedGuids.Teams.TopGunnersGid,
            StructureId = StructureData.StructureIds.Poland,
            Code = "TOPG",
            Name = SeedGuids.Teams.TopGunnersName,
            Type = TeamType.Sales,
            IsActive = true
        },

        // Foreign teams
        new()
        {
            Id = TeamIds.MadMen,
            Gid = SeedGuids.Teams.MadMenGid,
            StructureId = StructureData.StructureIds.Foreign,
            Code = "MADM",
            Name = SeedGuids.Teams.MadMenName,
            Type = TeamType.Sales,
            IsActive = true
        },
        new()
        {
            Id = TeamIds.VictorySquad,
            Gid = SeedGuids.Teams.VictorySquadGid,
            StructureId = StructureData.StructureIds.Foreign,
            Code = "VICT",
            Name = SeedGuids.Teams.VictorySquadName,
            Type = TeamType.Sales,
            IsActive = true
        },
        new()
        {
            Id = TeamIds.PitchPerfect,
            Gid = SeedGuids.Teams.PitchPerfectGid,
            StructureId = StructureData.StructureIds.Foreign,
            Code = "PITCH",
            Name = SeedGuids.Teams.PitchPerfectName,
            Type = TeamType.Sales,
            IsActive = true
        }
    ];
}
