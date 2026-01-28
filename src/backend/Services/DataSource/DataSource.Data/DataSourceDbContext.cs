using DataSource.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataSource.Data;

public class DataSourceDbContext : DbContext
{
    public DataSourceDbContext(DbContextOptions<DataSourceDbContext> options)
        : base(options)
    {
    }

    // Views as DbSet (read-only)
    public DbSet<VwCountry> Countries => Set<VwCountry>();
    public DbSet<VwProvince> Provinces => Set<VwProvince>();
    public DbSet<VwClient> Clients => Set<VwClient>();
    public DbSet<VwUserSales> UsersSales => Set<VwUserSales>();
    public DbSet<VwUsersAll> UsersAll => Set<VwUsersAll>();
    public DbSet<VwSbu> Sbu => Set<VwSbu>();
    public DbSet<VwTeam> Teams => Set<VwTeam>();
    public DbSet<VwBenefit> Benefits => Set<VwBenefit>();
    public DbSet<VwCertificate> Certificates => Set<VwCertificate>();
    public DbSet<VwTool> Tools => Set<VwTool>();
    public DbSet<VwTrait> Traits => Set<VwTrait>();
    public DbSet<VwOccupation> OccupationCodes => Set<VwOccupation>();
    public DbSet<VwPosition> Positions => Set<VwPosition>();
    public DbSet<VwUserSubordinate> UserSubordinates => Set<VwUserSubordinate>();

    // Read-only IQueryable (AsNoTracking) - use for queries
    public IQueryable<VwUserSubordinate> UserSubordinatesQuery => Set<VwUserSubordinate>().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure views - ToView() instead of ToTable()
        modelBuilder.Entity<VwCountry>(e =>
        {
            e.ToView("vw_countries");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwProvince>(e =>
        {
            e.ToView("vw_provinces");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwClient>(e =>
        {
            e.ToView("vw_clients");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwUserSales>(e =>
        {
            e.ToView("vw_users_sales");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwUsersAll>(e =>
        {
            e.ToView("vw_users_all");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwSbu>(e =>
        {
            e.ToView("vw_sbu");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwTeam>(e =>
        {
            e.ToView("vw_teams");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwBenefit>(e =>
        {
            e.ToView("vw_benefits");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwCertificate>(e =>
        {
            e.ToView("vw_certificates");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwTool>(e =>
        {
            e.ToView("vw_tools");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwTrait>(e =>
        {
            e.ToView("vw_traits");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwOccupation>(e =>
        {
            e.ToView("vw_occupation_codes");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwPosition>(e =>
        {
            e.ToView("vw_positions");
            e.HasKey(x => x.RecordId);
        });

        modelBuilder.Entity<VwUserSubordinate>(e =>
        {
            e.ToView("vw_user_subordinates");
            e.HasKey(x => x.RecordId);
        });
    }
}
