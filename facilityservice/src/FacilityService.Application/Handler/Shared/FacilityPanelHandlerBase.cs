using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Shared;

/// <summary>
/// پایهٔ Handlerهای پنل: کاربر جاری، زمان، بارگذاری منبع در دامنهٔ کاربر و گزارش تغییرات.
/// قاعدهٔ دامنه (معیار پذیرش ۴ و ۵): منبع خارج از دامنه «یافت نشد» برمی‌گرداند تا وجودش لو نرود.
/// </summary>
public abstract class FacilityPanelHandlerBase(IFacilityPanelDbContext db, IFacilityCurrentUser currentUser, TimeProvider time)
{
    protected IFacilityPanelDbContext Db { get; } = db;
    protected FacilityPanelUser User => currentUser.Value;
    protected PanelPermissions Permissions => User.Permissions;
    protected Registrar Me => User.ToRegistrar();
    protected DateTime Now => time.GetUtcNow().UtcDateTime;
    protected DateOnly Today => FacilityClock.Today(time);

    protected static FacilityBaseMessageResponse<T> Ok<T>(T data, string message = "عملیات با موفقیت انجام شد.") =>
        MessageResponse.GenerateMessageResponse(200, data, message);

    protected static FacilityDomainException Invalid(string message) => new(message);

    /// <summary>
    /// ادمین سامانه: طبق ماتریس سند، فقط ادمین «اطلاعات پایهٔ طرح» را با دامنهٔ سراسری ویرایش می‌کند.
    /// برای قواعدی که سند مخصوص ادمین تعریف کرده (مثل US-A6: توضیح و فایل اجباری در تصمیم).
    /// </summary>
    protected bool IsSystemAdmin => Permissions.ScopeFor(FacilityPermissionKeys.BasicInfoEdit).All;

    // ── مجوز روی منبع ──

    protected bool CanOn(string key, Company company) => Permissions.Can(key, Resource(company));

    protected static FacilityResource Resource(Company c) => FacilityResource.OfCompany(c.BranchCode, c.Id.ToString());

    protected bool CanOnAny(Company company, params string[] keys) => keys.Any(k => CanOn(k, company));

    /// <summary>اولین کلید ← 403 با پیام؛ برای عملیاتی که دیدن منبع مجاز است ولی تغییرش نه.</summary>
    protected void Require(string key, Company company, string? message = null)
    {
        if (!CanOn(key, company)) throw FacilityDomainException.Forbidden(message ?? FacilityAccess.ForbiddenMessage);
    }

    protected IQueryable<Company> CompaniesWithDetails() => Db.Companies
        .Include(c => c.Plans).Include(c => c.Balances).Include(c => c.Cancellation!).ThenInclude(x => x.LetterFile)
        .Include(c => c.AgreementFile);

