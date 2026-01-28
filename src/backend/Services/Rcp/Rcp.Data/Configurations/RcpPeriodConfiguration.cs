using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rcp.Data.Entities;

namespace Rcp.Data.Configurations;

public class RcpPeriodConfiguration : IEntityTypeConfiguration<RcpPeriod>
{
    public void Configure(EntityTypeBuilder<RcpPeriod> builder)
    {
        builder.ToTable("rcp_periods");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Gid).HasMaxLength(36).IsRequired();

        builder.HasIndex(x => x.Gid).IsUnique();

        // Unique constraint: jeden okres per (Year, Month)
        builder.HasIndex(x => new { x.Year, x.Month })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
