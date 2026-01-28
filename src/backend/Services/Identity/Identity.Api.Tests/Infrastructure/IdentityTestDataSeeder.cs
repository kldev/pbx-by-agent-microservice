using App.Bps.Enum;
using App.Shared.Tests.Infrastructure;
using Identity.Api.Infrastructure;
using Identity.Api.Seed;
using Identity.Data;
using Identity.Data.Entities;

namespace Identity.Api.Tests.Infrastructure;

/// <summary>
/// Seeds test data for Identity service integration tests.
/// Uses fixed IDs for deterministic and reproducible tests.
/// </summary>
public static class IdentityTestDataSeeder
{
    /// <summary>
    /// Default password for test users: "Test123!"
    /// </summary>
    public const string TestPassword = "Test123!";
    private static readonly string TestPasswordHash = PasswordHasher.Hash(TestPassword);

    public static void SeedTestData(IdentityDbContext context)
    {
        // Seed Roles from enum definitions (like production)
        if (!context.AppUserRoles.Any())
        {
            context.AppUserRoles.AddRange(RoleData.GetRoles());
            context.SaveChanges();
        }

        // Seed Permissions from enum definitions (like production)
        if (!context.AppPermissions.Any())
        {
            context.AppPermissions.AddRange(PermissionData.GetPermissions());
            context.SaveChanges();
        }

        // Seed Structures
        if (!context.StructureDict.Any())
        {
            context.StructureDict.AddRange(
                new StructureDict
                {
                    Id = TestFixtureIds.Ids.TestStructure1,
                    Name = "Test Structure 1",
                    Code = "STR1",
                    Region = StructureRegion.Poland,
                    IsActive = true
                },
                new StructureDict
                {
                    Id = TestFixtureIds.Ids.TestStructure2,
                    Name = "Test Structure 2",
                    Code = "STR2",
                    Region = StructureRegion.Poland,
                    IsActive = true
                }
            );
            context.SaveChanges();
        }

        // Seed Teams
        if (!context.Teams.Any())
        {
            context.Teams.AddRange(
                new Team
                {
                    Id = TestFixtureIds.Ids.TestTeam1,
                    Gid = TestFixtureIds.Gids.TestTeam1,
                    Name = "Test Team 1",
                    Code = "TEAM1",
                    StructureId = TestFixtureIds.Ids.TestStructure1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Team
                {
                    Id = TestFixtureIds.Ids.TestTeam2,
                    Gid = TestFixtureIds.Gids.TestTeam2,
                    Name = "Test Team 2",
                    Code = "TEAM2",
                    StructureId = TestFixtureIds.Ids.TestStructure2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
            context.SaveChanges();
        }

        // Seed Users
        if (!context.Employees.Any())
        {
            context.Employees.AddRange(
                new AppUser
                {
                    Id = TestFixtureIds.Ids.TestUser1,
                    Gid = TestFixtureIds.Gids.TestUser1,
                    FirstName = "Test",
                    LastName = "User1",
                    Email = "test.user1@example.com",
                    PasswordHash = TestPasswordHash,
                    StructureId = TestFixtureIds.Ids.TestStructure1,
                    TeamId = TestFixtureIds.Ids.TestTeam1,
                    Department = Department.Developers,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new AppUser
                {
                    Id = TestFixtureIds.Ids.TestUser2,
                    Gid = TestFixtureIds.Gids.TestUser2,
                    FirstName = "Test",
                    LastName = "User2",
                    Email = "test.user2@example.com",
                    PasswordHash = TestPasswordHash,
                    StructureId = TestFixtureIds.Ids.TestStructure2,
                    TeamId = TestFixtureIds.Ids.TestTeam2,
                    Department = Department.Support,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new AppUser
                {
                    Id = TestFixtureIds.Ids.TestUser3,
                    Gid = TestFixtureIds.Gids.TestUser3,
                    FirstName = "Inactive",
                    LastName = "User",
                    Email = "inactive.user@example.com",
                    PasswordHash = TestPasswordHash,
                    StructureId = TestFixtureIds.Ids.TestStructure1,
                    Department = Department.Operations,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow
                }
            );
            context.SaveChanges();
        }

        // Seed Role Assignments for test users
        if (!context.AppUserRoleAssignments.Any())
        {
            context.AppUserRoleAssignments.AddRange(
                // User1 (Admin) gets Root and User
                new AppUserRoleAssignment
                {
                    UserId = TestFixtureIds.Ids.TestUser1,
                    RoleId = (int)AppRole.Root,
                    AssignedAt = DateTime.UtcNow
                },
                new AppUserRoleAssignment
                {
                    UserId = TestFixtureIds.Ids.TestUser1,
                    RoleId = (int)AppRole.User,
                    AssignedAt = DateTime.UtcNow
                },
                // User2 (Support) gets Support and User
                new AppUserRoleAssignment
                {
                    UserId = TestFixtureIds.Ids.TestUser2,
                    RoleId = (int)AppRole.Support,
                    AssignedAt = DateTime.UtcNow
                },
                new AppUserRoleAssignment
                {
                    UserId = TestFixtureIds.Ids.TestUser2,
                    RoleId = (int)AppRole.User,
                    AssignedAt = DateTime.UtcNow
                }
                // User3 (inactive) - no role assignments
            );
            context.SaveChanges();
        }
    }
}
