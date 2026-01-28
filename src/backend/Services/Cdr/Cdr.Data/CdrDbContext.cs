using Microsoft.EntityFrameworkCore;
using CdrService.Data.Entities;

namespace CdrService.Data;

public class CdrDbContext : DbContext
{
    public DbSet<CallRecord> CallRecords => Set<CallRecord>();
    public DbSet<CallStatus> CallStatuses => Set<CallStatus>();
    public DbSet<TerminationCause> TerminationCauses => Set<TerminationCause>();
    public DbSet<CallDirection> CallDirections => Set<CallDirection>();

    // ReSharper disable once ConvertToPrimaryConstructor
    public CdrDbContext(DbContextOptions<CdrDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CdrDbContext).Assembly);
    }
}
