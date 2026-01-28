using Identity.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Data.Configurations;

public class StructureDictConfiguration : IEntityTypeConfiguration<StructureDict>
{
    public void Configure(EntityTypeBuilder<StructureDict> builder)
    {
        builder.ToTable("structure_dict");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id");

        builder.Property(e => e.Code)
            .HasColumnName("code")
            .HasMaxLength(25)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Region)
            .HasColumnName("region")
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.HasIndex(e => e.Code)
            .IsUnique();

        builder.HasIndex(e => e.Name)
            .IsUnique();
    }
}
