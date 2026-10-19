using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Dashboard.Queries.ExportDashboard;

public sealed class ExportDashboardQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : DashboardHandlerBase(db, user, time), IRequestHandler<ExportDashboardQuery, FacilityBaseMessageResponse<FileUpload>>
{
    public async Task<FacilityBaseMessageResponse<FileUpload>> Handle(ExportDashboardQuery request, CancellationToken ct)
    {
        var d = await LoadAsync(null, null, null, null, ct);
        var live = d.Companies.Where(c => !c.IsDeleted).ToList();
        var names = d.Companies.ToDictionary(c => c.Id, c => c.Name);
        var plans = d.Settings.Values.SelectMany(s => s.Plans).ToDictionary(p => p.Id, p => p.Title);
        var stamp = Jalali.Format(Today).Replace('/', '-');

        switch (request.Report)
        {
            case DashboardExport.Companies:
                return Ok(Csv.File($"companies-{stamp}.csv",
                    ["نام شرکت", "شناسهٔ ملی", "شمارهٔ تفاهم‌نامه", "کد شعبه", "تاریخ قرارداد", "تعداد طرح", "دریافت حقوق از سپه", "وضعیت", "ثبت‌کننده"],
                    live.Select(c => new object?[] { c.Name, c.NationalId, c.AgreementNumber, c.BranchCode, c.ContractDate, c.Plans.Count, c.SalaryFromSepah, StatusTitles.Of(c.Status), c.Registrar.UserName })));

            case DashboardExport.Personnel:
            {
                var ids = live.Select(c => c.Id).ToList();
                var people = await Db.Personnel.Where(p => ids.Contains(p.CompanyId) && p.Status != PersonnelStatus.Deleted).OrderBy(p => p.CompanyId).ThenBy(p => p.Priority).ToListAsync(ct);
                // پرسنل تأییدنشده فقط برای نقش‌هایی که آن‌ها را می‌بینند (معیار پرسنل ۳)
                var seesAll = live.Where(c => CanOnAny(c, K.PersonnelCreate, K.PersonnelApprove, K.PersonnelEdit)).Select(c => c.Id).ToHashSet();
                return Ok(Csv.File($"personnel-{stamp}.csv",
                    ["شرکت", "اولویت", "نام و نام خانوادگی", "کد ملی", "موبایل", "کد پرسنلی", "تاریخ استخدام", "وضعیت"],
                    people.Where(p => p.Status == PersonnelStatus.Approved || seesAll.Contains(p.CompanyId))
                        .Select(p => new object?[] { names[p.CompanyId], p.Priority, p.FullName, p.NationalCode, p.Mobile, p.PersonnelCode, p.HireDate, StatusTitles.Of(p.Status) })));
            }

            case DashboardExport.LoanRequests:
            {
                var personIds = d.Loans.Select(l => l.PersonnelId).Distinct().ToList();
                var people = await Db.Personnel.Where(p => personIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.FullName, ct);
                return Ok(Csv.File($"loan-requests-{stamp}.csv",
                    ["شمارهٔ درخواست", "شرکت", "کد شعبه", "پرسنل", "طرح", "مبلغ (ریال)", "ترتیب پرداخت", "تاریخ ثبت", "تاریخ تصمیم", "وضعیت"],
                    d.Loans.OrderByDescending(l => l.Id).Select(l => new object?[] { l.Id, names[l.CompanyId], l.BranchCode, people.GetValueOrDefault(l.PersonnelId), plans.GetValueOrDefault(l.PlanId), l.Amount, l.PaymentOrder, l.CreatedAt, l.DecidedAt, StatusTitles.Of(l.Status) })));
            }

            case DashboardExport.MonthlyAverages:
                return Ok(Csv.File($"monthly-averages-{stamp}.csv", ["شرکت", "ماه", "میانگین موجودی (ریال)", "ثبت‌کننده", "زمان ثبت"],
                    live.SelectMany(c => c.Balances.OrderBy(b => b.Month).Select(b => new object?[] { c.Name, PersianText.Digits(b.Month), b.Amount, b.EditedBy.UserName, b.EditedAt }))));

            default:
                return Ok(Csv.File($"removed-companies-{stamp}.csv", ["شرکت", "علت", "تاریخ حذف/لغو", "آخرین میانگین", "مجموع تخصیص", "تعداد درخواست", "درصد تأیید"],
                    Removed(d, d.Loans.Where(l => !LoanRequest.ReleasedStatuses.Contains(l.Status)).GroupBy(l => l.CompanyId).ToDictionary(g => g.Key, g => g.Sum(l => l.Amount)))
                        .Select(r => new object?[] { r.CompanyName, RemovalTitle(r.RemovalKind), r.RemovedAt, r.AverageBalance, r.AllocatedTotal, r.RequestCount, r.ApprovalRate })));
        }
    }

    private static string RemovalTitle(CompanyStatus status) => status switch
    {
        CompanyStatus.Cancelled => "لغو قرارداد",
        CompanyStatus.Expired => "انقضای قرارداد",
        _ => "حذف شرکت",
    };
}
