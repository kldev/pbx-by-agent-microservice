using App.Bps.Enum;
using App.Shared.Web;
using Identity.Api.Seed;
using Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Testcontainers.MySql;
using Xunit;
using Xunit.Abstractions;

namespace Identity.Api.Tests.Seed;

/// <summary>
/// Tests that verify the SeedService.SeedDictionariesAsync() correctly seeds showcase data.
/// These tests use a fresh database with no pre-seeded data and call the real SeedService.
/// This validates the same data that is used by the frontend showcase app.
/// </summary>
public class ShowcaseDataSeedTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private MySqlContainer? _container;
    private IdentityDbContext? _context;

    public ShowcaseDataSeedTests(ITestOutputHelper output)
    {
        _output = output;
    }

    public async Task InitializeAsync()
    {
        // Create a fresh MySQL container for each test class
        _container = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithDatabase("identity_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseMySQL(_container.GetConnectionString())
            .Options;

        _context = new IdentityDbContext(options);

        // Apply migrations
        await _context.Database.MigrateAsync();

        // Run the real SeedService (same as production/showcase app)
        var seedSettings = Options.Create(new SeedSettings());
        var seedService = new SeedService(_context, seedSettings, NullLogger<SeedService>.Instance);
        await seedService.SeedDictionariesAsync();
    }

    public async Task DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }
        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }

    [Fact]
    public async Task Showcase_Data_Should_Be_Working_AdminAccount_Has_Root_Role()
    {
        // Arrange - admin@pbx.local should have Root role
        var adminUser = await _context!.Employees
            .Include(u => u.RoleAssignments!)
            .ThenInclude(ra => ra.Role)
            .FirstOrDefaultAsync(u => u.Email == "admin@pbx.local");

        // Assert
        Assert.NotNull(adminUser);
        Assert.True(adminUser.IsActive, "Admin account should be active");
        Assert.Equal(Department.Developers, adminUser.Department);
        Assert.NotNull(adminUser.RoleAssignments);

        var roleNames = adminUser.RoleAssignments.Select(ra => ra.Role.Name).ToList();
        _output.WriteLine($"admin@pbx.local roles: {string.Join(", ", roleNames)}");

        Assert.Contains("Root", roleNames);
        Assert.Contains("User", roleNames);
    }

    [Fact]
    public async Task Showcase_Data_Should_Be_Working_StructureDict_Are_Seeded()
    {
        var structures = await _context!.StructureDict.ToListAsync();

        Assert.NotEmpty(structures);
        _output.WriteLine($"Seeded Structures: {structures.Count}");
        foreach (var structure in structures)
        {
            _output.WriteLine($"  - Structure {structure.Id}: {structure.Name} ({structure.Code}), Region: {structure.Region}");
        }

        // Verify at least one Structure exists
        Assert.True(structures.Count >= 1);
    }

    [Fact]
    public async Task Showcase_Data_Should_Be_Working_Teams_Are_Seeded()
    {
        var teams = await _context!.Teams.Include(t => t.Structure).ToListAsync();

        Assert.NotEmpty(teams);
        _output.WriteLine($"Seeded Teams: {teams.Count}");
        foreach (var team in teams.Take(10))
        {
            _output.WriteLine($"  - Team {team.Id}: {team.Name} ({team.Code}), Structure: {team.Structure?.Name}");
        }
        if (teams.Count > 10)
        {
            _output.WriteLine($"  ... and {teams.Count - 10} more teams");
        }
    }

    [Fact]
    public async Task Showcase_Data_Should_Be_Working_Users_Are_Seeded()
    {
        var users = await _context!.Employees.ToListAsync();

        Assert.NotEmpty(users);
        _output.WriteLine($"Seeded Users: {users.Count}");

        var activeUsers = users.Count(u => u.IsActive);
        var inactiveUsers = users.Count(u => !u.IsActive);
        _output.WriteLine($"  Active: {activeUsers}, Inactive: {inactiveUsers}");

        // Show some sample users
        foreach (var user in users.Take(5))
        {
            _output.WriteLine($"  - {user.FirstName} {user.LastName} ({user.Email}), Department: {user.Department}");
        }
    }

    [Fact]
    public async Task Showcase_Data_Should_Be_Working_Roles_Are_Seeded()
    {
        var roles = await _context!.AppUserRoles.ToListAsync();

        Assert.NotEmpty(roles);
        _output.WriteLine($"Seeded Roles: {roles.Count}");
        foreach (var role in roles)
        {
            _output.WriteLine($"  - {role.Id}: {role.Name} - {role.Description}");
        }

        // Verify essential roles exist
        var roleNames = roles.Select(r => r.Name).ToList();
        Assert.Contains("Root", roleNames);
        Assert.Contains("Admin", roleNames);
        Assert.Contains("Ops", roleNames);
        Assert.Contains("User", roleNames);
    }

    [Fact]
    public async Task Showcase_Data_Should_Be_Working_RoleAssignments_Are_Seeded()
    {
        var roleAssignments = await _context!.AppUserRoleAssignments
            .Include(ra => ra.User)
            .Include(ra => ra.Role)
            .ToListAsync();

        Assert.NotEmpty(roleAssignments);
        _output.WriteLine($"Seeded Role Assignments: {roleAssignments.Count}");

        // Count roles by type
        var roleGroups = roleAssignments
            .GroupBy(ra => ra.Role.Name)
            .Select(g => new { Role = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count);

        foreach (var group in roleGroups)
        {
            _output.WriteLine($"  - {group.Role}: {group.Count} users");
        }
    }

    [Fact]
    public async Task Showcase_Data_Should_Be_Working_Developers_Have_Root_Role()
    {
        // All Developers department users should have Root role
        var devUsers = await _context!.Employees
            .Include(u => u.RoleAssignments!)
            .ThenInclude(ra => ra.Role)
            .Where(u => u.Department == Department.Developers && u.IsActive)
            .ToListAsync();

        Assert.NotEmpty(devUsers);
        _output.WriteLine($"Developer Users: {devUsers.Count}");

        foreach (var user in devUsers)
        {
            var roles = user.RoleAssignments?.Select(ra => ra.Role.Name).ToList() ?? new List<string>();
            _output.WriteLine($"  - {user.FirstName} {user.LastName}: {string.Join(", ", roles)}");

            Assert.Contains("Root", roles);
            Assert.Contains("User", roles);
        }
    }

    [Fact]
    public async Task Showcase_Data_Should_Be_Working_All_Active_Users_Have_User_Role()
    {
        // Every active user should have User role
        var activeUsers = await _context!.Employees
            .Include(u => u.RoleAssignments!)
            .ThenInclude(ra => ra.Role)
            .Where(u => u.IsActive)
            .ToListAsync();

        Assert.NotEmpty(activeUsers);
        _output.WriteLine($"Active Users with User role check: {activeUsers.Count}");

        var usersWithoutUserRole = new List<string>();
        foreach (var user in activeUsers)
        {
            var roles = user.RoleAssignments?.Select(ra => ra.Role.Name).ToList() ?? new List<string>();
            if (!roles.Contains("User"))
            {
                usersWithoutUserRole.Add($"{user.FirstName} {user.LastName} ({user.Email})");
            }
        }

        if (usersWithoutUserRole.Any())
        {
            _output.WriteLine("Users without User role:");
            foreach (var user in usersWithoutUserRole)
            {
                _output.WriteLine($"  - {user}");
            }
        }

        Assert.Empty(usersWithoutUserRole);
    }
}
