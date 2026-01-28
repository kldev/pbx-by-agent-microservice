using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateService.Data.Entities;

namespace RateService.Data.Configurations;

public class RateConfiguration : IEntityTypeConfiguration<Rate>
{
    public void Configure(EntityTypeBuilder<Rate> builder)
    {
        builder.ToTable("rates");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Gid).HasColumnName("gid").HasMaxLength(36);

        builder.Property(x => x.Prefix).HasColumnName("prefix").HasMaxLength(15).IsRequired();
        builder.Property(x => x.DestinationName).HasColumnName("destination_name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.RatePerMinute).HasColumnName("rate_per_minute").HasPrecision(10, 6);
        builder.Property(x => x.ConnectionFee).HasColumnName("connection_fee").HasPrecision(10, 4);
        builder.Property(x => x.BillingIncrement).HasColumnName("billing_increment");
        builder.Property(x => x.MinimumDuration).HasColumnName("minimum_duration");
        builder.Property(x => x.EffectiveFrom).HasColumnName("effective_from");
        builder.Property(x => x.EffectiveTo).HasColumnName("effective_to");
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);

        // FK
        builder.Property(x => x.TariffId).HasColumnName("tariff_id");
        builder.HasOne(x => x.Tariff)
            .WithMany(t => t.Rates)
            .HasForeignKey(x => x.TariffId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.DestinationGroupId).HasColumnName("destination_group_id");
        builder.HasOne(x => x.DestinationGroup)
            .WithMany(g => g.Rates)
            .HasForeignKey(x => x.DestinationGroupId)
            .OnDelete(DeleteBehavior.SetNull);

        // Audit fields
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(x => x.ModifiedAt).HasColumnName("modified_at");
        builder.Property(x => x.ModifiedByUserId).HasColumnName("modified_by_user_id");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Indexes
        builder.HasIndex(x => x.Gid).IsUnique();
        builder.HasIndex(x => x.Prefix);
        builder.HasIndex(x => new { x.TariffId, x.Prefix });
    }
}
