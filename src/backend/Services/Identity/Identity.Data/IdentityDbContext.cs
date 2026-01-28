using App.BaseData;
using Identity.Data.Configurations;
using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data;

public class IdentityDbContext : DbContext
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    #region DbSets (do zapisu)

    public DbSet<AppUser> Employees => Set<AppUser>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<StructureDict> StructureDict => Set<StructureDict>();
    public DbSet<LoginAuditLog> LoginAuditLogs => Set<LoginAuditLog>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    // Roles and Permissions
    public DbSet<AppUserRole> AppUserRoles => Set<AppUserRole>();
    public DbSet<AppPermissionEntity> AppPermissions => Set<AppPermissionEntity>();
    public DbSet<AppUserRoleAssignment> AppUserRoleAssignments => Set<AppUserRoleAssignment>();
    public DbSet<AppUserPermissionAssignment> AppUserPermissionAssignments => Set<AppUserPermissionAssignment>();

    #endregion

    #region Agregaty (do odczytu z Include)

    /// <summary>
    /// AppUser z rolami i przełożonym - używany przy autoryzacji i zarządzaniu użytkownikami
    /// </summary>
    public IQueryable<AppUser> EmployeesWithRoles => Set<AppUser>()
        .Include(x => x.RoleAssignments)
            .ThenInclude(ra => ra.Role)
        .Include(x => x.Supervisor);

    /// <summary>
    /// AppUser z organizacją (Structure, Team) - używany przy wyświetlaniu kontekstu organizacyjnego
    /// </summary>
    public IQueryable<AppUser> EmployeesWithOrg => Set<AppUser>()
        .Include(x => x.Structure)
        .Include(x => x.Team);

    /// <summary>
    /// Team z Structure - Team zawsze potrzebuje informacji o Structure
    /// </summary>
    public IQueryable<Team> TeamsWithStructure => Set<Team>()
        .Include(x => x.Structure);

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new StructureDictConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new AppUserConfiguration());
        modelBuilder.ApplyConfiguration(new LoginAuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());

        // Roles and Permissions
        modelBuilder.ApplyConfiguration(new AppUserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new AppPermissionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AppUserRoleAssignmentConfiguration());
        modelBuilder.ApplyConfiguration(new AppUserPermissionAssignmentConfiguration());
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseAuditableTable>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAt = DateTime.UtcNow;
            }
        }
    }
}
