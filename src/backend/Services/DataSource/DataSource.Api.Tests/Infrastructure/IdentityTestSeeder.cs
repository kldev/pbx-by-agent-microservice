using App.Bps.Enum;
using Identity.Data;
using Identity.Data.Entities;

namespace DataSource.Api.Tests.Infrastructure;

/// <summary>
/// Minimal seed data for Identity database (for DataSource view tests)
/// </summary>
public static class IdentityTestSeeder
{
    public static void SeedMinimalData(IdentityDbContext context)
    {
        if (context.SbuDict.Any()) return;

        // SBU
        context.SbuDict.AddRange(
            new SbuDict { Id = 1, Name = "SBU Poland", Code = "PL", Region = SbuRegion.Poland, IsActive = true },
            new SbuDict { Id = 2, Name = "SBU Foreign", Code = "INT", Region = SbuRegion.Foreign, IsActive = true }
        );
        context.SaveChanges();

        // Teams
        context.Teams.AddRange(
            new Team { Id = 1, Gid = Guid.NewGuid().ToString(), Name = "Team Alpha", Code = "ALPHA", SbuId = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Team { Id = 2, Gid = Guid.NewGuid().ToString(), Name = "Team Beta", Code = "BETA", SbuId = 2, IsActive = true, CreatedAt = DateTime.UtcNow }
        );
        context.SaveChanges();

        // Users
        context.Employees.AddRange(
            new AppUser
            {
                Id = 1,
                Gid = Guid.NewGuid().ToString(),
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@test.com",
                PasswordHash = "hash",
                SbuId = 1,
                TeamId = 1,
                Department = Department.Sales,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new AppUser
            {
                Id = 2,
                Gid = Guid.NewGuid().ToString(),
                FirstName = "Anna",
                LastName = "Nowak",
                Email = "anna.nowak@test.com",
                PasswordHash = "hash",
                SbuId = 1,
                TeamId = 1,
                Department = Department.Recruitment,
                SupervisorId = 1, // Jan is supervisor
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
        context.SaveChanges();
    }
}
