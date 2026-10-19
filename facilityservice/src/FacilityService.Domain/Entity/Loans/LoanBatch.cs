namespace FacilityService.Domain.Entity.Loans;

/// <summary>
/// یک «ثبت» تخصیص وام: چند درخواست هم‌طرح که با هم ثبت شدند (US-D3). هر درخواست به‌ازای یک نفر است.
/// جریان هر درخواست: در انتظار مدیر شرکت ← در انتظار اعتبارات ← [در انتظار شعبهٔ دیجیتال] ← تأییدشده ← مرحلهٔ پس از تأیید.
/// </summary>
public class LoanBatch
{
    public long Id { get; private set; }
    public long CompanyId { get; private set; }
    public int PlanId { get; private set; }
    public AllocationMode Mode { get; private set; }
    public DateOnly BaseDate { get; private set; }
    public string? Description { get; private set; }
    public Registrar Registrar { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public List<LoanRequest> Requests { get; private set; } = [];

    private LoanBatch() { }

    public LoanBatch(long companyId, string branchCode, int planId, AllocationMode mode, DateOnly baseDate, string? description, Registrar by, DateTime at)
    {
        CompanyId = companyId;
        BranchCode = branchCode;
        PlanId = planId;
        Mode = mode;
        BaseDate = baseDate;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Registrar = by;
        CreatedAt = at;
    }

    public string BranchCode { get; private set; } = null!;

    public LoanRequest Add(long personnelId, long amount, int paymentOrder, Registrar by, DateTime at)
    {
        var r = LoanRequest.Create(this, personnelId, amount, paymentOrder, by, at);
        Requests.Add(r);
        return r;
    }

    /// <summary>US-D4: افزودن فرد فقط تا وقتی که درخواستی از این ثبت هنوز به تأیید اعتبارات نرسیده.</summary>
    public bool AcceptsNewMembers => Requests.Any(r => r.Status is LoanRequestStatus.PendingManager or LoanRequestStatus.PendingCredit);
}
