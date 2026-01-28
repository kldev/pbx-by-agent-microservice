using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CdrService.Data.Entities;

namespace CdrService.Data.Configurations;

public class CallStatusConfiguration : IEntityTypeConfiguration<CallStatus>
{
    public void Configure(EntityTypeBuilder<CallStatus> builder)
    {
        builder.ToTable("call_statuses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Gid).HasColumnName("gid").HasMaxLength(36);

        builder.Property(x => x.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.Property(x => x.NamePL).HasColumnName("name_pl").HasMaxLength(100).IsRequired();
        builder.Property(x => x.NameEN).HasColumnName("name_en").HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(x => x.SortOrder).HasColumnName("sort_order");
        builder.Property(x => x.IsActive).HasColumnName("is_active");

        builder.HasIndex(x => x.Gid).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
