/* =====================================================================
   کاتالوگ مجوزها + ماتریس پیش‌فرض ۵ نقش
   مبنا: «ماتریس جامع دسترسی بخش‌ها و قابلیت‌ها» سند طرح نگین میثاق

   ساختار: هر تب یک ردیف Kind=1 با کلید «.view» و Url است؛ عملیات‌های آن تب
   ردیف‌های فرزند Kind=2 بدون Url هستند. پس:
     - تب در منو دیده می‌شود ⇔ کاربر کلید view آن را دارد  (معیار پذیرش ۱)
     - دکمه‌ها جدا کنترل می‌شوند                            («بخش و سرویس‌های آن بخش»)

   کلیدها عیناً با FacilityPermissionKeys و AdminPermissionKeys یکی هستند تا اگر
   سرویس Facility اضافه شد، چیزی تغییر نکند. کلید منتشرشده هرگز تغییر نام نمی‌دهد.

   پیش‌نیاز: 01_panel_authorization.sql
   قابل اجرای چندباره (idempotent).
   ===================================================================== */

SET XACT_ABORT ON;
BEGIN TRAN;

IF COL_LENGTH('dbo.PanelMenu', 'PermissionKey') IS NULL
    THROW 50010, N'اول 01_panel_authorization.sql را اجرا کن.', 1;

/* ---------------------------------------------------------------------
   الف) کاتالوگ
   Kind: 1 = تب/مشاهده (Url دارد، در منو دیده می‌شود) ، 2 = عملیات (در منو نیست)
   --------------------------------------------------------------------- */

DECLARE @cat TABLE
(
    [Key]      VARCHAR(150) NOT NULL PRIMARY KEY,
    ParentKey  VARCHAR(150) NULL,
    Kind       TINYINT      NOT NULL,
    Title      NVARCHAR(150) NOT NULL,
    Url        VARCHAR(200) NULL,
    SortOrder  INT          NOT NULL
);

INSERT @cat ([Key], ParentKey, Kind, Title, Url, SortOrder) VALUES
-- اطلاعات پایه طرح ----------------------------------------------------
 ('facility.basic-info.view',            NULL,                         1, N'اطلاعات پایه طرح',                       '/basic-info',      100),
 ('facility.basic-info.edit',            'facility.basic-info.view',    2, N'ویرایش پارامترها، نرخ‌ها و دلایل',       NULL,               101),

-- مدیریت شرکت‌ها -------------------------------------------------------
 ('facility.companies.view',             NULL,                         1, N'مدیریت شرکت‌ها',                          '/companies',       200),
 ('facility.companies.create',           'facility.companies.view',     2, N'ثبت شرکت جدید',                          NULL,               201),
 ('facility.companies.edit',             'facility.companies.view',     2, N'ویرایش شرکت',                            NULL,               202),
 ('facility.companies.delete',           'facility.companies.view',     2, N'حذف شرکت',                               NULL,               203),
 ('facility.companies.balance.edit',     'facility.companies.view',     2, N'ثبت/ویرایش میانگین موجودی',              NULL,               204),
 ('facility.companies.documents.view',   'facility.companies.view',     2, N'مشاهده و دانلود مستندات میانگین',        NULL,               205),
 ('facility.companies.documents.upload', 'facility.companies.view',     2, N'پیوست مستندات میانگین',                  NULL,               206),
 ('facility.companies.documents.delete', 'facility.companies.view',     2, N'حذف مستند میانگین',                      NULL,               207),
 ('facility.companies.contract.cancel',  'facility.companies.view',     2, N'لغو قرارداد شرکت',                       NULL,               208),

-- پرسنل شرکت ----------------------------------------------------------
 ('facility.personnel.view',             NULL,                         1, N'پرسنل شرکت',                              '/personnel',       300),
 ('facility.personnel.create',           'facility.personnel.view',     2, N'معرفی پرسنل',                            NULL,               301),
 ('facility.personnel.edit',             'facility.personnel.view',     2, N'ویرایش پرسنل',                           NULL,               302),
 ('facility.personnel.delete',           'facility.personnel.view',     2, N'حذف پرسنل (با توضیح و فایل)',            NULL,               303),
 ('facility.personnel.upload',           'facility.personnel.view',     2, N'آپلود اکسل پرسنل',                       NULL,               304),
 ('facility.personnel.approve',          'facility.personnel.view',     2, N'تأیید/رد افراد معرفی‌شده',                NULL,               305),

