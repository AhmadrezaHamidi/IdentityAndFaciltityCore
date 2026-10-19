namespace FacilityService.Domain.Enums;

public enum ContractType : byte { QardHasan = 1, Murabaha = 2 }

public enum CompanyStatus : byte { Active = 1, Cancelled = 2, Expired = 3, Deleted = 4 }

public enum PersonnelStatus : byte
{
    /// <summary>در انتظار تأیید مدیر شرکت</summary>
    PendingManager = 1,
    Approved = 2,
    Rejected = 3,
    Deleted = 4,
}

public enum LoanRequestStatus : byte
{
    PendingManager = 1,
    RejectedByManager = 2,
    PendingCredit = 3,
    RejectedByCredit = 4,
    PendingDigital = 5,
    RejectedByDigital = 6,
    Approved = 7,
    Cancelled = 8,
}

/// <summary>مرحلهٔ پس از تأیید (نمونهٔ پنل؛ در سند نیست ⚠ Q17).</summary>
public enum PostApprovalStage : byte
{
    FileOpenedAtBranch = 1,      // تشکیل پرونده در شعبه
    ReceivedFromOmidBank = 2,    // دریافت تسهیلات از امید بانک
    ReceivedAtBranch = 3,        // دریافت تسهیلات در شعبه
    RemoteGrantDenied = 4,       // عدم اعطای تسهیلات غیرحضوری (دلیل الزامی)
}

public enum AllocationMode : byte
{
    /// <summary>مبلغ یکسان برای همه (فیلد ۱۵۱: single)</summary>
    Single = 1,
    /// <summary>مبلغ جدا برای هر نفر (variable)</summary>
    Variable = 2,
}

public enum EligibilityStatus : byte
{
    /// <summary>هنوز به مهلت معرفی پرسنل نرسیده</summary>
    IntroductionNotYetAllowed = 1,
    /// <summary>امروز روز مجاز انتقال اعتبار نیست (روز افتتاح حساب)</summary>
    NotTransferDay = 2,
    /// <summary>شرکت مجاز به ثبت درخواست وام در این ماه است</summary>
    Allowed = 3,
    /// <summary>قرارداد لغو یا منقضی شده</summary>
    ContractInactive = 4,
}

public enum ApprovalStage : byte { Manager = 1, Credit = 2, Digital = 3 }

/// <summary>انواع فهرست‌های داینامیک اطلاعات پایه.</summary>
public enum ReasonType : byte
{
    /// <summary>دلایل لغو قرارداد شرکت</summary>
    ContractCancel = 1,
    /// <summary>دلایل رد درخواست وام (تأیید اعتبارات و مدیر شرکت)</summary>
    LoanReject = 2,
    /// <summary>دلایل عدم اعطای تسهیلات غیرحضوری (مرحلهٔ پس از تأیید)</summary>
    RemoteGrantDenied = 3,
    /// <summary>دلایل لغو درخواست وام (قاعدهٔ فرایندی ۸)</summary>
    LoanCancel = 4,
    /// <summary>دلایل رد پرسنل معرفی‌شده</summary>
    PersonnelReject = 5,
    /// <summary>نوع مستند میانگین موجودی (فیلد ۲۶۱: تصویر بنکو، نامه، ...)</summary>
    DocumentType = 6,
}

public enum ChangeLogCategory : byte { Company = 1, Personnel = 2, Loan = 3, User = 4 }

public enum HintLevel : byte { Info = 1, Success = 2, Warning = 3, Error = 4 }
