using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateService.Data.Entities;

namespace RateService.Data.Configurations;

public class TariffConfiguration : IEntityTypeConfiguration<Tariff>
{
    public void Configure(EntityTypeBuilder<Tariff> builder)
    {
        builder.ToTable("tariffs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Gid).HasColumnName("gid").HasMaxLength(36);

        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3).IsRequired();
        builder.Property(x => x.IsDefault).HasColumnName("is_default");
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.ValidFrom).HasColumnName("valid_from");
        builder.Property(x => x.ValidTo).HasColumnName("valid_to");
        builder.Property(x => x.BillingIncrement).HasColumnName("billing_increment");
        builder.Property(x => x.MinimumDuration).HasColumnName("minimum_duration");
        builder.Property(x => x.ConnectionFee).HasColumnName("connection_fee").HasPrecision(10, 4);

        // Audit fields
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(x => x.ModifiedAt).HasColumnName("modified_at");
        builder.Property(x => x.ModifiedByUserId).HasColumnName("modified_by_user_id");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        // Indexes
        builder.HasIndex(x => x.Gid).IsUnique();
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.IsDefault);
    }
}