-- تخصیص وام -----------------------------------------------------------
 ('facility.loans.view',                 NULL,                         1, N'تخصیص وام',                               '/loan-allocation', 400),
 ('facility.loans.credit.view',          'facility.loans.view',         2, N'مشاهدهٔ اعتبار طرح‌ها و میانگین‌ها',       NULL,               401),
 ('facility.loans.create',               'facility.loans.view',         2, N'ثبت درخواست وام',                        NULL,               402),
 ('facility.loans.members.edit',         'facility.loans.view',         2, N'افزودن/حذف فرد از درخواست',              NULL,               403),
 ('facility.loans.cancel',               'facility.loans.view',         2, N'لغو درخواست قبل از تأیید اعتبارات',      NULL,               404),

-- تأیید مدیر شرکت -----------------------------------------------------
 ('facility.manager-approval.view',      NULL,                         1, N'تأیید مدیر شرکت',                         '/manager-approval', 500),
 ('facility.manager-approval.approve',   'facility.manager-approval.view', 2, N'تأیید درخواست‌ها (مدیر شرکت)',        NULL,               501),
 ('facility.manager-approval.reject',    'facility.manager-approval.view', 2, N'رد درخواست‌ها (مدیر شرکت)',           NULL,               502),

-- تأیید اعتبارات + شعبهٔ دیجیتال ---------------------------------------
 ('facility.credit-approval.view',       NULL,                         1, N'تأیید اعتبارات',                          '/credit-approval', 600),
 ('facility.credit-approval.approve',    'facility.credit-approval.view', 2, N'تأیید اعتبارات',                       NULL,               601),
 ('facility.credit-approval.reject',     'facility.credit-approval.view', 2, N'رد اعتبارات',                          NULL,               602),
 ('facility.credit-approval.stage',      'facility.credit-approval.view', 2, N'تغییر مرحلهٔ پس از تأیید',             NULL,               603),
 ('facility.digital-approval.decide',    'facility.credit-approval.view', 2, N'تصمیم مرحلهٔ شعبهٔ دیجیتال',           NULL,               604),

-- داشبورد -------------------------------------------------------------
 ('facility.dashboard.view',             NULL,                         1, N'داشبورد مدیریتی',                         '/dashboard',       700),
 ('facility.dashboard.export',           'facility.dashboard.view',     2, N'خروجی CSV داشبورد',                      NULL,               701),

-- گزارش تغییرات -------------------------------------------------------
 ('facility.audit.view',                 NULL,                         1, N'گزارش تغییرات',                           '/change-log',      800),
 ('facility.audit.export',               'facility.audit.view',         2, N'خروجی گزارش تغییرات',                    NULL,               801),
 ('facility.audit.restore',              'facility.audit.view',         2, N'بازگردانی عملیات',                       NULL,               802),

-- مدیریت کاربران ------------------------------------------------------
 ('admin.users.read',                    NULL,                         1, N'مدیریت کاربران',                          '/user-management', 900),
 ('admin.users.write',                   'admin.users.read',            2, N'ساخت و ویرایش کاربر پنل',                NULL,               901),
 ('admin.users.assign',                  'admin.users.read',            2, N'انتساب نقش با دامنه',                    NULL,               902),
 ('admin.users.reset-password',          'admin.users.read',            2, N'بازنشانی رمز کاربر',                     NULL,               903),
 ('admin.users.revoke-sessions',         'admin.users.read',            2, N'خروج اجباری کاربر',                      NULL,               904),
 ('admin.roles.read',                    'admin.users.read',            2, N'مشاهدهٔ نقش‌ها',                          NULL,               905),
 ('admin.roles.write',                   'admin.users.read',            2, N'تعریف و ویرایش نقش',                     NULL,               906),
 ('admin.groups.read',                   'admin.users.read',            2, N'مشاهدهٔ گروه‌ها',                        NULL,               907),
 ('admin.groups.write',                  'admin.users.read',            2, N'تعریف و ویرایش گروه',                    NULL,               908),
 ('admin.permissions.read',              'admin.users.read',            2, N'مشاهدهٔ کاتالوگ مجوزها',                 NULL,               909),
 ('admin.permissions.write',             'admin.users.read',            2, N'ویرایش کاتالوگ مجوزها',                  NULL,               910),
 ('admin.audit.read',                    'admin.users.read',            2, N'لاگ امنیتی کاربران',                     NULL,               911);

