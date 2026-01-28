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

        // Development team
        CreateUser("Jan", "Nowak", Department.Developers, StructureIds.MainSbu, TeamData.TeamIds.Development),
        CreateUser("Anna", "Kowalska", Department.Developers, StructureIds.MainSbu, TeamData.TeamIds.Development),

        // Support team
        CreateUser("Piotr", "Wiśniewski", Department.Support, StructureIds.MainSbu, TeamData.TeamIds.Support),

        // Operations team
        CreateUser("Maria", "Zielińska", Department.Operations, StructureIds.MainSbu, TeamData.TeamIds.Operations)
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
        StructureId = StructureIds.MainSbu,
        TeamId = TeamData.TeamIds.Development,
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
