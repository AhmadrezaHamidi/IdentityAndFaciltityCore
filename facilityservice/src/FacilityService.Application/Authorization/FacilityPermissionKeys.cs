namespace FacilityService.Application.Authorization;

/// <summary>
/// کلیدهای مجوز Facility. باید دقیقاً با IdentityService.Domain.Authorization.FacilityPermissionKeys یکی باشد
/// (تست FacilityPermissionKeysParityTests این را چک می‌کند). بهتر است هر دو از یک پکیج مشترک بیایند.
/// </summary>
public static class FacilityPermissionKeys
{
    public const string BasicInfoView = "facility.basic-info.view";
    public const string BasicInfoEdit = "facility.basic-info.edit";

    public const string CompaniesView = "facility.companies.view";
    public const string CompaniesCreate = "facility.companies.create";
    public const string CompaniesEdit = "facility.companies.edit";
    public const string CompaniesDelete = "facility.companies.delete";
    public const string CompaniesBalanceEdit = "facility.companies.balance.edit";
    public const string CompaniesDocumentsView = "facility.companies.documents.view";
    public const string CompaniesDocumentsUpload = "facility.companies.documents.upload";
    public const string CompaniesDocumentsDelete = "facility.companies.documents.delete";
    public const string CompaniesContractCancel = "facility.companies.contract.cancel";

    public const string PersonnelView = "facility.personnel.view";
    public const string PersonnelCreate = "facility.personnel.create";
    public const string PersonnelEdit = "facility.personnel.edit";
    public const string PersonnelDelete = "facility.personnel.delete";
    public const string PersonnelUpload = "facility.personnel.upload";
    public const string PersonnelApprove = "facility.personnel.approve";

    public const string LoansView = "facility.loans.view";
    public const string LoansCreditView = "facility.loans.credit.view";
    public const string LoansCreate = "facility.loans.create";
    public const string LoansMembersEdit = "facility.loans.members.edit";
    public const string LoansCancel = "facility.loans.cancel";

    public const string ManagerApprovalView = "facility.manager-approval.view";
    public const string ManagerApprovalApprove = "facility.manager-approval.approve";
    public const string ManagerApprovalReject = "facility.manager-approval.reject";

    public const string CreditApprovalView = "facility.credit-approval.view";
    public const string CreditApprovalApprove = "facility.credit-approval.approve";
    public const string CreditApprovalReject = "facility.credit-approval.reject";
    public const string CreditApprovalStage = "facility.credit-approval.stage";
    public const string DigitalApprovalDecide = "facility.digital-approval.decide";

    public const string DashboardView = "facility.dashboard.view";
    public const string DashboardExport = "facility.dashboard.export";

    public const string AuditView = "facility.audit.view";
    public const string AuditExport = "facility.audit.export";
    public const string AuditRestore = "facility.audit.restore";
}
