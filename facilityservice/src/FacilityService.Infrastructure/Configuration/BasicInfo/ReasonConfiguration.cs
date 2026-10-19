using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static FacilityService.Infrastructure.DBContext.FacilityPanelModel;

namespace FacilityService.Infrastructure.Configuration.BasicInfo;

public sealed class ReasonConfiguration : IEntityTypeConfiguration<Reason>
{
    public void Configure(EntityTypeBuilder<Reason> e)
    {
        e.ToTable("Reasons", Schema);
        e.Property(x => x.Title).HasMaxLength(100).IsRequired();
        // کشویی‌ها همیشه با (طرح، نوع) خوانده می‌شوند
        e.HasIndex(x => new { x.SchemeId, x.Type, x.IsActive, x.SortOrder });
        // عنوان تکراری در یک طرح و یک نوع پذیرفته نمی‌شود؛ غیرفعال‌ها بیرون‌اند تا
        // بتوان عنوانی را که قبلاً حذف نرم شده دوباره ساخت.
        e.HasIndex(x => new { x.SchemeId, x.Type, x.Title }).IsUnique().HasFilter("[IsActive] = 1");
    }
}
