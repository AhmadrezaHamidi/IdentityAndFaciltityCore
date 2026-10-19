using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.BasicInfo.ReadModels;

/// <param name="UsableTable">جدول درصد قابل استفاده از میانگین: هر ردیف یک مدت، هر ستون یک نرخ.</param>
/// <remarks>نرخ‌ها و درصدها «درصد» هستند (۴ یعنی ۴٪)، مثل نمونهٔ پنل.</remarks>
public sealed record BasicInfoDto(
    int SchemeId,
    IReadOnlyList<ContractType> ContractTypes,
    decimal QardHasanRate,
    IReadOnlyList<decimal> MurabahaRates,
    IReadOnlyList<int> RepaymentTermsMonths,
    decimal SalaryFromSepahMultiplierYes,
    decimal SalaryFromSepahMultiplierNo,
    int ContractDurationMonths,
    int GraceMonthsAfterExpiry,
    int MaxPlansPerCompany,
    int PersonnelIntroductionDelayMonths,
    int WaitingPeriodDays,
    IReadOnlyList<UsablePercentDto> UsableTable,
    string RowVersion,
    bool DigitalApprovalEnabled = false);
