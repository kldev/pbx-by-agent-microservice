using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rcp.Data.Entities;

namespace Rcp.Data.Configurations;

public class RcpEntryCommentConfiguration : IEntityTypeConfiguration<RcpEntryComment>
{
    public void Configure(EntityTypeBuilder<RcpEntryComment> builder)
    {
        builder.ToTable("rcp_entry_comments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Gid).HasMaxLength(36).IsRequired();
        builder.Property(x => x.Content).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.AuthorName).HasMaxLength(200);
        builder.Property(x => x.AuthorRole).HasMaxLength(50);

        builder.HasIndex(x => x.Gid).IsUnique();

        // Relacja do MonthlyEntry
        builder.HasOne(x => x.MonthlyEntry)
            .WithMany(m => m.Comments)
            .HasForeignKey(x => x.MonthlyEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
