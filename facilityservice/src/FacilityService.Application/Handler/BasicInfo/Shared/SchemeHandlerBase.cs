using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.BasicInfo.Shared;

/// <summary>
/// عضوهای مشترک سرویس‌های طرح. «فرم طرح» از سه جا سر هم می‌شود (هویت طرح، تنظیمات،
/// فهرست‌های دلایل) و هر سه سرویس خواندن/نوشتن باید همان شکل را برگردانند، پس ساختنش
/// یک جا نوشته شده.
/// </summary>
public abstract class SchemeHandlerBase(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : FacilityPanelHandlerBase(db, user, time)
{
    protected async Task<SchemeFormDto> FormAsync(Scheme scheme, CancellationToken ct)
    {
        var settings = await SettingsAsync(scheme.Id, ct);
        var reasons = await Db.Reasons.Where(r => r.SchemeId == scheme.Id)
            .OrderBy(r => r.SortOrder).ThenBy(r => r.Id).ToListAsync(ct);
        var companies = await Db.Companies.CountAsync(c => c.SchemeId == scheme.Id && c.DeletedAt == null, ct);

        // غیرفعال‌ها فقط برای کسی که فرم طرح را ویرایش می‌کند؛ بقیه کشویی تمیز می‌خواهند.
        var showInactive = Permissions.HasAnywhere(FacilityPermissionKeys.BasicInfoEdit);
        IReadOnlyList<ReasonDto> Of(ReasonType type) => reasons
            .Where(r => r.Type == type && (showInactive || r.IsActive)).Select(r => r.ToDto()).ToList();

        return new SchemeFormDto(scheme.Id, scheme.Code, scheme.Title, scheme.IsActive, settings.ToDto(),
            new SchemeReasonsDto(
                Of(ReasonType.ContractCancel), Of(ReasonType.LoanReject), Of(ReasonType.RemoteGrantDenied),
                Of(ReasonType.DocumentType), Of(ReasonType.LoanCancel), Of(ReasonType.PersonnelReject)),
            companies);
    }

    /// <summary>
    /// فهرست‌های طرح را با فهرست‌های ورودی یکی می‌کند: مورد تازه اضافه، مورد موجود با
    /// همان عنوان نگه داشته (تا رکوردهای قدیمی‌اش نشکنند) و مورد نبوده غیرفعال می‌شود.
    /// null یعنی این فهرست تغییر نکند.
    /// </summary>
    protected async Task MergeReasonsAsync(int schemeId, ReasonType type, IReadOnlyList<string>? titles, CancellationToken ct)
    {
        if (titles == null) return;
        var clean = titles.Select(t => (t ?? "").Trim()).Where(t => t.Length > 0).ToList();
        if (clean.Count != clean.Distinct().Count()) throw Invalid("در یکی از فهرست‌های طرح عنوان تکراری وجود دارد.");

        var existing = await Db.Reasons.Where(r => r.SchemeId == schemeId && r.Type == type).ToListAsync(ct);
        for (var i = 0; i < clean.Count; i++)
        {
            var match = existing.FirstOrDefault(r => r.Title == clean[i]);
            if (match == null) Db.Reasons.Add(new Reason(schemeId, type, clean[i], i));
            else match.Update(clean[i], isActive: true, sortOrder: i);
        }
        foreach (var gone in existing.Where(r => r.IsActive && !clean.Contains(r.Title))) gone.Deactivate();
    }
}
