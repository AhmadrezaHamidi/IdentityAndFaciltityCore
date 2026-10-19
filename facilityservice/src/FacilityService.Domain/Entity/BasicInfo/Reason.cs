namespace FacilityService.Domain.Entity.BasicInfo;

/// <summary>
/// فهرست‌های داینامیک اطلاعات پایه (دلایل لغو، رد، نوع مستند، ...).
///
/// هر دلیل به یک طرح تعلق دارد: سند فرانت فهرست‌های ۱۱ تا ۱۴ را بخشی از فرم طرح می‌داند و
/// هر طرح می‌تواند تعداد متفاوتی داشته باشد. قبلاً یک فهرست سراسری بود، یعنی دلیل لغو
/// «نگین میثاق» در کشویی «شجره طیبه» هم ظاهر می‌شد.
/// </summary>
public class Reason
{
    public int Id { get; private set; }

    /// <summary>طرحی که این دلیل به آن تعلق دارد.</summary>
    public int SchemeId { get; private set; }

    public ReasonType Type { get; private set; }
    public string Title { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;
    public int SortOrder { get; private set; }

    private Reason() { }

    public Reason(int schemeId, ReasonType type, string title, int sortOrder)
    {
        SchemeId = schemeId;
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
