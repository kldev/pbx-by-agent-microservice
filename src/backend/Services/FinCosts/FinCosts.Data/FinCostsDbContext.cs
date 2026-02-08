using App.BaseData;
using FinCosts.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinCosts.Data;

public class FinCostsDbContext  : DbContext
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public FinCostsDbContext(DbContextOptions<FinCostsDbContext> options) : base(options)
    {
    }
    
    public IQueryable<CostDocumentTypeDict> CostDocumentTypeReadOnly => Set<CostDocumentTypeDict>();
    public DbSet<CostDocumentTypeDict> CostDocumentTypeSeed => Set<CostDocumentTypeDict>();
    
    public IQueryable<CurrencyTypeDict> CurrencyTypeReadOnly => Set<CurrencyTypeDict>();
    public DbSet<CurrencyTypeDict> CurrencyTypeSeed => Set<CurrencyTypeDict>();

    public IQueryable<DocumentEntry> DocumentEntryReadOnly => Set<DocumentEntry>();
    public DbSet<DocumentEntry> DocumentEntrySeed => Set<DocumentEntry>();
    
    public IQueryable<VatRateTypeDict> VatRateTypeReadOnly => Set<VatRateTypeDict>();
    public DbSet<VatRateTypeDict> VatRateTypeSeed => Set<VatRateTypeDict>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinCostsDbContext).Assembly);
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