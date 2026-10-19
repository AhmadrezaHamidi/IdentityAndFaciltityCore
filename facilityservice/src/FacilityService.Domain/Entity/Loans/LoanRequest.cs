namespace FacilityService.Domain.Entity.Loans;

public class LoanRequest : VersionedEntity
{
    public const long MinAmount = 10_000_000;
    public const long MaxAmount = 4_000_000_000;

    public long Id { get; private set; }
    public long BatchId { get; private set; }
    public long CompanyId { get; private set; }

    /// <summary>کد شعبهٔ شرکت (کپی؛ شعبهٔ شرکت عوض نمی‌شود) تا فیلتر دامنهٔ شعبه بدون Join روی لیست‌ها اعمال شود.</summary>
    public string BranchCode { get; private set; } = null!;

    public long PersonnelId { get; private set; }
    public int PlanId { get; private set; }
    public long Amount { get; private set; }
    public int PaymentOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DecidedAt { get; private set; }
    public LoanRequestStatus Status { get; private set; } = LoanRequestStatus.PendingManager;
    public PostApprovalStage? PostStage { get; private set; }
    public string? Description { get; private set; }
    public Registrar Registrar { get; private set; } = null!;
    public List<LoanRequestHistory> History { get; private set; } = [];
    public List<LoanRequestDocument> Documents { get; private set; } = [];

    private LoanRequest() { }

    internal static LoanRequest Create(LoanBatch batch, long personnelId, long amount, int paymentOrder, Registrar by, DateTime at)
    {
        if (amount is < MinAmount or > MaxAmount)
            throw new FacilityDomainException("مبلغ وام باید بین ۱۰٬۰۰۰٬۰۰۰ و ۴٬۰۰۰٬۰۰۰٬۰۰۰ ریال باشد.");
        if (paymentOrder is < 1 or > 9_999) throw new FacilityDomainException("ترتیب پرداخت باید بین ۱ تا ۹۹۹۹ باشد.");
        var r = new LoanRequest
        {
            CompanyId = batch.CompanyId,
            BranchCode = batch.BranchCode,
            PersonnelId = personnelId,
            PlanId = batch.PlanId,
            Amount = amount,
            PaymentOrder = paymentOrder,
            CreatedAt = at,
            Registrar = by,
            Description = batch.Description,
        };
        r.History.Add(new LoanRequestHistory(null, LoanRequestStatus.PendingManager, null, null, null, by, at));
        return r;
    }

    /// <summary>درخواست باز (در جریان تأیید) — برای یک نفر دو درخواست باز مجاز نیست.</summary>
    public static readonly LoanRequestStatus[] OpenStatuses =
        [LoanRequestStatus.PendingManager, LoanRequestStatus.PendingCredit, LoanRequestStatus.PendingDigital];

    /// <summary>وضعیت‌هایی که مبلغشان از اعتبار طرح کم نمی‌شود (ردشده و لغوشده).</summary>
    public static readonly LoanRequestStatus[] ReleasedStatuses =
        [LoanRequestStatus.RejectedByManager, LoanRequestStatus.RejectedByCredit, LoanRequestStatus.RejectedByDigital, LoanRequestStatus.Cancelled];

    public static LoanRequestStatus PendingStatusOf(ApprovalStage stage) => stage switch
    {
        ApprovalStage.Manager => LoanRequestStatus.PendingManager,
        ApprovalStage.Credit => LoanRequestStatus.PendingCredit,
        _ => LoanRequestStatus.PendingDigital,
    };

