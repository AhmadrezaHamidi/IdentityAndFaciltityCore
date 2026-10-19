using IdentityService.Domain.Entity.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Infrastructure.Configuration
{
    public class ContentItemEntityConfiguration : IEntityTypeConfiguration<ContentItemEntity>
    {
        public void Configure(EntityTypeBuilder<ContentItemEntity> builder)
        {
            builder.ToTable("ContentItem");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Category).HasMaxLength(30).IsUnicode(false).IsRequired();
            builder.Property(e => e.Title).HasMaxLength(300).IsRequired();
            builder.Property(e => e.Body).HasMaxLength(4000);
            builder.Property(e => e.ImageUrl).HasMaxLength(500).IsUnicode(false);
            builder.Property(e => e.CreatedAtUtc).HasColumnType("datetime2");
            builder.Property(e => e.UpdatedAtUtc).HasColumnType("datetime2");

            builder.HasIndex(e => new { e.Category, e.DisplayOrder }, "IX_ContentItem_Category_DisplayOrder");
        }
    }
}
