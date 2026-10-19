using System.Linq;

namespace IdentityService.Domain.Entity.ContentManagement;

/// <summary>
/// «مدیریت محتوا» در Figma چهار زیرمنو دارد که هرکدام لیست + افزودن + ویرایش
/// دارند: مدیریت بنرها، مدیریت صفحات محتوایی (CMS)، مدیریت سوالات متداول (FAQ)،
/// مدیریت متن‌های اطلاع‌رسانی. طبق همان الگوی <c>RoleEntity.Kind</c> و
/// <c>BasicInfoItemEntity.Category</c>، یک جدول با ستون تفکیک‌کننده.
/// </summary>
public sealed class ContentItemEntity
{
    public int Id { get; set; }

    public string Category { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Body { get; set; }

    /// <summary>فقط برای بنرها معنا دارد؛ برای بقیهٔ دسته‌ها null می‌ماند.</summary>
    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public int? CreatedByPanelUserId { get; set; }
}

public static class ContentCategory
{
    public const string Banner = "Banner";
    public const string ContentPage = "ContentPage";
    public const string Faq = "Faq";
    public const string InfoText = "InfoText";

    public static readonly string[] All = [Banner, ContentPage, Faq, InfoText];

    public static bool Exists(string category) => All.Contains(category);
}
