using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

/// <param name="CreditVsAllocated">برای ادمین/دیجیتال به تفکیک شعبه (US-C3: نسبت درخواست به اعتبار تخصیصی شعب)، برای بقیه به تفکیک شرکت</param>
/// <param name="CreditVsAllocatedGroupKind">
/// می‌گوید گروه‌های <paramref name="CreditVsAllocated"/> شعبه‌اند یا شرکت؛ به دامنهٔ کاربر بستگی دارد
/// و فرانت نباید حدس بزند.
/// </param>
/// <param name="MonthlyAllocation">
/// مبلغ هر ماه با کلید ماه شمسی به قالب <c>1404/05</c>. ترتیب ستون‌ها را از
/// <c>GET Dashboard/Months</c> بگیرید؛ در این آبجکت فقط ماه‌هایی هستند که مبلغ دارند.
/// </param>
public sealed record DashboardDto(
    DashboardKpisDto Kpis,
    RequestStatusChartDto RequestStatus,
    IReadOnlyList<PlanShareDto> PlanShare,
    IReadOnlyList<CreditVsAllocatedDto> CreditVsAllocated,
    DashboardGroupKind CreditVsAllocatedGroupKind,
    IReadOnlyList<ContractsByMonthDto> ContractsByMonth,
    IReadOnlyList<MonthlyAllocationDto> MonthlyAllocation,
    IReadOnlyList<RemovedCompanyDto> RemovedCompanies);
