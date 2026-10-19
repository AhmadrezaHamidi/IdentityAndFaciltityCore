/* =====================================================================
   مدل دسترسی پنل — «دسترسی به نقش، نقش به کاربر، با دامنه»
   مبنا: شاخهٔ identityservice-base

   هیچ جدول جدیدی برای مجوز ساخته نمی‌شود: زنجیرهٔ موجود
     PanelUser → PanelUserRole → Role → RolePanelMenu → PanelMenu
   نگه داشته می‌شود و فقط ستون‌های لازم اضافه می‌شوند.

   قابل اجرای چندباره (idempotent).
   ===================================================================== */

SET XACT_ABORT ON;
BEGIN TRAN;

/* ---------- ۱) فیلدهای فرم کاربر در سند + نسخهٔ مجوز (معیار ۶) ---------- */

IF COL_LENGTH('dbo.PanelUser', 'FullName')               IS NULL ALTER TABLE dbo.PanelUser ADD FullName               NVARCHAR(60)  NULL;
IF COL_LENGTH('dbo.PanelUser', 'UserCode')               IS NULL ALTER TABLE dbo.PanelUser ADD UserCode               VARCHAR(20)   NULL;
IF COL_LENGTH('dbo.PanelUser', 'BranchCode')             IS NULL ALTER TABLE dbo.PanelUser ADD BranchCode             NVARCHAR(50)  NULL;
IF COL_LENGTH('dbo.PanelUser', 'BranchName')             IS NULL ALTER TABLE dbo.PanelUser ADD BranchName             NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.PanelUser', 'PermissionVersion')      IS NULL ALTER TABLE dbo.PanelUser ADD PermissionVersion      INT NOT NULL CONSTRAINT DF_PanelUser_PermissionVersion DEFAULT (1);
IF COL_LENGTH('dbo.PanelUser', 'LastPermissionChangeAt') IS NULL ALTER TABLE dbo.PanelUser ADD LastPermissionChangeAt DATETIME2(3)  NULL;
GO

/* کد کاربر یکتاست، ولی رکوردهای قدیمی NULL‌اند ← ایندکس فیلترشده */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PanelUser_UserCode' AND object_id = OBJECT_ID('dbo.PanelUser'))
    CREATE UNIQUE INDEX UX_PanelUser_UserCode ON dbo.PanelUser(UserCode) WHERE UserCode IS NOT NULL;
GO

/* ---------- ۲) نقش: دامنهٔ مجاز و سیستمی بودن ---------- */

IF COL_LENGTH('dbo.Role', 'AllowedScopeType') IS NULL ALTER TABLE dbo.Role ADD AllowedScopeType TINYINT NOT NULL CONSTRAINT DF_Role_AllowedScopeType DEFAULT (0);
IF COL_LENGTH('dbo.Role', 'IsSystem')         IS NULL ALTER TABLE dbo.Role ADD IsSystem         BIT     NOT NULL CONSTRAINT DF_Role_IsSystem         DEFAULT (0);
GO

/* ---------- ۳) انتساب نقش: دامنه ---------- */

IF COL_LENGTH('dbo.PanelUserRole', 'ScopeType')            IS NULL ALTER TABLE dbo.PanelUserRole ADD ScopeType            TINYINT NOT NULL CONSTRAINT DF_PanelUserRole_ScopeType DEFAULT (0);
IF COL_LENGTH('dbo.PanelUserRole', 'ScopeId')              IS NULL ALTER TABLE dbo.PanelUserRole ADD ScopeId              VARCHAR(50) NULL;
IF COL_LENGTH('dbo.PanelUserRole', 'CreatedAt')            IS NULL ALTER TABLE dbo.PanelUserRole ADD CreatedAt            DATETIME2(3) NOT NULL CONSTRAINT DF_PanelUserRole_CreatedAt DEFAULT (SYSUTCDATETIME());
IF COL_LENGTH('dbo.PanelUserRole', 'CreatedByPanelUserId') IS NULL ALTER TABLE dbo.PanelUserRole ADD CreatedByPanelUserId INT NULL;
GO

/* یک کاربر می‌تواند یک نقش را در چند دامنه داشته باشد (کاربر شعبه در دو شعبه)،
   ولی نه دو بار در یک دامنه. ایندکس یکتای قدیمی (PanelUserId, RoleId) اگر هست باید برود. */
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PanelUserRole_PanelUserId_RoleOd' AND object_id = OBJECT_ID('dbo.PanelUserRole'))
    DROP INDEX UX_PanelUserRole_PanelUserId_RoleOd ON dbo.PanelUserRole;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_PanelUserRole_User_Role_Scope' AND object_id = OBJECT_ID('dbo.PanelUserRole'))
    CREATE UNIQUE INDEX UX_PanelUserRole_User_Role_Scope
        ON dbo.PanelUserRole(PanelUserId, RoleId, ScopeType, ScopeId);
GO

/* ---------- ۴) PanelMenu به کاتالوگ دو سطحی مجوز ---------- */