/* ردیف‌های ریشه (تب‌ها) اول، تا ParentId فرزندها قابل تنظیم باشد */
MERGE dbo.PanelMenu AS t
USING (SELECT * FROM @cat WHERE ParentKey IS NULL) AS s
ON t.PermissionKey = s.[Key]
WHEN MATCHED THEN
    UPDATE SET t.Title = s.Title, t.Url = s.Url, t.[Kind] = s.Kind,
               t.SortOrder = s.SortOrder, t.IsActive = 1
WHEN NOT MATCHED THEN
    INSERT (ParentId, Title, Url, IsRootMenu, IsInternalMenu, CreationTime, IsActive, PermissionKey, [Kind], SortOrder)
    VALUES (NULL, s.Title, s.Url, 1, 0, SYSUTCDATETIME(), 1, s.[Key], s.Kind, s.SortOrder);

MERGE dbo.PanelMenu AS t
USING (
    SELECT c.[Key], c.Title, c.Url, c.Kind, c.SortOrder, p.Id AS ParentId
    FROM @cat c
    JOIN dbo.PanelMenu p ON p.PermissionKey = c.ParentKey
    WHERE c.ParentKey IS NOT NULL
) AS s
ON t.PermissionKey = s.[Key]
WHEN MATCHED THEN
    UPDATE SET t.Title = s.Title, t.Url = s.Url, t.[Kind] = s.Kind,
               t.SortOrder = s.SortOrder, t.ParentId = s.ParentId, t.IsActive = 1
WHEN NOT MATCHED THEN
    INSERT (ParentId, Title, Url, IsRootMenu, IsInternalMenu, CreationTime, IsActive, PermissionKey, [Kind], SortOrder)
    VALUES (s.ParentId, s.Title, s.Url, 0, 0, SYSUTCDATETIME(), 1, s.[Key], s.Kind, s.SortOrder);

/* ---------------------------------------------------------------------
   ب) ماتریس پیش‌فرض
   A = ادمین ، D = شعبهٔ دیجیتال ، B = کاربر شعبه ، M = مدیر شرکت ، H = منابع انسانی
   ۱ = دارد ، ۰ = ندارد. «R» سند یعنی فقط کلیدهای view ← ستون D برای عملیات صفر است.
   --------------------------------------------------------------------- */

DECLARE @m TABLE ([Key] VARCHAR(150) NOT NULL PRIMARY KEY, A BIT, D BIT, B BIT, M BIT, H BIT);

INSERT @m ([Key], A, D, B, M, H) VALUES
-- اطلاعات پایه: «✓ ویرایش کامل» فقط ادمین؛ بقیه «—»
 ('facility.basic-info.view',            1, 0, 0, 0, 0),
 ('facility.basic-info.edit',            1, 0, 0, 0, 0),

-- مدیریت شرکت‌ها: ادمین ✓ ، دیجیتال R ، کاربر شعبه ✓ (شعبهٔ خود) ، شرکت —
 ('facility.companies.view',             1, 1, 1, 0, 0),
 ('facility.companies.create',           1, 0, 1, 0, 0),
 ('facility.companies.edit',             1, 0, 1, 0, 0),
 ('facility.companies.delete',           1, 0, 0, 0, 0),   -- «حذف شرکت فقط ادمین»
 ('facility.companies.balance.edit',     1, 0, 1, 0, 0),
 ('facility.companies.documents.view',   1, 1, 1, 0, 0),   -- دیجیتال: «R (مشاهده/دانلود)»
 ('facility.companies.documents.upload', 1, 0, 1, 0, 0),
 ('facility.companies.documents.delete', 1, 0, 0, 0, 0),   -- «حذف مستند فقط ادمین»
 ('facility.companies.contract.cancel',  1, 0, 1, 0, 0),

