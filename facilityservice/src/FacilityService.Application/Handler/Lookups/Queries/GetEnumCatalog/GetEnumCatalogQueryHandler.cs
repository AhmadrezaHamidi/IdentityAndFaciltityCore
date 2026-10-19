using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Domain.Enums;
using MediatR;

namespace FacilityService.Application.Handler.Lookups.Queries.GetEnumCatalog;

/// <summary>
/// عنوان‌ها این‌جا نوشته شده‌اند و نه روی خود enum با Attribute، چون enumها در لایهٔ Domain
/// هستند و نباید به نمایش گره بخورند. اگر عنوانی عوض شود، فقط همین فایل عوض می‌شود.
///
/// به دیتابیس کاری ندارد، پس DbContext هم نمی‌گیرد.
/// </summary>
public sealed class GetEnumCatalogQueryHandler
    : IRequestHandler<GetEnumCatalogQuery, FacilityBaseMessageResponse<EnumCatalogDto>>
{
    public Task<FacilityBaseMessageResponse<EnumCatalogDto>> Handle(GetEnumCatalogQuery request, CancellationToken ct) =>
        Task.FromResult(MessageResponse.GenerateMessageResponse(200, Catalog, "عملیات با موفقیت انجام شد."));

    private static readonly EnumCatalogDto Catalog = new(
        CompanyStatus:
        [
            V(Domain.Enums.CompanyStatus.Active,    "فعال"),
            V(Domain.Enums.CompanyStatus.Cancelled, "لغوشده"),
            V(Domain.Enums.CompanyStatus.Expired,   "منقضی‌شده"),
            V(Domain.Enums.CompanyStatus.Deleted,   "حذف‌شده"),
        ],
        PersonnelStatus:
        [
            V(Domain.Enums.PersonnelStatus.PendingManager, "در انتظار تأیید مدیر شرکت"),
            V(Domain.Enums.PersonnelStatus.Approved,       "تأییدشده"),
            V(Domain.Enums.PersonnelStatus.Rejected,       "ردشده"),
            V(Domain.Enums.PersonnelStatus.Deleted,        "حذف‌شده"),
        ],
        LoanRequestStatus:
        [
            V(Domain.Enums.LoanRequestStatus.PendingManager,    "در انتظار تأیید مدیر شرکت"),
            V(Domain.Enums.LoanRequestStatus.RejectedByManager, "ردشده توسط مدیر شرکت"),
            V(Domain.Enums.LoanRequestStatus.PendingCredit,     "در انتظار تأیید اعتبارات"),
            V(Domain.Enums.LoanRequestStatus.RejectedByCredit,  "ردشده توسط اعتبارات"),
            V(Domain.Enums.LoanRequestStatus.PendingDigital,    "در انتظار تأیید شعبهٔ دیجیتال"),
            V(Domain.Enums.LoanRequestStatus.RejectedByDigital, "ردشده توسط شعبهٔ دیجیتال"),
            V(Domain.Enums.LoanRequestStatus.Approved,          "تأییدشده"),
            V(Domain.Enums.LoanRequestStatus.Cancelled,         "لغوشده"),
        ],
        PostApprovalStage:
        [
            V(Domain.Enums.PostApprovalStage.FileOpenedAtBranch,   "تشکیل پرونده در شعبه"),
            V(Domain.Enums.PostApprovalStage.ReceivedFromOmidBank, "دریافت تسهیلات از امید بانک"),
            V(Domain.Enums.PostApprovalStage.ReceivedAtBranch,     "دریافت تسهیلات در شعبه"),
            // تنها مرحله‌ای که دلیل می‌خواهد (دلایل نوع ۳)
            V(Domain.Enums.PostApprovalStage.RemoteGrantDenied,    "عدم اعطای تسهیلات غیرحضوری", requiresReason: true),
        ],
        AllocationMode:
        [
            V(Domain.Enums.AllocationMode.Single,   "مبلغ یکسان برای همه"),
            V(Domain.Enums.AllocationMode.Variable, "مبلغ جدا برای هر نفر"),
        ],
        EligibilityStatus:
        [
            V(Domain.Enums.EligibilityStatus.IntroductionNotYetAllowed, "هنوز به مهلت معرفی پرسنل نرسیده"),
            V(Domain.Enums.EligibilityStatus.NotTransferDay,            "امروز روز مجاز انتقال اعتبار نیست"),
            V(Domain.Enums.EligibilityStatus.Allowed,                   "مجاز به ثبت درخواست وام"),
            V(Domain.Enums.EligibilityStatus.ContractInactive,          "قرارداد لغو یا منقضی شده"),
        ],
        ApprovalStage:
        [
            V(Domain.Enums.ApprovalStage.Manager, "تأیید مدیر شرکت"),
            V(Domain.Enums.ApprovalStage.Credit,  "تأیید اعتبارات"),
            V(Domain.Enums.ApprovalStage.Digital, "تأیید شعبهٔ دیجیتال"),
        ],
        DecisionType:
        [
            V(Contracts.Common.DecisionType.Approve, "تأیید"),
            V(Contracts.Common.DecisionType.Reject,  "رد"),
        ],
        ContractType:
        [
            V(Domain.Enums.ContractType.QardHasan, "قرض‌الحسنه"),
            V(Domain.Enums.ContractType.Murabaha,  "مرابحه"),
        ],
        ReasonType:
        [
            V(Domain.Enums.ReasonType.ContractCancel,    "دلیل لغو قرارداد شرکت"),
            V(Domain.Enums.ReasonType.LoanReject,        "دلیل رد درخواست وام"),
            V(Domain.Enums.ReasonType.RemoteGrantDenied, "دلیل عدم اعطای تسهیلات غیرحضوری"),
            V(Domain.Enums.ReasonType.LoanCancel,        "دلیل لغو درخواست وام"),
            V(Domain.Enums.ReasonType.PersonnelReject,   "دلیل رد پرسنل معرفی‌شده"),
            V(Domain.Enums.ReasonType.DocumentType,      "نوع مستند میانگین موجودی"),
        ],
        ChangeLogCategory:
        [
            V(Domain.Enums.ChangeLogCategory.Company,   "شرکت"),
            V(Domain.Enums.ChangeLogCategory.Personnel, "پرسنل"),
            V(Domain.Enums.ChangeLogCategory.Loan,      "وام"),
            // دستهٔ چهارمی که فرانت نمی‌دانست چیست
            V(Domain.Enums.ChangeLogCategory.User,      "کاربران"),
        ],
        CompanyRankingSort:
        [
            V(Contracts.Dashboard.ReadModels.CompanyRankingSort.AverageBalance,  "میانگین موجودی"),
            V(Contracts.Dashboard.ReadModels.CompanyRankingSort.AllocatedAmount, "مبلغ وام تخصیص‌یافته"),
            V(Contracts.Dashboard.ReadModels.CompanyRankingSort.RequestCount,    "تعداد درخواست"),
            V(Contracts.Dashboard.ReadModels.CompanyRankingSort.PersonnelCount,  "تعداد پرسنل"),
        ],
        DashboardExport:
        [
            V(Contracts.Dashboard.ReadModels.DashboardExport.Companies,       "شرکت‌ها"),
            V(Contracts.Dashboard.ReadModels.DashboardExport.Personnel,       "پرسنل"),
            V(Contracts.Dashboard.ReadModels.DashboardExport.LoanRequests,    "درخواست‌های وام"),
            V(Contracts.Dashboard.ReadModels.DashboardExport.MonthlyAverages, "میانگین‌های ماهانه"),
            V(Contracts.Dashboard.ReadModels.DashboardExport.RemovedCompanies, "شرکت‌های حذف‌شده"),
        ],
        HintLevel:
        [
            V(Domain.Enums.HintLevel.Info,    "اطلاع"),
            V(Domain.Enums.HintLevel.Success, "موفق"),
            V(Domain.Enums.HintLevel.Warning, "هشدار"),
            V(Domain.Enums.HintLevel.Error,   "خطا"),
        ]);

    private static EnumValueDto V<TEnum>(TEnum value, string title, bool requiresReason = false)
        where TEnum : struct, Enum =>
        new(Convert.ToByte(value), value.ToString()!, title, requiresReason);
}
