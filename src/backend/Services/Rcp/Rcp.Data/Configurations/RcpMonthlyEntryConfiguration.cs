using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rcp.Data.Entities;

namespace Rcp.Data.Configurations;

public class RcpMonthlyEntryConfiguration : IEntityTypeConfiguration<RcpMonthlyEntry>
{
    public void Configure(EntityTypeBuilder<RcpMonthlyEntry> builder)
    {
        builder.ToTable("rcp_monthly_entries");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Gid).HasMaxLength(36).IsRequired();
        builder.Property(x => x.UserGid).HasMaxLength(36);
        builder.Property(x => x.EmployeeFullName).HasMaxLength(200);
        builder.Property(x => x.StatusChangedByFullName).HasMaxLength(200);
        builder.Property(x => x.ApprovedByFullName).HasMaxLength(200);

        builder.HasIndex(x => x.Gid).IsUnique();

        // Unique constraint: jeden wpis per użytkownik per okres
        builder.HasIndex(x => new { x.RcpPeriodId, x.UserId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Relacja do RcpPeriod
        builder.HasOne(x => x.RcpPeriod)
            .WithMany(p => p.MonthlyEntries)
            .HasForeignKey(x => x.RcpPeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
