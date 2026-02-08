using FinCosts.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinCosts.Data.Configurations;

public class CostDocumentTypeDictConfiguration : IEntityTypeConfiguration<CostDocumentTypeDict>
{
    public void Configure(EntityTypeBuilder<CostDocumentTypeDict> builder)
    {
        builder.ToTable("cost_document_type");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.Category).HasColumnName("category").HasConversion<string>().HasMaxLength(120);
        
        builder.Property(x => x.NamePL).HasColumnName("name_pl").HasConversion<string>().HasMaxLength(120);
        builder.Property(x => x.NameEN).HasColumnName("name_en").HasConversion<string>().HasMaxLength(120);

        builder.HasIndex(x => x.NamePL).IsUnique();
        builder.HasIndex(x => x.NameEN).IsUnique();
    }
}