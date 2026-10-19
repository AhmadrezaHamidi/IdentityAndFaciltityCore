using System.Text.RegularExpressions;

namespace FacilityService.Domain.Entity.Companies;

/// <summary>مشخصات قابل ویرایش شرکت (فرم ثبت و ویرایش، فیلدهای ۲۱۷ تا ۲۳۱).</summary>
public sealed record CompanyData(
    string Name, string AgreementNumber, string AccountNumber, DateOnly ContractDate, DateOnly AccountOpeningDate,
    bool SalaryFromSepah, IReadOnlyList<int> PlanIds);
