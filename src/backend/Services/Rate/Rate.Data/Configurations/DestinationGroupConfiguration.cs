using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RateService.Data.Entities;

namespace RateService.Data.Configurations;

public class DestinationGroupConfiguration : IEntityTypeConfiguration<DestinationGroup>
{
    public void Configure(EntityTypeBuilder<DestinationGroup> builder)
    {
        builder.ToTable("destination_groups");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        builder.Property(x => x.NamePL).HasColumnName("name_pl").HasMaxLength(50).IsRequired();
        builder.Property(x => x.NameEN).HasColumnName("name_en").HasMaxLength(50);
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(200);
        builder.Property(x => x.IsActive).HasColumnName("is_active");

        // Indexes
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
