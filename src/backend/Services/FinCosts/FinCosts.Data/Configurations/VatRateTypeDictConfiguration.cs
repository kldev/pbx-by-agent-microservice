using FinCosts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinCosts.Data.Configurations;

public class VatRateTypeDictConfiguration : IEntityTypeConfiguration<VatRateTypeDict>
{
    public void Configure(EntityTypeBuilder<VatRateTypeDict> builder)
    {
        builder.ToTable("vat_rate_type");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x=>x.VatRate).HasColumnName("vat_rate").IsRequired();

        builder.Property(x => x.NamePL).HasColumnName("name_pl").HasConversion<string>().HasMaxLength(120);
        builder.Property(x => x.NameEN).HasColumnName("name_en").HasConversion<string>().HasMaxLength(120);

        builder.HasIndex(x => x.NamePL).IsUnique();
        builder.HasIndex(x => x.NameEN).IsUnique();
        builder.HasIndex(x => x.VatRate);
    }
}