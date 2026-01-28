using App.Bps.Enum;
using App.Shared.Web;
using Identity.Data;
using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Api.Seed;

public class SeedService : ISeedService
{
    private readonly IdentityDbContext _context;
    private readonly SeedSettings _settings;
    private readonly ILogger<SeedService> _logger;

    // ReSharper disable once ConvertToPrimaryConstructor
    public SeedService(IdentityDbContext context, IOptions<SeedSettings> settings, ILogger<SeedService> logger)
    {
        _context = context;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SeedDictionariesAsync()
    {
        _logger.LogInformation("Starting system dictionary seed...");

        // Always sync roles and permissions from enum definitions
        await SyncRolesAsync();
        await SyncPermissionsAsync();

        await SafeSeedAsync("StructureDict", SeedStructureDictAsync);
        await SafeSeedAsync("Teams", SeedTeamsAsync);
        await SafeSeedAsync("Users", SeedUsersAsync);
        await SafeSeedAsync("UserRoleAssignments", SeedUserRoleAssignmentsAsync);
        _logger.LogInformation("System dictionary seed completed");
    }

    public async Task SeedShowcaseDataAsync()
    {
        _logger.LogInformation("Starting showcase seed...");
        await SafeSeedAsync("Teams", SeedTeamsAsync);
        await SafeSeedAsync("Users", SeedUsersAsync);
        _logger.LogInformation("System showcase completed");
    }

    public Task ResetAndSeedAsync(bool includeShowcase)
    {
        return Task.CompletedTask;
    }


    #region Helper Methods for Resilience

    private async Task SafeSeedAsync(string entityName, Func<Task> seedAction)
    {
        try
        {
            await seedAction();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to seed {EntityName}, skipping (may already exist)", entityName);
            // Clear any tracked entities that failed to save
            _context.ChangeTracker.Clear();
        }
    }

    private async Task SafeDeleteAsync(string entityName, Func<Task<int>> deleteAction)
    {
        try
        {
            await deleteAction();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete {EntityName}, skipping", entityName);
        }
    }

    #endregion

    /// <summary>
    /// Synchronizes roles from AppRole enum to database. Always runs on startup.
    /// Inserts new roles and updates existing ones.
    /// </summary>
    private async Task SyncRolesAsync()
    {
        var definedRoles = RoleData.GetRoles();
        var existingRoles = await _context.AppUserRoles.ToDictionaryAsync(r => r.Id);

        foreach (var role in definedRoles)
        {
            if (existingRoles.TryGetValue(role.Id, out var existing))
            {
                // Update existing role
                existing.Name = role.Name;
                existing.Description = role.Description;
                existing.IsActive = role.IsActive;
            }
            else
            {
                // Insert new role
                _context.AppUserRoles.Add(role);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Synchronized {Count} roles from AppRole enum", definedRoles.Count);
    }

    /// <summary>
    /// Synchronizes permissions from AppPermission enum to database. Always runs on startup.
    /// Inserts new permissions and updates existing ones.
    /// </summary>
    private async Task SyncPermissionsAsync()
    {
        var definedPermissions = PermissionData.GetPermissions();
        var existingPermissions = await _context.AppPermissions.ToDictionaryAsync(p => p.Id);

        foreach (var permission in definedPermissions)
        {
            if (existingPermissions.TryGetValue(permission.Id, out var existing))
            {
                // Update existing permission
                existing.Name = permission.Name;
                existing.Description = permission.Description;
                existing.Category = permission.Category;
                existing.IsActive = permission.IsActive;
                existing.IsSpecial = permission.IsSpecial;
            }
            else
            {
                // Insert new permission
                _context.AppPermissions.Add(permission);
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Synchronized {Count} permissions from AppPermission enum", definedPermissions.Count);
    }

    private async Task SeedStructureDictAsync()
    {
        if (await _context.StructureDict.AnyAsync()) return;

        _context.StructureDict.AddRange(StructureData.GetStructureDict());
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded StructureDict");
    }

    private async Task SeedTeamsAsync()
    {
        if (await _context.Teams.AnyAsync()) return;

        _context.Teams.AddRange(TeamData.GetTeams());
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded Teams");
    }

    private async Task SeedUsersAsync()
    {
        if (await _context.Employees.AnyAsync()) return;

        var allUsers = UserData.GetAllUsers();
        _context.Employees.AddRange(allUsers);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} Users ({Active} active, {Inactive} inactive)",
            allUsers.Count,
            allUsers.Count(u => u.IsActive),
            allUsers.Count(u => !u.IsActive));
    }

    private async Task SeedUserRoleAssignmentsAsync()
    {
        if (await _context.AppUserRoleAssignments.AnyAsync()) return;

        // Only assign roles to active users
        var users = await _context.Employees
            .Where(u => u.IsActive)
            .ToListAsync();

        var assignments = new List<AppUserRoleAssignment>();

        foreach (var user in users)
        {
            // Everyone gets User role
            assignments.Add(new AppUserRoleAssignment
            {
                UserId = user.Id,
                RoleId = (int)AppRole.User,
                AssignedAt = DateTime.UtcNow
            });

            // Map Department to AppRole
            var appRole = user.Department switch
            {
                Department.Developers => AppRole.Root,
                Department.Support => AppRole.Support,
                Department.Operations => AppRole.Ops,
                Department.Finance => AppRole.Admin,
                _ => (AppRole?)null
            };

            if (appRole.HasValue && appRole.Value != AppRole.User)
            {
                assignments.Add(new AppUserRoleAssignment
                {
                    UserId = user.Id,
                    RoleId = (int)appRole.Value,
                    AssignedAt = DateTime.UtcNow
                });
            }
        }

        _context.AppUserRoleAssignments.AddRange(assignments);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} user role assignments for {UserCount} active users",
            assignments.Count, users.Count);
    }

    private async Task ResetAutoIncrementAsync()
    {
        var tablesToReset = new[]
        {
            "structure_dict",
            "teams",
            "users",
            "login_audit_logs",
            "password_reset_tokens"
        };

        foreach (var table in tablesToReset)
        {
            try
            {
                // Table names are from hardcoded array - safe from SQL injection
                #pragma warning disable EF1002
                await _context.Database.ExecuteSqlRawAsync($"ALTER TABLE `{table}` AUTO_INCREMENT = 1");
                #pragma warning restore EF1002
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to reset AUTO_INCREMENT for {Table}", table);
            }
        }

        _logger.LogInformation("Reset AUTO_INCREMENT counters");
    }
}
