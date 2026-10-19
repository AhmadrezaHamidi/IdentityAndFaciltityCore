namespace FacilityService.Domain.Entity.BasicInfo;

/// <summary>فهرست‌های داینامیک اطلاعات پایه (دلایل لغو، رد، نوع مستند، ...).</summary>
public class Reason
{
    public int Id { get; private set; }
    public ReasonType Type { get; private set; }
    public string Title { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; }

    private Reason() { }

    public Reason(ReasonType type, string title, int sortOrder)
    {
        Type = type;
        Rename(title);
        SortOrder = sortOrder;
    }

    public void Rename(string title)
    {
        title = (title ?? "").Trim();
        if (title.Length is < 3 or > 100) throw new FacilityDomainException("عنوان دلیل باید ۳ تا ۱۰۰ حرف باشد.");
        Title = title;
    }

    public void Update(string title, bool isActive, int sortOrder)
    {
        Rename(title);
        IsActive = isActive;
        SortOrder = sortOrder;
    }

    /// <summary>حذف نرم: رکوردهای قدیمی که این دلیل را دارند سالم می‌مانند.</summary>
    public void Deactivate() => IsActive = false;
}