    /// <summary>شرکت حذف‌نشده‌ای که حداقل یکی از کلیدها روی آن مجاز است؛ وگرنه 404.</summary>
    protected async Task<Company> CompanyInScopeAsync(long id, CancellationToken ct, params string[] keys)
    {
        var company = await CompaniesWithDetails().FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, ct);
        if (company == null || !CanOnAny(company, keys)) throw FacilityDomainException.NotFound("شرکت یافت نشد.");
        return company;
    }

    /// <summary>فیلتر دامنهٔ یک کلید روی شرکت‌ها (قبل از صفحه‌بندی).</summary>
    protected IQueryable<Company> ScopedCompanies(string key) =>
        Permissions.ScopeFor(key).Apply(Db.Companies.Where(c => c.DeletedAt == null), c => c.BranchCode, c => c.Id.ToString());

    /// <summary>فیلتر دامنه روی درخواست‌های وام.</summary>
    protected IQueryable<LoanRequest> ScopedLoans(string key) =>
        Permissions.ScopeFor(key).Apply(Db.LoanRequests.AsQueryable(), l => l.BranchCode, l => l.CompanyId.ToString());

    /// <summary>
    /// لیست درخواست‌های قابل‌مشاهده (Q12 / قاعدهٔ فرایندی ۱): درخواست در مرحلهٔ مدیر شرکت فقط برای کسی دیده می‌شود
    /// که صف مدیر را می‌بیند یا خودش ثبت‌کنندهٔ مجاز آن شرکت است؛ کاربر شعبه از مرحلهٔ اعتبارات به بعد می‌بیند.
    /// </summary>
    protected IQueryable<LoanRequest> ViewableLoans()
    {
        var managerStage = new[] { LoanRequestStatus.PendingManager, LoanRequestStatus.RejectedByManager };
        var early = ScopedLoans(FacilityPermissionKeys.ManagerApprovalView).Select(l => l.Id)
            .Union(ScopedLoans(FacilityPermissionKeys.LoansCreate).Select(l => l.Id));
        return ScopedLoans(FacilityPermissionKeys.LoansView)
            .Where(l => !managerStage.Contains(l.Status) || early.Contains(l.Id));
    }

    /// <summary>همان قاعدهٔ <see cref="ViewableLoans"/> برای یک درخواست مشخص.</summary>
    protected bool CanSeeStage(LoanRequest l) =>
        (l.Status != LoanRequestStatus.PendingManager && l.Status != LoanRequestStatus.RejectedByManager)
        || CanOn(FacilityPermissionKeys.ManagerApprovalView, l) || CanOn(FacilityPermissionKeys.LoansCreate, l);

    // ── اطلاعات پایه ──

    protected async Task<SchemeSettings> SettingsAsync(int schemeId, CancellationToken ct) =>
        await Db.SchemeSettings.Include(s => s.Plans).FirstOrDefaultAsync(s => s.SchemeId == schemeId, ct)
        ?? throw FacilityDomainException.NotFound("اطلاعات پایهٔ این طرح تعریف نشده است.");

    /// <summary>
    /// دلیل، در دامنهٔ طرح. هر دلیل به یک طرح تعلق دارد، پس دلیلی که به طرح این شرکت
    /// تعلق ندارد همان‌قدر نامعتبر است که دلیل ناموجود.
    /// </summary>
    /// <summary>
    /// طرح پیش‌فرض، برای فراخوان‌هایی که طرح نمی‌فرستند. تا وقتی یک طرح فعال هست کار می‌کند؛
    /// با طرح دوم عمداً خطا می‌دهد تا کسی بی‌خبر فهرست طرح اشتباهی را نگیرد.
    /// </summary>
    protected async Task<int> DefaultSchemeIdAsync(CancellationToken ct)
    {
        var active = await Db.Schemes.Where(s => s.IsActive).Select(s => s.Id).Take(2).ToListAsync(ct);
        return active.Count switch
        {
            1 => active[0],
            0 => throw Invalid("هیچ طرح فعالی تعریف نشده است."),
            _ => throw Invalid("بیش از یک طرح فعال وجود دارد؛ طرح را در درخواست مشخص کنید (schemeId)."),
        };
    }

    protected async Task<Reason> ReasonAsync(int? id, ReasonType type, int schemeId, string requiredMessage, CancellationToken ct)
    {
        if (id is null or <= 0) throw Invalid(requiredMessage);
        return await Db.Reasons.FirstOrDefaultAsync(r => r.Id == id && r.SchemeId == schemeId && r.Type == type && r.IsActive, ct)
               ?? throw Invalid(requiredMessage);
    }

    /// <summary>
    /// دلیل، بدون قید طرح — برای تصمیم گروهی که ردیف‌هایش می‌توانند از چند طرح باشند.
    /// تطابق طرح همان‌جا و برای هر ردیف بررسی می‌شود، تا ردیف ناهمخوان رد شود نه کل دسته.
    /// </summary>
    protected async Task<Reason> ReasonAnySchemeAsync(int? id, ReasonType type, string requiredMessage, CancellationToken ct)
    {
        if (id is null or <= 0) throw Invalid(requiredMessage);
        return await Db.Reasons.FirstOrDefaultAsync(r => r.Id == id && r.Type == type && r.IsActive, ct) ?? throw Invalid(requiredMessage);
    }

    /// <summary>طرح‌های تفاهم‌نامهٔ شرکت به ترتیب ثبت.</summary>
    protected static IReadOnlyList<SchemePlan> PlansOf(Company company, SchemeSettings settings) =>
        company.Plans.Select(cp => settings.Plans.FirstOrDefault(p => p.Id == cp.PlanId)).Where(p => p != null).Cast<SchemePlan>().ToList();

    /// <summary>درخواست‌های ردنشده/لغونشدهٔ شرکت با تاریخ مبنای ثبتشان (کسر از اعتبار).</summary>
    protected async Task<IReadOnlyList<Allocation>> AllocationsAsync(long companyId, CancellationToken ct) =>
        (await (from l in Db.LoanRequests
                join b in Db.LoanBatches on l.BatchId equals b.Id
                where l.CompanyId == companyId && !LoanRequest.ReleasedStatuses.Contains(l.Status)
                select new { l.PlanId, l.Amount, b.BaseDate }).ToListAsync(ct))
        .Select(x => new Allocation(x.PlanId, x.Amount, x.BaseDate)).ToList();

    protected async Task<CompanyCredit> CreditAsync(Company company, DateOnly baseDate, CancellationToken ct)
    {
        var settings = await SettingsAsync(company.SchemeId, ct);
        return CreditRules.Calculate(settings, company, PlansOf(company, settings), await AllocationsAsync(company.Id, ct), baseDate);
    }

    // ── فایل ──

    /// <summary>اعتبارسنجی (FileRules)، ذخیرهٔ محتوا و افزودن متادیتا؛ متادیتا با SaveChanges همان Command ذخیره می‌شود.</summary>
    protected async Task<StoredFile?> StoreAsync(IFacilityFileStorage storage, FileUpload? file, string[] extensions, long maxSize, bool required, CancellationToken ct)
    {
        var error = FileRules.Validate(file, extensions, maxSize, required);
        if (error != null) throw Invalid(error);
        if (file == null) return null;
        var key = await storage.PutAsync(file.FileName, file.ContentType, file.Content, ct);
        var stored = new StoredFile(Path.GetFileName(file.FileName), string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType,
            file.Length, key, Now, Me.UserName);
        Db.Files.Add(stored);
        return stored;
    }

    protected static async Task<FileUpload> ReadAsync(IFacilityFileStorage storage, StoredFile file, CancellationToken ct)
    {
        var content = await storage.GetAsync(file.StorageKey, ct) ?? throw FacilityDomainException.NotFound("فایل یافت نشد.");
        return new FileUpload(content, file.FileName, file.ContentType);
    }

    // ── گزارش تغییرات ──

    protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    protected static string? ToJson(object? value) => value == null ? null : JsonSerializer.Serialize(value, Json);

    protected ChangeLog Log(ChangeLogCategory category, string action, string details, Company? company,
        object? before = null, object? after = null, string? restoreKind = null, object? restorePayload = null)
    {
        var log = new ChangeLog(Now, Me, category, action, details, ToJson(before), ToJson(after), company,
            restoreKind, ToJson(restorePayload));
        Db.ChangeLogs.Add(log);
        return log;
    }

    // ── درخواست‌های وام ──

    protected static FacilityResource Resource(LoanRequest l) => FacilityResource.OfCompany(l.BranchCode, l.CompanyId.ToString());

    protected bool CanOn(string key, LoanRequest l) => Permissions.Can(key, Resource(l));

    /// <summary>ردیف‌های جدول درخواست‌ها با نام شرکت، پرسنل، عنوان طرح و امکان لغو برای کاربر جاری.</summary>
    protected async Task<List<LoanRequestListItemDto>> LoanItemsAsync(IReadOnlyCollection<LoanRequest> loans, CancellationToken ct)
    {
        if (loans.Count == 0) return [];
        var ids = loans.Select(l => l.Id).ToList();
        var companyIds = loans.Select(l => l.CompanyId).Distinct().ToList();
        var personnelIds = loans.Select(l => l.PersonnelId).Distinct().ToList();
        var planIds = loans.Select(l => l.PlanId).Distinct().ToList();
        var companies = await Db.Companies.Where(c => companyIds.Contains(c.Id)).Select(c => new { c.Id, c.Name }).ToDictionaryAsync(c => c.Id, c => c.Name, ct);
        var people = await Db.Personnel.Where(p => personnelIds.Contains(p.Id)).Select(p => new { p.Id, p.FullName, p.PersonnelCode }).ToDictionaryAsync(p => p.Id, ct);
        var plans = await Db.SchemePlans.Where(p => planIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, ct);
        // فرانت در هر ردیف خود فایل مستند مدیر شرکت را نشان می‌دهد، نه فقط تعدادش؛ تا امروز
        // باید برای هر ردیف جزئیات درخواست را جدا می‌گرفت.
        var docs = (await Db.LoanRequests.Where(l => ids.Contains(l.Id))
                .SelectMany(l => l.Documents.Select(d => new { RequestId = l.Id, d.File }))
                .ToListAsync(ct))
            .GroupBy(x => x.RequestId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<StoredFileDto>)g.Select(x => x.File.ToDto()).ToList());
        return loans.Select(l => new LoanRequestListItemDto(
            l.Id, l.BatchId, l.CompanyId, companies.GetValueOrDefault(l.CompanyId, "—"), l.BranchCode, l.PersonnelId,
            people.TryGetValue(l.PersonnelId, out var p) ? p.FullName : "—", p?.PersonnelCode ?? "",
            plans.TryGetValue(l.PlanId, out var plan) ? plan.Title : "", l.Amount, l.PaymentOrder, l.CreatedAt, l.DecidedAt,
            l.Status, l.PostStage, l.Description, docs.GetValueOrDefault(l.Id, []).Count,
            l.Status is LoanRequestStatus.PendingManager or LoanRequestStatus.PendingCredit && CanOn(FacilityPermissionKeys.LoansCancel, l),
            docs.GetValueOrDefault(l.Id, []))).ToList();
    }

    protected static FacilityPagedResponse<T> Page<T>(IReadOnlyList<T> items, int total, int page, int pageSize) => new(items, total, page, pageSize);

    protected static (int Page, int Size) Paging(int page, int size, int defaultSize = 20, int max = 500) =>
        (Math.Max(1, page), size is < 1 ? defaultSize : Math.Min(size, max));
}
