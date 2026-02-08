using FinCosts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinCosts.Data.Configurations;

public class CurrencyTypeDictConfiguration : IEntityTypeConfiguration<CurrencyTypeDict>
{
    public void Configure(EntityTypeBuilder<CurrencyTypeDict> builder)
    {
        builder.ToTable("currency_type");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.Category).HasColumnName("category").HasConversion<string>().HasMaxLength(120);

        builder.Property(x => x.NamePL).HasColumnName("name_pl").HasConversion<string>().HasMaxLength(120);
        builder.Property(x => x.NameEN).HasColumnName("name_en").HasConversion<string>().HasMaxLength(120);

        builder.HasIndex(x => x.NamePL).IsUnique();
        builder.HasIndex(x => x.NameEN).IsUnique();
    }
}
