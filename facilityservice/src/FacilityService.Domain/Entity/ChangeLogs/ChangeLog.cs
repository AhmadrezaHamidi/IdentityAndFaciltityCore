namespace FacilityService.Domain.Entity.ChangeLogs;

/// <summary>گزارش تغییرات (US-A4): هر ثبت، ویرایش، لغو، حذف و تصمیم با کاربر، زمان و مقدار قبلی/جدید.</summary>
public class ChangeLog
{
    public long Id { get; private set; }
    public DateTime At { get; private set; }
    public int UserId { get; private set; }
    public Registrar User { get; private set; } = null!;
    public ChangeLogCategory Category { get; private set; }
    public string Action { get; private set; } = null!;
    public string Details { get; private set; } = null!;
    public string? Before { get; private set; }
    public string? After { get; private set; }
    public long? CompanyId { get; private set; }
    public Company? Company { get; private set; }
    public string? BranchCode { get; private set; }

    /// <summary>نوع عملیات قابل بازگردانی (⚠ Q16): balances، personnel-delete. null = غیرقابل بازگردانی.</summary>
    public string? RestoreKind { get; private set; }
    public string? RestorePayload { get; private set; }
    public DateTime? RestoredAt { get; private set; }

    private ChangeLog() { }

    public ChangeLog(DateTime at, Registrar user, ChangeLogCategory category, string action, string details,
        string? before, string? after, Company? company, string? restoreKind = null, string? restorePayload = null)
    {
        At = at;
        UserId = user.PanelUserId;
        User = user;
        Category = category;
        Action = action;
        Details = details.Length > 2000 ? details[..2000] : details;
        Before = before;
        After = after;
        Company = company;
        BranchCode = company?.BranchCode;
        RestoreKind = restoreKind;
        RestorePayload = restorePayload;
    }

    public bool CanRestore => RestoreKind != null && RestoredAt == null;

    public void MarkRestored(DateTime at)
    {
        if (!CanRestore) throw new FacilityDomainException("این عملیات قابل بازگردانی نیست.");
        RestoredAt = at;
    }
}
