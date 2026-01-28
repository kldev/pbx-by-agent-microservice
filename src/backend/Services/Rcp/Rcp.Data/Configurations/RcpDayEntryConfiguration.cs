using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rcp.Data.Entities;

namespace Rcp.Data.Configurations;

public class RcpDayEntryConfiguration : IEntityTypeConfiguration<RcpDayEntry>
{
    public void Configure(EntityTypeBuilder<RcpDayEntry> builder)
    {
        builder.ToTable("rcp_day_entries");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Gid).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(500);

        builder.HasIndex(x => x.Gid).IsUnique();

        // Unique constraint: jeden wpis per dzień per miesięczny wpis
        builder.HasIndex(x => new { x.MonthlyEntryId, x.WorkDate })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        // Relacja do MonthlyEntry
        builder.HasOne(x => x.MonthlyEntry)
            .WithMany(m => m.DayEntries)
            .HasForeignKey(x => x.MonthlyEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
