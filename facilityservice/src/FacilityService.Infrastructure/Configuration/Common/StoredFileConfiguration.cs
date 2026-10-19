using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.Common;

public sealed class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> e)
    {
        e.ToTable("Files", Schema);
        e.Property(x => x.FileName).HasMaxLength(260).IsRequired();
        e.Property(x => x.ContentType).HasMaxLength(150).IsRequired();
        e.Property(x => x.StorageKey).HasMaxLength(300).IsRequired();
        e.Property(x => x.UploadedBy).HasMaxLength(150).IsRequired();
    }
}
