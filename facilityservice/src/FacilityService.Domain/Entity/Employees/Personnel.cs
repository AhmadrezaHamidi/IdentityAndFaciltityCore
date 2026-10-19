using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Employees;

/// <summary>پرسنل معرفی‌شدهٔ شرکت. جریان: در انتظار تأیید مدیر شرکت ← تأیید/رد؛ ردشده با «اصلاح و ارسال دوباره» برمی‌گردد.</summary>
public class Personnel : VersionedEntity
{
    public long Id { get; private set; }
    public long CompanyId { get; private set; }
    public int Priority { get; private set; }
    public string FullName { get; private set; } = null!;
    public string NationalCode { get; private set; } = null!;
    public string Mobile { get; private set; } = null!;
    public string PersonnelCode { get; private set; } = null!;
    public DateOnly HireDate { get; private set; }
    public PersonnelStatus Status { get; private set; } = PersonnelStatus.PendingManager;
    public string? RejectReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DecidedAt { get; private set; }
    public Registrar Registrar { get; private set; } = null!;
    public long? ImportBatchId { get; private set; }
    public string? DeleteDescription { get; private set; }
    public long? DeleteFileId { get; private set; }
    public StoredFile? DeleteFile { get; private set; }
    public List<PersonnelHistory> History { get; private set; } = [];

    private Personnel() { }

    /// <param name="data">قبلاً با <see cref="PersonnelData.Normalize"/> اعتبارسنجی شده</param>
    public static Personnel Create(long companyId, PersonnelData data, Registrar by, DateTime now, long? importBatchId = null)
    {
        var p = new Personnel { CompanyId = companyId, Registrar = by, CreatedAt = now, ImportBatchId = importBatchId };
        p.Set(data);
        p.History.Add(new PersonnelHistory(null, PersonnelStatus.PendingManager, null, null, by, now));
        return p;
    }

    /// <summary>ویرایش؛ برای فرد ردشده «اصلاح و ارسال دوباره» (← در انتظار تأیید مدیر). فرد تأییدشده یا قفل‌شده قابل ویرایش نیست.</summary>
    public void Edit(PersonnelData data, bool isLocked, Registrar by, DateTime now)
    {
        EnsureNotDeleted();
        if (isLocked) throw new FacilityDomainException("این فرد در درخواست وام استفاده شده و ویرایش او ممکن نیست.");
        if (Status == PersonnelStatus.Approved) throw new FacilityDomainException("فرد تأییدشده قابل ویرایش نیست.");
        Set(data);
        if (Status == PersonnelStatus.Rejected)
        {
            History.Add(new PersonnelHistory(PersonnelStatus.Rejected, PersonnelStatus.PendingManager, null, "اصلاح و ارسال دوباره", by, now));
            Status = PersonnelStatus.PendingManager;
            RejectReason = null;
            DecidedAt = null;
        }
        Bump();
    }

    /// <summary>US-D1: تأیید یا رد مدیر شرکت. فقط از «در انتظار».</summary>
    public bool TryDecide(bool approve, string? reasonTitle, string? description, Registrar by, DateTime now)
    {
        if (Status != PersonnelStatus.PendingManager) return false;
        var to = approve ? PersonnelStatus.Approved : PersonnelStatus.Rejected;
        History.Add(new PersonnelHistory(Status, to, reasonTitle, description, by, now));
        Status = to;
        DecidedAt = now;
        RejectReason = approve ? null : reasonTitle ?? description;
        Bump();
        return true;
    }

    public PersonnelStatus Delete(string description, StoredFile file, bool isLocked, Registrar by, DateTime now)
    {
        EnsureNotDeleted();
        if (isLocked) throw new FacilityDomainException("این فرد در درخواست وام استفاده شده و حذف او ممکن نیست.");
        description = (description ?? "").Trim();
        if (description.Length is < 5 or > 500) throw new FacilityDomainException("درج توضیحات حذف (۵ تا ۵۰۰ حرف) الزامی است.");
        var before = Status;
        History.Add(new PersonnelHistory(Status, PersonnelStatus.Deleted, null, description, by, now));
        Status = PersonnelStatus.Deleted;
        DeleteDescription = description;
        DeleteFile = file;
        Bump();
        return before;
    }

    /// <summary>بازگردانی حذف از گزارش تغییرات.</summary>
    public void Undelete(PersonnelStatus status, Registrar by, DateTime now)
    {
        if (Status != PersonnelStatus.Deleted) throw new FacilityDomainException("این فرد حذف‌شده نیست.");
        History.Add(new PersonnelHistory(PersonnelStatus.Deleted, status, null, "بازگردانی", by, now));
        Status = status;
        DeleteDescription = null;
        Bump();
    }

    private void Set(PersonnelData d)
    {
        FullName = d.FullName;
        NationalCode = d.NationalCode;
        Mobile = d.Mobile;
        PersonnelCode = d.PersonnelCode;
        HireDate = d.HireDate;
        Priority = d.Priority;
    }

    private void EnsureNotDeleted()
    {
        if (Status == PersonnelStatus.Deleted) throw FacilityDomainException.NotFound("پرسنل یافت نشد.");
    }
}
