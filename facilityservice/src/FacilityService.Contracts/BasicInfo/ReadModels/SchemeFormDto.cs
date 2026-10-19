using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.BasicInfo.ReadModels;

/// <summary>
/// یک ردیف جدول «اطلاعات پایه طرح». هر چیزی که در جدول نشان داده می‌شود این‌جاست، تا فرانت
/// برای پر کردن جدول مجبور نشود برای هر ردیف سرویس جزئیات را صدا بزند.
/// </summary>
public sealed record SchemeListItemDto(
    int Id,
    string Code,
    string Title,
    decimal QardHasanRate,
    IReadOnlyList<decimal> MurabahaRates,
    IReadOnlyList<int> RepaymentTermsMonths,
    int ContractDurationMonths,
    int MaxPlansPerCompany,
    bool IsActive,
    int CompanyCount);

/// <summary>
/// همهٔ ۱۴ مورد فرم طرح، در یک پاسخ — و همین شکل در ایجاد و ویرایش برگردانده می‌شود.
///
/// چرا این‌طور شد: قبلاً هویت طرح (نام و کد)، تنظیماتش و فهرست‌های دلایلش سه جای جدا بودند
/// و فهرست‌ها هم سراسری بودند، پس «فرم طرح» در هیچ سرویسی کامل نبود.
/// </summary>
/// <param name="Reasons">فهرست‌های ۱۱ تا ۱۴ فرم، همه متعلق به همین طرح.</param>
public sealed record SchemeFormDto(
    int Id,
    string Code,
    string Title,
    bool IsActive,
    BasicInfoDto Settings,
    SchemeReasonsDto Reasons,
    int CompanyCount);

/// <param name="ContractCancel">دلایل لغو قرارداد (مورد ۱۱ فرم).</param>
/// <param name="LoanReject">دلایل رد درخواست وام (مورد ۱۲).</param>
/// <param name="RemoteGrantDenied">دلایل عدم اعطای تسهیلات غیرحضوری (مورد ۱۳).</param>
/// <param name="DocumentType">انواع مستند میانگین موجودی (مورد ۱۴).</param>
public sealed record SchemeReasonsDto(
    IReadOnlyList<ReasonDto> ContractCancel,
    IReadOnlyList<ReasonDto> LoanReject,
    IReadOnlyList<ReasonDto> RemoteGrantDenied,
    IReadOnlyList<ReasonDto> DocumentType,
    IReadOnlyList<ReasonDto> LoanCancel,
    IReadOnlyList<ReasonDto> PersonnelReject);
