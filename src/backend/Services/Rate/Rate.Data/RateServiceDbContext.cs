using App.BaseData;
using Microsoft.EntityFrameworkCore;
using RateService.Data.Entities;

namespace RateService.Data;

public class RateServiceDbContext : DbContext
{
    public DbSet<Tariff> Tariffs => Set<Tariff>();
    public DbSet<Rate> Rates => Set<Rate>();
    public DbSet<DestinationGroup> DestinationGroups => Set<DestinationGroup>();

    
    // ReSharper disable once ConvertToPrimaryConstructor
    public RateServiceDbContext(DbContextOptions<RateServiceDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RateServiceDbContext).Assembly);
    }
}
