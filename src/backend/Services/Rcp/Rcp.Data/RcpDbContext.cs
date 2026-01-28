using Microsoft.EntityFrameworkCore;

using Rcp.Data.Entities;

namespace Rcp.Data;

public class RcpDbContext : DbContext
{
    

    
    public RcpDbContext(DbContextOptions<RcpDbContext> options) : base(options)
    {
    }
    
    // RCP (Time Tracking)
    public DbSet<RcpPeriod> RcpPeriods => Set<RcpPeriod>();
    public DbSet<RcpMonthlyEntry> RcpMonthlyEntries => Set<RcpMonthlyEntry>();
    public DbSet<RcpDayEntry> RcpDayEntries => Set<RcpDayEntry>();
    public DbSet<RcpEntryComment> RcpEntryComments => Set<RcpEntryComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // RCP (Time Tracking)
        modelBuilder.ApplyConfiguration(new Configurations.RcpPeriodConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.RcpMonthlyEntryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.RcpDayEntryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.RcpEntryCommentConfiguration());
    }
}