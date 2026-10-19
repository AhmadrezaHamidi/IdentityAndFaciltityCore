using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using K = FacilityService.Application.Authorization.FacilityPermissionKeys;

namespace FacilityService.Application.Handler.Approvals.Shared;

/// <summary>عضوهای مشترک Handlerهای این بخش (قبلاً در ApprovalHandlers).</summary>
public abstract class ApprovalHandlerBase(IFacilityPanelDbContext db, IFacilityCurrentUser user, TimeProvider time) : FacilityPanelHandlerBase(db, user, time)
{
    protected string ViewKey(ApprovalStage stage) => stage switch
    {
        ApprovalStage.Manager => K.ManagerApprovalView,
        ApprovalStage.Credit => K.CreditApprovalView,
        _ => Permissions.HasAnywhere(K.DigitalApprovalDecide) ? K.DigitalApprovalDecide : K.CreditApprovalView,
    };

    protected static string DecideKey(ApprovalStage stage, bool approve) => (stage, approve) switch
    {
        (ApprovalStage.Manager, true) => K.ManagerApprovalApprove,
        (ApprovalStage.Manager, false) => K.ManagerApprovalReject,
        (ApprovalStage.Credit, true) => K.CreditApprovalApprove,
        (ApprovalStage.Credit, false) => K.CreditApprovalReject,
        _ => K.DigitalApprovalDecide,
    };

    /// <summary>
    /// بدون فیلتر وضعیت: مدیر شرکت و دیجیتال فقط منتظر تصمیم؛ اعتبارات منتظر + تأییدشده/ردشده (برای «مرحلهٔ پس از تأیید»، مثل نمونهٔ پنل).
    /// </summary>
    protected static LoanRequestStatus[] DefaultStatuses(ApprovalStage stage) => stage switch
    {
        ApprovalStage.Manager => [LoanRequestStatus.PendingManager],
        ApprovalStage.Credit => [LoanRequestStatus.PendingCredit, LoanRequestStatus.RejectedByCredit, LoanRequestStatus.PendingDigital, LoanRequestStatus.Approved],
        _ => [LoanRequestStatus.PendingDigital],
    };

    /// <summary>
    /// وضعیت‌هایی که فیلتر صف هر مرحله می‌پذیرد. فیلتر خارج از مرحله رد می‌شود تا کاربر شعبه با «stage=Credit&amp;status=1»
    /// درخواست‌های مرحلهٔ مدیر شرکت را نبیند (قاعدهٔ فرایندی ۱).
    /// </summary>
    protected static LoanRequestStatus[] StageStatuses(ApprovalStage stage) => stage switch
    {
        ApprovalStage.Manager => [LoanRequestStatus.PendingManager, LoanRequestStatus.RejectedByManager],
        ApprovalStage.Credit => [.. DefaultStatuses(stage), LoanRequestStatus.RejectedByDigital],
        _ => [LoanRequestStatus.PendingDigital, LoanRequestStatus.Approved, LoanRequestStatus.RejectedByDigital],
    };
}
