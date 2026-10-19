using FacilityService.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Infrastructure.Seed;

/// <summary>
/// دادهٔ اولیه: طرح «نگین میثاق» با تنظیمات و جدول درصد قابل استفادهٔ نمونهٔ پنل و فهرست‌های داینامیک.
/// همان دادهٔ Migration «SeedNeginMisaghScheme»؛ برای SQLite محلی و تست (روی SQL Server خود Migration این را می‌سازد).
/// </summary>
public static class FacilityPanelSeed
{
    public static readonly decimal[] MurabahaRates = [5, 10, 15, 20, 23];

    public static IReadOnlyList<UsablePercentRow> UsableTable { get; } =
    [
        .. new (int Term, decimal Usable)[] { (12, 8), (24, 6), (36, 5), (48, 4), (60, 3) }
            .Select(x => new UsablePercentRow(x.Term, ContractType.QardHasan, 4, x.Usable)),
        // ترتیب نمونهٔ پنل (buildPlanMatrix): برای هر نرخ مرابحه، همهٔ مدت‌ها
        .. MurabahaRates.SelectMany((rate, i) => new (int Term, decimal[] Usable)[]
            {
                (12, [10, 15, 25, 35, 45]), (24, [8, 10, 15, 20, 25]), (36, [6, 8, 10, 15, 20]),
                (48, [4, 6, 8, 10, 15]), (60, [3, 5, 6, 8, 10]),
            }
            .Select(x => new UsablePercentRow(x.Term, ContractType.Murabaha, rate, x.Usable[i]))),
    ];

    public static readonly (ReasonType Type, string[] Titles)[] Reasons =
    [
        (ReasonType.ContractCancel, ["عدم تمدید به درخواست شرکت", "لغو قرارداد توسط مدیریت شعب منطقه"]),
        (ReasonType.LoanReject, ["عدم کنترل سقف فردی", "پایین بودن رتبه اعتباری"]),
        (ReasonType.RemoteGrantDenied, ["رتبه اعتباری غیرمجاز", "عدم کفایت سقف تسهیلات بر اساس اعتبارسنجی"]),
        (ReasonType.LoanCancel, ["انصراف متقاضی", "ترک خدمت"]),
        (ReasonType.PersonnelReject, ["اطلاعات نادرست", "عدم احراز شرایط"]),
        (ReasonType.DocumentType, ["تصویر بنکو", "نامه", "گردش حساب"]),
    ];

    public static async Task EnsureSeedAsync(IFacilityPanelDbContext db, CancellationToken ct = default)
    {
        if (await db.Schemes.AnyAsync(ct)) return;
        var scheme = new Scheme("NEGIN", "نگین میثاق");
        db.Schemes.Add(scheme);
        await db.SaveChangesAsync(ct);

        var settings = new SchemeSettings(scheme.Id);
        settings.Update([ContractType.QardHasan, ContractType.Murabaha], 4, MurabahaRates, [12, 24, 36, 48, 60], 1.1m, 1m,
            contractDurationMonths: 12, graceMonths: 4, maxPlans: 5, introductionDelayMonths: 1, waitingPeriodDays: 0,
            digitalApprovalEnabled: false, usableTable: UsableTable);
        db.SchemeSettings.Add(settings);

        foreach (var (type, titles) in Reasons)
            for (var i = 0; i < titles.Length; i++)
                db.Reasons.Add(new Reason(scheme.Id, type, titles[i], i));
        await db.SaveChangesAsync(ct);
    }
}