    /// <summary>
    /// تصمیم یک مرحله (قاعدهٔ فرایندی ۵: تاریخ تصمیم، تصمیم‌گیرنده، وضعیت قبلی و جدید در تاریخچه).
    /// false یعنی درخواست در این مرحله نیست (در تصمیم گروهی Skipped می‌شود).
    /// </summary>
    public bool TryDecide(ApprovalStage stage, bool approve, bool digitalApprovalEnabled, string? reasonTitle, string? description,
        Registrar by, DateTime at)
    {
        if (Status != PendingStatusOf(stage)) return false;
        var to = (stage, approve) switch
        {
            (ApprovalStage.Manager, true) => LoanRequestStatus.PendingCredit,
            (ApprovalStage.Manager, false) => LoanRequestStatus.RejectedByManager,
            (ApprovalStage.Credit, true) => digitalApprovalEnabled ? LoanRequestStatus.PendingDigital : LoanRequestStatus.Approved,
            (ApprovalStage.Credit, false) => LoanRequestStatus.RejectedByCredit,
            (_, true) => LoanRequestStatus.Approved,
            _ => LoanRequestStatus.RejectedByDigital,
        };
        Transition(to, null, reasonTitle, description, by, at);
        if (!approve) Description = string.Join(" — ", new[] { reasonTitle, description }.Where(s => !string.IsNullOrWhiteSpace(s)));
        return true;
    }

    /// <summary>US-D5: لغو تا قبل از تصمیم تأیید اعتبارات؛ دلیل از فهرست (قاعدهٔ ۸) + توضیح.</summary>
    public void Cancel(Reason reason, string description, Registrar by, DateTime at)
    {
        if (Status is not (LoanRequestStatus.PendingManager or LoanRequestStatus.PendingCredit))
            throw new FacilityDomainException("پس از تصمیم تأیید اعتبارات، لغو درخواست ممکن نیست.");
        if (reason.Type != ReasonType.LoanCancel || !reason.IsActive) throw new FacilityDomainException("انتخاب دلیل لغو الزامی است.");
        description = (description ?? "").Trim();
        if (description.Length is < 5 or > 1000) throw new FacilityDomainException("درج توضیحات لغو (۵ تا ۱۰۰۰ حرف) الزامی است.");
        Transition(LoanRequestStatus.Cancelled, null, reason.Title, description, by, at);
        Description = $"{reason.Title} — {description}";
    }

    /// <summary>US-D4: حذف فرد از ثبت «قبل از تأیید اعتبارات» (در انتظار مدیر شرکت یا در انتظار اعتبارات).</summary>
    public void RemoveFromBatch(Registrar by, DateTime at)
    {
        if (Status is not (LoanRequestStatus.PendingManager or LoanRequestStatus.PendingCredit))
            throw new FacilityDomainException("پس از تصمیم تأیید اعتبارات، حذف فرد از درخواست ممکن نیست.");
        Transition(LoanRequestStatus.Cancelled, null, null, "حذف از درخواست", by, at);
    }

    /// <summary>مرحلهٔ پس از تأیید (نمونهٔ پنل). «عدم اعطای غیرحضوری» دلیل لازم دارد.</summary>
    public void SetPostStage(PostApprovalStage stage, Reason? reason, string? description, Registrar by, DateTime at)
    {
        if (Status != LoanRequestStatus.Approved) throw new FacilityDomainException("مرحلهٔ پس از تأیید فقط برای درخواست تأییدشده است.");
        if (!Enum.IsDefined(stage)) throw new FacilityDomainException("مرحله نامعتبر است.");
        if (stage == PostApprovalStage.RemoteGrantDenied && (reason is null || reason.Type != ReasonType.RemoteGrantDenied))
            throw new FacilityDomainException("برای «عدم اعطای تسهیلات غیرحضوری» انتخاب دلیل الزامی است.");
        PostStage = stage;
        History.Add(new LoanRequestHistory(Status, Status, stage, reason?.Title, description, by, at));
        if (stage == PostApprovalStage.RemoteGrantDenied)
            Description = string.Join(" — ", new[] { reason!.Title, description }.Where(s => !string.IsNullOrWhiteSpace(s)));
        Bump();
    }

    public void AttachDocument(StoredFile file) => Documents.Add(new LoanRequestDocument(file));

    private void Transition(LoanRequestStatus to, PostApprovalStage? stage, string? reason, string? description, Registrar by, DateTime at)
    {
        History.Add(new LoanRequestHistory(Status, to, stage, reason, description, by, at));
        Status = to;
        DecidedAt = at;
        Bump();
    }
}