-- پرسنل: ادمین ✓ ، دیجیتال R ، شعبه — ، مدیر ✓ ، منابع انسانی ✓
 ('facility.personnel.view',             1, 1, 0, 1, 1),
 ('facility.personnel.create',           1, 0, 0, 1, 1),
 ('facility.personnel.edit',             1, 0, 0, 1, 1),
 ('facility.personnel.delete',           1, 0, 0, 1, 1),
 ('facility.personnel.upload',           1, 0, 0, 1, 1),
 ('facility.personnel.approve',          1, 0, 0, 1, 0),   -- «تأیید/رد فقط مدیر شرکت»

-- تخصیص وام
 ('facility.loans.view',                 1, 1, 1, 1, 1),
 ('facility.loans.credit.view',          1, 1, 1, 1, 1),   -- ⚠ Q1 (پایین را بخوان)
 ('facility.loans.create',               1, 0, 0, 1, 1),
 ('facility.loans.members.edit',         1, 0, 0, 1, 1),
 ('facility.loans.cancel',               1, 0, 0, 1, 0),   -- «لغو: ✓ مدیر ، — منابع انسانی»

-- تأیید مدیر شرکت
 ('facility.manager-approval.view',      1, 1, 0, 1, 0),
 ('facility.manager-approval.approve',   1, 0, 0, 1, 0),
 ('facility.manager-approval.reject',    1, 0, 0, 1, 0),

-- تأیید اعتبارات: ادمین ✓ ، دیجیتال R ، کاربر شعبه ✓
 ('facility.credit-approval.view',       1, 1, 1, 0, 0),
 ('facility.credit-approval.approve',    1, 0, 1, 0, 0),
 ('facility.credit-approval.reject',     1, 0, 1, 0, 0),
 ('facility.credit-approval.stage',      1, 0, 1, 0, 0),
-- تأیید شعبهٔ دیجیتال: تنها عملیاتی که دیجیتال ✓ دارد
 ('facility.digital-approval.decide',    1, 1, 1, 0, 0),

-- داشبورد: منابع انسانی «—» (جملهٔ صریح سند)
 ('facility.dashboard.view',             1, 1, 1, 1, 0),
 ('facility.dashboard.export',           1, 0, 1, 1, 0),

-- گزارش تغییرات: بازگردانی فقط ادمین
 ('facility.audit.view',                 1, 1, 1, 1, 0),
 ('facility.audit.export',               1, 1, 1, 1, 0),
 ('facility.audit.restore',              1, 0, 0, 0, 0),

-- مدیریت کاربران: ⚠ Q2 (پایین را بخوان)
 ('admin.users.read',                    1, 1, 0, 0, 0),
 ('admin.users.write',                   1, 0, 0, 0, 0),
 ('admin.users.assign',                  1, 0, 0, 0, 0),
 ('admin.users.reset-password',          1, 0, 0, 0, 0),
 ('admin.users.revoke-sessions',         1, 0, 0, 0, 0),
 ('admin.roles.read',                    1, 1, 0, 0, 0),
 ('admin.roles.write',                   1, 0, 0, 0, 0),
 ('admin.groups.read',                   1, 1, 0, 0, 0),
 ('admin.groups.write',                  1, 0, 0, 0, 0),
 ('admin.permissions.read',              1, 1, 0, 0, 0),
 ('admin.permissions.write',             1, 0, 0, 0, 0),
 ('admin.audit.read',                    1, 1, 0, 0, 0);

/* درج ردیف‌های RolePanelMenu طبق ماتریس */
;WITH flat AS (
    SELECT v.RoleTitle, m.[Key]
    FROM @m m
    CROSS APPLY (VALUES
        (N'admin',           m.A),
        (N'digital_branch',  m.D),
        (N'branch_user',     m.B),
        (N'company_manager', m.M),
        (N'company_hr',      m.H)
    ) v(RoleTitle, Has)
    WHERE v.Has = 1
)
MERGE dbo.RolePanelMenu AS t
USING (
    SELECT r.Id AS RoleId, pm.Id AS PanelMenuId
    FROM flat f
    JOIN dbo.[Role]      r  ON r.Title = f.RoleTitle AND r.IsSystem = 1
    JOIN dbo.PanelMenu   pm ON pm.PermissionKey = f.[Key]
) AS s
ON t.RoleId = s.RoleId AND t.PanelMenuId = s.PanelMenuId
WHEN NOT MATCHED THEN
    INSERT (RoleId, PanelMenuId) VALUES (s.RoleId, s.PanelMenuId);