IF COL_LENGTH('dbo.PanelMenu', 'PermissionKey') IS NULL ALTER TABLE dbo.PanelMenu ADD PermissionKey VARCHAR(150) NULL;
IF COL_LENGTH('dbo.PanelMenu', 'Kind')          IS NULL ALTER TABLE dbo.PanelMenu ADD Kind          TINYINT NOT NULL CONSTRAINT DF_PanelMenu_Kind DEFAULT (0);
IF COL_LENGTH('dbo.PanelMenu', 'SortOrder')     IS NULL ALTER TABLE dbo.PanelMenu ADD SortOrder     INT     NOT NULL CONSTRAINT DF_PanelMenu_SortOrder DEFAULT (0);
GO

/* عمداً یکتا نیست: دو ردیف می‌توانند به یک کلید اشاره کنند (مثلاً دو تب که با یک مجوز باز می‌شوند).
   فقط برای جست‌وجوی سریع در فیلتر. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PanelMenu_PermissionKey' AND object_id = OBJECT_ID('dbo.PanelMenu'))
    CREATE INDEX IX_PanelMenu_PermissionKey ON dbo.PanelMenu(PermissionKey) WHERE PermissionKey IS NOT NULL;
GO

/* ---------- ۵) لاگ امنیتی («گزارش تغییرات» دستهٔ کاربران) ---------- */

IF OBJECT_ID('dbo.PanelSecurityAuditLog') IS NULL
BEGIN
    CREATE TABLE dbo.PanelSecurityAuditLog
    (
        Id               BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PanelSecurityAuditLog PRIMARY KEY,
        [Action]         VARCHAR(60)   NOT NULL,
        ActorPanelUserId INT           NULL,
        TargetType       VARCHAR(60)   NOT NULL,
        TargetId         VARCHAR(50)   NULL,
        BeforeJson       NVARCHAR(MAX) NULL,
        AfterJson        NVARCHAR(MAX) NULL,
        Ip               VARCHAR(45)   NULL,
        UserAgent        NVARCHAR(300) NULL,
        CreatedAt        DATETIME2(3)  NOT NULL CONSTRAINT DF_PanelSecurityAuditLog_CreatedAt DEFAULT (SYSUTCDATETIME())
    );

    CREATE INDEX IX_PanelSecurityAuditLog_CreatedAt ON dbo.PanelSecurityAuditLog(CreatedAt);
    CREATE INDEX IX_PanelSecurityAuditLog_Target    ON dbo.PanelSecurityAuditLog(TargetType, TargetId);
END
GO

/* ---------- ۶) ۵ نقش سند ---------- */
/* AllowedScopeType: 0 سراسری، 1 شعبه، 2 شرکت */

MERGE dbo.[Role] AS t
USING (VALUES
    (N'admin',           N'ادمین (مدیر سامانه)',   0),
    (N'digital_branch',  N'شعبهٔ دیجیتال',          0),
    (N'branch_user',     N'کاربر شعبه',             1),
    (N'company_manager', N'مدیر شرکت',              2),
    (N'company_hr',      N'منابع انسانی شرکت',      2)
) AS s(Title, PersianTitle, AllowedScopeType)
ON t.Title = s.Title
WHEN MATCHED THEN
    UPDATE SET t.IsSystem = 1, t.AllowedScopeType = s.AllowedScopeType, t.IsActive = 1
WHEN NOT MATCHED THEN
    INSERT (Title, PersianTitle, IsActive, [Kind], AllowedScopeType, IsSystem, CreatedAt)
    VALUES (s.Title, s.PersianTitle, 1, 0, s.AllowedScopeType, 1, SYSUTCDATETIME());
GO

COMMIT;
GO

/* =====================================================================
   مانده (عمداً اینجا نیست، چون به تصمیم شما بسته است):

   الف) کلیدهای مجوز: ردیف‌های PanelMenu با Kind و PermissionKey.
        ساختار: تب (Kind=0، بدون کلید) و زیرش مشاهده (Kind=1) و عملیات (Kind=2).
        مثال برای «تأیید اعتبارات»:
          INSERT dbo.PanelMenu (ParentId, Title, Kind, PermissionKey, IsActive, ...)
          VALUES (@tab, N'مشاهدهٔ صف اعتبارات', 1, 'facility.credit-approval.view',    1, ...),
                 (@tab, N'تأیید اعتبارات',      2, 'facility.credit-approval.approve', 1, ...),
                 (@tab, N'رد اعتبارات',          2, 'facility.credit-approval.reject',  1, ...);

   ب) ماتریس پیش‌فرض: ردیف‌های RolePanelMenu برای ۵ نقش، طبق «ماتریس جامع دسترسی» سند.

   این دو را وقتی فهرست نهایی تب/سرویس‌ها را تأیید کردید می‌نویسم — چون باید مو‌به‌مو با
   ماتریس سند بخواند و یک ردیف اشتباه یعنی یک نفر دسترسی‌ای دارد که نباید.
   ===================================================================== */
