using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Persistence;
using MediatR;

namespace FacilityService.Application.Handler.Dashboard.Queries.GetDashboardMonths;

public sealed class GetDashboardMonthsQueryHandler(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time)
    : DashboardHandlerBase(db, user, time), IRequestHandler<GetDashboardMonthsQuery, FacilityBaseMessageResponse<IReadOnlyList<MonthOptionDto>>>
{
    private static readonly string[] Names =
        ["فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"];

    public async Task<FacilityBaseMessageResponse<IReadOnlyList<MonthOptionDto>>> Handle(GetDashboardMonthsQuery request, CancellationToken ct)
    {
        var d = await LoadAsync(null, null, null, null, ct);

        // ماه‌هایی که واقعاً داده دارند: ماه میانگین موجودی شرکت‌ها و ماه ثبت درخواست‌ها.
        // ماه جاری همیشه هست تا فیلتر در شروع کار شرکت خالی نماند.
        var keys = d.Companies.SelectMany(c => c.Balances.Select(b => b.Month))
            .Concat(d.Loans.Select(l => Jalali.MonthKey(DateOnly.FromDateTime(l.CreatedAt))))
            .Append(Jalali.MonthKey(Today))
            .Where(k => Jalali.TryParseMonthKey(k, out _, out _))
            .Distinct()
            .OrderByDescending(k => k, StringComparer.Ordinal)
            .ToList();

        return Ok<IReadOnlyList<MonthOptionDto>>(keys.Select(Option).ToList());
    }

    private static MonthOptionDto Option(string key)
    {
        Jalali.TryParseMonthKey(key, out var year, out var month);
        return new MonthOptionDto(key, $"{Names[month - 1]} {PersianText.Digits(year.ToString())}");
    }
}
