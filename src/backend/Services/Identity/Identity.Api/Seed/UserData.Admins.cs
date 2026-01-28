using App.Bps.Enum;
using Identity.Data.Entities;

namespace Identity.Api.Seed;

/// <summary>
/// Seed users for PBX system.
/// </summary>
public static partial class UserData
{
    /// <summary>
    /// All test users
    /// </summary>
    public static List<AppUser> GetUsers() =>
    [
        // Test admin account - DO NOT REMOVE - used in E2E tests
        CreateTestAdmin(),

        // Poland - WolfPack team
        CreateUser("Jan", "Nowak", Department.Sales, StructureData.StructureIds.Poland, TeamData.TeamIds.WolfPack),
        CreateUser("Anna", "Kowalska", Department.Sales, StructureData.StructureIds.Poland, TeamData.TeamIds.WolfPack),

        // Poland - TopGunners team
        CreateUser("Piotr", "Wiśniewski", Department.Support, StructureData.StructureIds.Poland, TeamData.TeamIds.TopGunners),

        // Foreign - MadMen team
        CreateUser("Maria", "Zielińska", Department.Sales, StructureData.StructureIds.Foreign, TeamData.TeamIds.MadMen)
    ];

    /// <summary>
    /// Creates test admin account with fixed email admin@pbx.local.
    /// Used in E2E tests - password: Agent666
    /// </summary>
    private static AppUser CreateTestAdmin() => new()
    {
        Gid = Guid.NewGuid().ToString(),
        Email = "admin@pbx.local",
        PasswordHash = DefaultPasswordHash,
        FirstName = "Test",
        LastName = "Admin",
        Department = Department.Developers,
        StructureId = StructureData.StructureIds.Poland,
        TeamId = TeamData.TeamIds.WolfPack,
        IsActive = true
    };

    private static AppUser CreateUser(
        string firstName,
        string lastName,
        Department department,
        int structureId,
        int teamId,
        bool isActive = true) => new()
    {
        Gid = Guid.NewGuid().ToString(),
        Email = GenerateEmail(firstName, lastName),
        PasswordHash = DefaultPasswordHash,
        FirstName = firstName,
        LastName = lastName,
        Department = department,
        StructureId = structureId,
        TeamId = teamId,
        IsActive = isActive
    };
}
