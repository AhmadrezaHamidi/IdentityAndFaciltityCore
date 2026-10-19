using System.Text.Json;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.ChangeLogs.Shared;

/// <summary>عضوهای مشترک Handlerهای این بخش (قبلاً در ChangeLogHandlers).</summary>
public abstract class ChangeLogHandlerBase(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time) : FacilityPanelHandlerBase(db, user, time)
{
    /// <summary>Q16 (2026-10-16): بازگردانی فعلاً ساخته نمی‌شود؛ بعداً فقط ویرایش شرکت/پرسنل و تنظیمات طرح. با روشن‌کردن این مقدار، منطق قبلی دوباره فعال می‌شود.</summary>
    protected const bool RestoreEnabled = false;

    /// <summary>
    /// دامنهٔ گزارش تغییرات. ردیف‌های «پرسنل» (معرفی، رد و اصلاح افراد در انتظار) فقط برای کسی است که پرسنل تأییدنشدهٔ
    /// همان شرکت را می‌بیند (سند، پرسنل شرکت ۳: ادمین، منابع انسانی و مدیر شرکت)؛ کاربر شعبه آن‌ها را نمی‌بیند.
    /// </summary>
    protected IQueryable<ChangeLog> Scoped()
    {
        IQueryable<ChangeLog> In(string key) =>
            Permissions.ScopeFor(key).Apply(Db.ChangeLogs.AsQueryable(), l => l.BranchCode, l => l.CompanyId.ToString());
        var personnel = In(K.PersonnelCreate).Select(l => l.Id)
            .Union(In(K.PersonnelApprove).Select(l => l.Id))
            .Union(In(K.PersonnelEdit).Select(l => l.Id));
        return In(K.AuditView).Where(l => l.Category != ChangeLogCategory.Personnel || personnel.Contains(l.Id));
    }

    /// <summary>
    /// فیلترها یک جا، چون خروجی اکسل باید دقیقاً همان چیزی باشد که کاربر در جدول می‌بیند؛
    /// قبلاً جستجوی متنی و فیلتر کاربر در خروجی اعمال نمی‌شد.
    /// </summary>
    protected IQueryable<ChangeLog> Filter(ChangeLogCategory? category, long? companyId, int? userId,
        DateTime? from, DateTime? to, string? term = null)
    {
        var q = Scoped();
        if (category != null) q = q.Where(l => l.Category == category);
        if (companyId != null) q = q.Where(l => l.CompanyId == companyId);
        if (userId != null) q = q.Where(l => l.UserId == userId);
        if (from != null) q = q.Where(l => l.At >= from);
        if (to != null) q = q.Where(l => l.At <= to);
        var text = term?.Trim();
        if (!string.IsNullOrEmpty(text)) q = q.Where(l => l.Action.Contains(text) || l.Details.Contains(text));
        return q;
    }

    protected ChangeLogItemDto ToDto(ChangeLog l, bool canRestore) =>
        new(l.Id, l.At, l.User.ToDto(), l.Category, l.Action, l.Details, l.Before, l.After, l.CompanyId, RestoreEnabled && canRestore && l.CanRestore, l.RestoredAt);
}