/* ردیف‌هایی که ماتریس نمی‌خواهد ولی در دیتابیس هستند، برای ۵ نقش سیستمی پاک می‌شوند
   تا اجرای دوباره، ماتریس را دقیقاً به حالت سند برگرداند.
   ⚠ اگر ادمین عمداً دسترسی‌ای به یک نقش سیستمی اضافه کرده، این آن را برمی‌دارد. */
;WITH wanted AS (
    SELECT r.Id AS RoleId, pm.Id AS PanelMenuId
    FROM @m m
    CROSS APPLY (VALUES
        (N'admin', m.A), (N'digital_branch', m.D), (N'branch_user', m.B),
        (N'company_manager', m.M), (N'company_hr', m.H)
    ) v(RoleTitle, Has)
    JOIN dbo.[Role]    r  ON r.Title = v.RoleTitle AND r.IsSystem = 1
    JOIN dbo.PanelMenu pm ON pm.PermissionKey = m.[Key]
    WHERE v.Has = 1
)
DELETE rpm
FROM dbo.RolePanelMenu rpm
JOIN dbo.[Role] r ON r.Id = rpm.RoleId AND r.IsSystem = 1
WHERE NOT EXISTS (SELECT 1 FROM wanted w WHERE w.RoleId = rpm.RoleId AND w.PanelMenuId = rpm.PanelMenuId);

/* مجوز نقش‌ها عوض شد ← نشست‌های فعلی باید باطل شوند (معیار پذیرش ۶) */
UPDATE u
SET u.PermissionVersion = u.PermissionVersion + 1,
    u.LastPermissionChangeAt = SYSUTCDATETIME()
FROM dbo.PanelUser u
WHERE EXISTS (
    SELECT 1 FROM dbo.PanelUserRole ur
    JOIN dbo.[Role] r ON r.Id = ur.RoleId AND r.IsSystem = 1
    WHERE ur.PanelUserId = u.Id
);

COMMIT;
GO

/* =====================================================================
   دو تناقض سند — این‌ها را خودتان تأیید کنید

   ⚠ Q1 — «مشاهدهٔ اعتبار طرح‌ها» برای منابع انسانی
     ماتریس جامع: «—»
     ولی شرح نقش منابع انسانی بند ۲: «تخصیص وام: مشاهده اعتبار طرح‌ها طبق تفاهم‌نامه»
     و US-E4: «می‌خواهم ... اعتبار هر طرح و میانگین‌ها را ببینم»
     ⇒ اینجا ۱ گذاشته شد، چون بدون دیدن اعتبار نمی‌تواند درخواست ثبت کند
       (و ثبت درخواست در ماتریس برایش ✓ است).
     اگر ماتریس درست است:  UPDATE ... SET H = 0 برای facility.loans.credit.view

   ⚠ Q2 — «مدیریت کاربران» برای شعبهٔ دیجیتال
     ماتریس جامع: «✓ (ویرایش/افزودن/غیرفعال‌سازی)»
     ولی تعریف نقش: «در حالت پیش‌فرض نقش آن فقط مشاهده است و امکان ایجاد، ویرایش،
     حذف، تأیید/رد یا هر تغییر دیگری ندارد»
     و US-C4: «ماتریس دسترسی و فهرست کاربران را ... فقط مشاهده کنم»
     ⇒ اینجا فقط کلیدهای read داده شد (محدودتر)، چون دو جای سند «فقط مشاهده» می‌گویند
       و در امنیت، تفسیر محدودتر پیش‌فرض درست است.
     اگر ماتریس درست است: admin.users.write / assign / reset-password /
       revoke-sessions / roles.write / groups.write را برای D یک کنید.

   این دو ردیف تنها جاهایی هستند که سند با خودش نمی‌خواند؛ بقیهٔ ۴۳ ردیف
   مستقیماً از ماتریس آمده‌اند.
   ===================================================================== */
