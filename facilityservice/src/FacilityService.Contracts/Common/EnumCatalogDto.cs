namespace FacilityService.Contracts.Common;

/// <summary>
/// یک مقدار از یک enum، با عنوان فارسی‌اش.
/// فرانت وضعیت‌ها را از همین می‌خواند و عنوان‌ها را در کد خودش تکرار نمی‌کند — پس افزودن
/// یک وضعیت جدید در بک‌اند، فرانت را نمی‌شکند و نیازی به دیپلوی هم‌زمان ندارد.
/// </summary>
/// <param name="Value">همان عدد روی سیم.</param>
/// <param name="Name">نام انگلیسی عضو enum؛ برای کدی که باید به یک مقدار خاص واکنش دهد.</param>
/// <param name="Title">عنوان فارسی برای نمایش.</param>
/// <param name="RequiresReason">
/// این مقدار هنگام انتخاب، دلیل هم لازم دارد. فعلاً فقط «عدم اعطای تسهیلات غیرحضوری»
/// در مرحلهٔ پس از تأیید. فرانت با همین می‌فهمد کِی باید کشویی دلیل را نشان دهد.
/// </param>
public sealed record EnumValueDto(byte Value, string Name, string Title, bool RequiresReason = false);

/// <summary>
/// همهٔ enumهای روی سیم، در یک پاسخ. فرانت یک بار می‌خواند و کش می‌کند.
///
/// کلید هر فهرست همان نام enum در بک‌اند است، تا وقتی چیزی اضافه شد، جای پیدا کردنش روشن باشد.
/// </summary>
public sealed record EnumCatalogDto(
    IReadOnlyList<EnumValueDto> CompanyStatus,
    IReadOnlyList<EnumValueDto> PersonnelStatus,
    IReadOnlyList<EnumValueDto> LoanRequestStatus,
    IReadOnlyList<EnumValueDto> PostApprovalStage,
    IReadOnlyList<EnumValueDto> AllocationMode,
    IReadOnlyList<EnumValueDto> EligibilityStatus,
    IReadOnlyList<EnumValueDto> ApprovalStage,
    IReadOnlyList<EnumValueDto> DecisionType,
    IReadOnlyList<EnumValueDto> ContractType,
    IReadOnlyList<EnumValueDto> ReasonType,
    IReadOnlyList<EnumValueDto> ChangeLogCategory,
    IReadOnlyList<EnumValueDto> CompanyRankingSort,
    IReadOnlyList<EnumValueDto> DashboardExport,
    IReadOnlyList<EnumValueDto> HintLevel);
