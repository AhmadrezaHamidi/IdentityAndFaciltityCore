/* =====================================================================
   Facility — دادهٔ تست (فقط دیتابیس توسعه/تست، نه عملیاتی)
   ---------------------------------------------------------------------
   پیش‌نیاز: facility_panel_migrations.sql (جداول + طرح نگین میثاق + طرح‌ها و دلایل)
   ترتیب: اول همین فایل روی دیتابیس Facility، بعد
          identityservice/Database/08_test_users.sql روی دیتابیس Identity (کاربران و نقش‌های تست).

   همان دادهٔ نمونهٔ کد (FacilityPanelSampleData) که تست‌های خودکار و فرانت با آن نوشته شده‌اند:
     ۳ شرکت   آلفا و بتا در شعبهٔ 150002، گاما در 150003؛ قرارداد ۳ ماه شمسی پیش
              روز افتتاح حساب آلفا و بتا = روز امروز (ثبت وام امروز مجاز است)، گاما روز دیگر (مجاز نیست)
     ۱۴ پرسنل تأییدشده، در انتظار تأیید مدیر، ردشده
     ۸ درخواست وام، یکی در هر وضعیت:
              در انتظار مدیر، در انتظار اعتبارات (۲)، تأییدشده + مرحلهٔ پس از تأیید، تأییدشده،
              لغوشده، ردشده توسط اعتبارات، ردشده توسط مدیر
     ۹ ردیف گزارش تغییرات

   idempotent: اگر شرکت آلفا (شناسهٔ ملی 10101010101) هست، کاری نمی‌کند.
   پاک‌کردن دادهٔ تست: بخش «پاک‌سازی» در انتهای فایل (کامنت).

   ⚠ «روز مجاز انتقال» به روز امروز وابسته است: فردا ثبت وام برای آلفا خطای «روز مجاز انتقال» می‌دهد.
     برای تست دوباره، پاک‌سازی را اجرا کنید و این فایل را دوباره بزنید.
   ===================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/* ── تبدیل میلادی به شمسی (موقت؛ آخر فایل حذف می‌شود) ── */
IF OBJECT_ID('panel.TestData_ToJalali') IS NOT NULL DROP FUNCTION panel.TestData_ToJalali;
GO
CREATE FUNCTION panel.TestData_ToJalali (@d DATE) RETURNS TABLE AS RETURN
SELECT jy = c.jy2 + CASE WHEN c.r2 > 365 THEN (c.r2 - 1) / 365 ELSE 0 END,
       jm = CASE WHEN d.r3 < 186 THEN 1 + d.r3 / 31 ELSE 7 + (d.r3 - 186) / 30 END,
       jd = CASE WHEN d.r3 < 186 THEN 1 + d.r3 % 31 ELSE 1 + (d.r3 - 186) % 30 END
FROM (SELECT gy = YEAR(@d), gm = MONTH(@d), gd = DAY(@d)) g
CROSS APPLY (SELECT gy2 = g.gy + CASE WHEN g.gm > 2 THEN 1 ELSE 0 END) g2
CROSS APPLY (SELECT days = 355666 + 365 * g.gy + (g2.gy2 + 3) / 4 - (g2.gy2 + 99) / 100 + (g2.gy2 + 399) / 400 + g.gd
                         + CHOOSE(g.gm, 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334)) a
CROSS APPLY (SELECT jy1 = -1595 + 33 * (a.days / 12053), r1 = a.days % 12053) b
CROSS APPLY (SELECT jy2 = b.jy1 + 4 * (b.r1 / 1461), r2 = b.r1 % 1461) c
CROSS APPLY (SELECT r3 = CASE WHEN c.r2 > 365 THEN (c.r2 - 1) % 365 ELSE c.r2 END) d;
GO

IF EXISTS (SELECT 1 FROM panel.Companies WHERE NationalId = '10101010101' AND DeletedAt IS NULL)
BEGIN
    PRINT N'دادهٔ تست از قبل هست (شرکت آلفا). برای ساخت دوباره، اول بخش پاک‌سازی را اجرا کنید.';
    RETURN;
END;

IF NOT EXISTS (SELECT 1 FROM panel.SchemeSettings)
    THROW 50101, N'اول facility_panel_migrations.sql را اجرا کنید (طرح نگین میثاق پیدا نشد).', 1;

BEGIN TRAN;

DECLARE @now DATETIME2 = SYSUTCDATETIME();
DECLARE @today DATE = CAST(SYSDATETIME() AS DATE);

/* ── تاریخ‌ها: قرارداد = ۳ ماه شمسی پیش با همان روز (اگر ماه کوتاه‌تر است، آخرین روزش) ── */
DECLARE @ty INT, @tm INT, @td INT;
SELECT @ty = jy, @tm = jm, @td = jd FROM panel.TestData_ToJalali(@today);

DECLARE @idx INT = @ty * 12 + (@tm - 1) - 3;
DECLARE @cy INT = @idx / 12, @cm INT = @idx % 12 + 1;

DECLARE @days TABLE (d DATE PRIMARY KEY, jy INT, jm INT, jd INT);
INSERT @days SELECT x.d, j.jy, j.jm, j.jd
FROM (SELECT d = DATEADD(DAY, -n, @today) FROM (SELECT TOP (130) n = ROW_NUMBER() OVER (ORDER BY (SELECT 1)) - 1 FROM sys.all_objects) t) x
CROSS APPLY panel.TestData_ToJalali(x.d) j;

DECLARE @contract DATE = (SELECT MAX(d) FROM @days WHERE jy = @cy AND jm = @cm AND jd <= @td);
DECLARE @cd INT = (SELECT jd FROM @days WHERE d = @contract);
DECLARE @otherDay DATE = (SELECT d FROM @days WHERE jy = @cy AND jm = @cm AND jd = @cd % 28 + 1);   -- روز افتتاح گاما

-- ۱۲ ماه قرارداد (کلید «yyyy/MM» شمسی، مثل Jalali.ContractMonths)
DECLARE @months TABLE (i INT PRIMARY KEY, MonthKey VARCHAR(7));
INSERT @months SELECT i, FORMAT((@cy * 12 + (@cm - 1) + i) / 12, '0000') + '/' + FORMAT((@cy * 12 + (@cm - 1) + i) % 12 + 1, '00')
FROM (VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11)) v(i);

/* ── طرح و طرح‌های اعتباری ── */
DECLARE @scheme INT = (SELECT TOP (1) SchemeId FROM panel.SchemeSettings ORDER BY SchemeId);
DECLARE @qard12 INT = (SELECT Id FROM panel.SchemePlans WHERE SchemeId = @scheme AND ContractType = 1 AND TermMonths = 12 AND IsActive = 1);
DECLARE @murabaha12 INT = (SELECT Id FROM panel.SchemePlans WHERE SchemeId = @scheme AND ContractType = 2 AND TermMonths = 12 AND Rate = 5 AND IsActive = 1);
IF @qard12 IS NULL OR @murabaha12 IS NULL THROW 50102, N'طرح قرض‌الحسنه ۱۲ ماهه یا مرابحه ۱۲ ماهه ۵٪ پیدا نشد.', 1;

DECLARE @loanReject NVARCHAR(200) = (SELECT TOP (1) Title FROM panel.Reasons WHERE Type = 2 AND IsActive = 1 ORDER BY Id);
DECLARE @loanCancel NVARCHAR(200) = (SELECT TOP (1) Title FROM panel.Reasons WHERE Type = 4 AND IsActive = 1 ORDER BY Id);

/* ── ثبت‌کننده‌ها (همان کاربران تست Identity؛ شناسه فقط برای نمایش است) ── */
DECLARE @reg TABLE (k VARCHAR(10) PRIMARY KEY, UserId INT, Json NVARCHAR(400));
INSERT @reg VALUES
 ('admin',    1, N'{"panelUserId":1,"userName":"مدیر سامانه","userCode":"001","branchName":"ستاد مرکزی","branchCode":"150000"}'),
 ('branch',   3, N'{"panelUserId":3,"userName":"کارشناس شعبه بازار","userCode":"399","branchName":"شعبه بازار تهران","branchCode":"150002"}'),
 ('branch2',  4, N'{"panelUserId":4,"userName":"کارشناس شعبه ونک","userCode":"402","branchName":"شعبه ونک","branchCode":"150003"}'),
 ('manager',  5, N'{"panelUserId":5,"userName":"مدیر شرکت آلفا","userCode":"501","branchName":"شعبه بازار تهران","branchCode":"150002"}'),
 ('hr',       6, N'{"panelUserId":6,"userName":"منابع انسانی آلفا","userCode":"502","branchName":"شعبه بازار تهران","branchCode":"150002"}'),
 ('manager3', 7, N'{"panelUserId":7,"userName":"مدیر شرکت گاما","userCode":"503","branchName":"شعبه ونک","branchCode":"150003"}');

/* ── ۱) شرکت‌ها ── */
DECLARE @companies TABLE (k VARCHAR(10) PRIMARY KEY, Name NVARCHAR(100), NationalId VARCHAR(11), BranchCode VARCHAR(50),
                          Opening DATE, Sepah BIT, Reg VARCHAR(10), Id BIGINT NULL);
INSERT @companies (k, Name, NationalId, BranchCode, Opening, Sepah, Reg) VALUES
 ('alfa',  N'شرکت آلفا', '10101010101', '150002', @contract, 1, 'branch'),
 ('beta',  N'شرکت بتا',  '20202020202', '150002', @contract, 0, 'branch'),
 ('gamma', N'شرکت گاما', '30303030303', '150003', @otherDay, 1, 'branch2');

DECLARE @k VARCHAR(10);
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT k FROM @companies;
OPEN c; FETCH NEXT FROM c INTO @k;
WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT panel.Companies (SchemeId, Name, NationalId, AgreementNumber, AccountNumber, ContractDate, AccountOpeningDate, SalaryFromSepah,
                            BranchCode, Status, Registrar, CreatedAt, BalancesVersion, Version)
    SELECT @scheme, x.Name, x.NationalId, 'NM-' + LEFT(x.NationalId, 3), '0100' + LEFT(x.NationalId, 6), @contract, x.Opening, x.Sepah,
           x.BranchCode, 1, r.Json, @now, 2, 1
    FROM @companies x JOIN @reg r ON r.k = x.Reg WHERE x.k = @k;
    UPDATE @companies SET Id = SCOPE_IDENTITY() WHERE k = @k;
    FETCH NEXT FROM c INTO @k;
END;
CLOSE c; DEALLOCATE c;

DECLARE @alfa BIGINT = (SELECT Id FROM @companies WHERE k = 'alfa');
DECLARE @beta BIGINT = (SELECT Id FROM @companies WHERE k = 'beta');
DECLARE @gamma BIGINT = (SELECT Id FROM @companies WHERE k = 'gamma');

INSERT panel.CompanyPlans (CompanyId, PlanId) VALUES (@alfa, @qard12), (@alfa, @murabaha12), (@beta, @qard12), (@gamma, @qard12);

-- میانگین موجودی حساب جاری ماه‌های اول قرارداد
INSERT panel.MonthlyBalances (CompanyId, Month, Amount, EditedBy, EditedAt)
SELECT b.CompanyId, m.MonthKey, b.Amount, r.Json, @now
FROM (VALUES (@alfa, 0, 5000000000, 'branch'), (@alfa, 1, 6000000000, 'branch'), (@alfa, 2, 7000000000, 'branch'),
             (@beta, 0, 1000000000, 'branch'),
             (@gamma, 0, 3000000000, 'branch2'), (@gamma, 1, 3000000000, 'branch2')) b(CompanyId, i, Amount, Reg)
JOIN @months m ON m.i = b.i
JOIN @reg r ON r.k = b.Reg;

/* ── ۲) پرسنل — Status: 1 در انتظار مدیر، 2 تأییدشده، 3 ردشده ── */
DECLARE @people TABLE (Code VARCHAR(20) PRIMARY KEY, CompanyId BIGINT, Name NVARCHAR(60), NationalCode VARCHAR(10), Mobile VARCHAR(11),
                       HireDate DATE, Priority INT, Status TINYINT, Reg VARCHAR(10), Decider VARCHAR(10), Id BIGINT NULL);
INSERT @people (Code, CompanyId, Name, NationalCode, Mobile, HireDate, Priority, Status, Reg, Decider) VALUES
 ('P101', @alfa,  N'علی محمدی',   '0010112340', '09121010000', '2023-04-10', 1, 2, 'hr',      'manager'),
 ('P102', @alfa,  N'زهرا احمدی',  '0010212345', '09121020000', '2023-04-10', 2, 2, 'hr',      'manager'),
 ('P103', @alfa,  N'رضا کریمی',   '0010312341', '09121030000', '2023-04-10', 3, 2, 'hr',      'manager'),
 ('P104', @alfa,  N'مریم حسینی',  '0010412344', '09121040000', '2023-04-10', 4, 2, 'hr',      'manager'),
 ('P105', @alfa,  N'حسین رضایی',  '0010512349', '09121050000', '2023-04-10', 5, 1, 'hr',      NULL),
 ('P106', @alfa,  N'نرگس موسوی',  '0010612343', '09121060000', '2023-04-10', 6, 3, 'hr',      'manager'),
 ('G201', @gamma, N'امیر نوری',   '0010712348', '09121070000', '2023-04-10', 1, 2, 'branch2', 'manager3'),
 ('G202', @gamma, N'سارا جعفری',  '0010812342', '09121080000', '2023-04-10', 2, 2, 'branch2', 'manager3'),
 ('P107', @alfa,  N'کیان صادقی',  '0020112343', '09122010000', '2022-09-01', 7, 2, 'hr',      'manager'),
 ('P108', @alfa,  N'ندا شریفی',   '0020212348', '09122020000', '2022-09-01', 8, 2, 'hr',      'manager'),
 ('B301', @beta,  N'بهار عباسی',  '0020312342', '09122030000', '2022-09-01', 1, 2, 'branch',  'admin'),
 ('B302', @beta,  N'امید فرهادی', '0020412347', '09122040000', '2022-09-01', 2, 2, 'branch',  'admin'),
 ('B303', @beta,  N'پویا رحیمی',  '0020512341', '09122050000', '2022-09-01', 3, 1, 'branch',  NULL),
 ('G203', @gamma, N'حامد قاسمی',  '0020612346', '09122060000', '2022-09-01', 3, 2, 'branch2', 'manager3');

DECLARE @code VARCHAR(20);
DECLARE p CURSOR LOCAL FAST_FORWARD FOR SELECT Code FROM @people;
OPEN p; FETCH NEXT FROM p INTO @code;
WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT panel.Personnel (CompanyId, Priority, FullName, NationalCode, Mobile, PersonnelCode, HireDate, Status, RejectReason,
                            CreatedAt, DecidedAt, Registrar, Version)
    SELECT x.CompanyId, x.Priority, x.Name, x.NationalCode, x.Mobile, x.Code, x.HireDate, x.Status,
           CASE WHEN x.Status = 3 THEN N'اطلاعات نادرست' END, @now, CASE WHEN x.Status <> 1 THEN @now END, r.Json,
           CASE WHEN x.Status = 1 THEN 1 ELSE 2 END
    FROM @people x JOIN @reg r ON r.k = x.Reg WHERE x.Code = @code;
    UPDATE @people SET Id = SCOPE_IDENTITY() WHERE Code = @code;
    FETCH NEXT FROM p INTO @code;
END;
CLOSE p; DEALLOCATE p;

-- تاریخچه: معرفی (← در انتظار) و تصمیم مدیر
INSERT panel.PersonnelHistory (PersonnelId, [From], [To], Reason, Description, [By], [At])
SELECT x.Id, NULL, 1, NULL, NULL, r.Json, @now FROM @people x JOIN @reg r ON r.k = x.Reg
UNION ALL
SELECT x.Id, 1, x.Status, CASE WHEN x.Status = 3 THEN N'اطلاعات نادرست' END, NULL, r.Json, @now
FROM @people x JOIN @reg r ON r.k = x.Decider WHERE x.Status <> 1;

/* ── ۳) درخواست‌های وام — هر درخواست یک دسته (Mode = 1 مبلغ یکسان)
   Status: 1 در انتظار مدیر، 2 رد مدیر، 3 در انتظار اعتبارات، 4 رد اعتبارات، 7 تأییدشده، 8 لغوشده
   Stage (تاریخچه): 1 مدیر، 2 اعتبارات ── */
DECLARE @loans TABLE (n INT PRIMARY KEY, Code VARCHAR(20), Amount BIGINT, Reg VARCHAR(10), Status TINYINT, PostStage TINYINT NULL,
                      Description NVARCHAR(1200) NULL);
INSERT @loans VALUES
 (1, 'P101', 50000000, 'hr',       1, NULL, NULL),
 (2, 'P102', 40000000, 'hr',       3, NULL, NULL),
 (3, 'G201', 30000000, 'hr',       3, NULL, NULL),
 (4, 'P107', 35000000, 'hr',       7, 1,    NULL),
 (5, 'P108', 20000000, 'hr',       8, NULL, @loanCancel + N' — انصراف کارمند از دریافت وام'),
 (6, 'B301', 25000000, 'admin',    7, NULL, NULL),
 (7, 'B302', 45000000, 'admin',    4, NULL, @loanReject + N' — سقف اعتبار شرکت کافی نیست'),
 (8, 'G203', 15000000, 'manager3', 2, NULL, @loanReject + N' — سابقهٔ کار کافی نیست');

-- مراحل هر درخواست به ترتیب: (n, ترتیب، از، به، مرحلهٔ پس از تأیید، دلیل، توضیح، تصمیم‌گیرنده)
DECLARE @steps TABLE (n INT, s INT, [From] TINYINT NULL, [To] TINYINT, Stage TINYINT NULL, Reason NVARCHAR(200) NULL,
                      Description NVARCHAR(1000) NULL, Reg VARCHAR(10));
INSERT @steps VALUES
 (2, 1, 1, 3, NULL, NULL, NULL, 'manager'),
 (3, 1, 1, 3, NULL, NULL, NULL, 'manager3'),
 (4, 1, 1, 3, NULL, NULL, NULL, 'manager'),
 (4, 2, 3, 7, NULL, NULL, NULL, 'branch'),
 (4, 3, 7, 7, 1,    NULL, NULL, 'branch'),
 (5, 1, 1, 8, NULL, @loanCancel, N'انصراف کارمند از دریافت وام', 'manager'),
 (6, 1, 1, 3, NULL, NULL, NULL, 'admin'),
 (6, 2, 3, 7, NULL, NULL, NULL, 'branch'),
 (7, 1, 1, 3, NULL, NULL, NULL, 'admin'),
 (7, 2, 3, 4, NULL, @loanReject, N'سقف اعتبار شرکت کافی نیست', 'branch'),
 (8, 1, 1, 2, NULL, @loanReject, N'سابقهٔ کار کافی نیست', 'manager3');

DECLARE @n INT = 1, @batch BIGINT, @request BIGINT;
WHILE @n <= 8
BEGIN
    INSERT panel.LoanBatches (CompanyId, PlanId, Mode, BaseDate, Description, Registrar, CreatedAt, BranchCode)
    SELECT pp.CompanyId, @qard12, 1, @today, NULL, r.Json, @now, co.BranchCode
    FROM @loans l JOIN @people pp ON pp.Code = l.Code JOIN @companies co ON co.Id = pp.CompanyId JOIN @reg r ON r.k = l.Reg
    WHERE l.n = @n;
    SET @batch = SCOPE_IDENTITY();

    INSERT panel.LoanRequests (BatchId, CompanyId, BranchCode, PersonnelId, PlanId, Amount, PaymentOrder, CreatedAt, DecidedAt, Status,
                               PostStage, Description, Registrar, Version)
    SELECT @batch, pp.CompanyId, co.BranchCode, pp.Id, @qard12, l.Amount, 1, @now, CASE WHEN l.Status <> 1 THEN @now END, l.Status,
           l.PostStage, l.Description, r.Json, 1 + (SELECT COUNT(*) FROM @steps s WHERE s.n = l.n AND s.[From] <> s.[To])
    FROM @loans l JOIN @people pp ON pp.Code = l.Code JOIN @companies co ON co.Id = pp.CompanyId JOIN @reg r ON r.k = l.Reg
    WHERE l.n = @n;
    SET @request = SCOPE_IDENTITY();

    INSERT panel.LoanRequestHistory (RequestId, [From], [To], Stage, Reason, Description, [By], [At])
    SELECT @request, NULL, 1, NULL, NULL, NULL, r.Json, @now FROM @loans l JOIN @reg r ON r.k = l.Reg WHERE l.n = @n
    UNION ALL
    SELECT @request, s.[From], s.[To], s.Stage, s.Reason, s.Description, r.Json, DATEADD(SECOND, s.s, @now)
    FROM @steps s JOIN @reg r ON r.k = s.Reg WHERE s.n = @n;

    SET @n += 1;
END;

/* ── ۴) گزارش تغییرات — Category: 1 شرکت، 2 پرسنل، 3 وام ── */
INSERT panel.ChangeLogs ([At], UserId, [User], Category, Action, Details, CompanyId, BranchCode)
SELECT DATEADD(MINUTE, l.i - 10, @now), r.UserId, r.Json, l.Category, l.Action, l.Details, co.Id, co.BranchCode
FROM (VALUES
 (1, 'branch',  1, N'ثبت شرکت',                  N'شرکت آلفا',                                   'alfa'),
 (2, 'branch',  1, N'ثبت شرکت',                  N'شرکت بتا',                                    'beta'),
 (3, 'branch2', 1, N'ثبت شرکت',                  N'شرکت گاما',                                   'gamma'),
 (4, 'branch',  1, N'ثبت میانگین موجودی',        N'شرکت آلفا — ۳ ماه',                           'alfa'),
 (5, 'hr',      2, N'معرفی پرسنل',               N'کیان صادقی (P107)',                           'alfa'),
 (6, 'branch',  2, N'معرفی پرسنل',               N'پویا رحیمی (B303)',                           'beta'),
 (7, 'hr',      3, N'ثبت درخواست وام',           N'کیان صادقی — ۳۵٬۰۰۰٬۰۰۰ ریال',                'alfa'),
 (8, 'branch',  3, N'تغییر مرحلهٔ پس از تأیید',  N'کیان صادقی — تشکیل پرونده در شعبه',           'alfa'),
 (9, 'manager', 3, N'لغو درخواست وام',           N'ندا شریفی — انصراف کارمند از دریافت وام',     'alfa')
) l(i, Reg, Category, Action, Details, Company)
JOIN @reg r ON r.k = l.Reg
JOIN @companies co ON co.k = l.Company;

COMMIT;

/* ── گزارش ── */
SELECT N'شناسهٔ شرکت‌ها (در 08_test_users.sql همین‌ها را بگذارید)' AS [مورد],
       CONCAT(N'آلفا=', @alfa, N'  بتا=', @beta, N'  گاما=', @gamma) AS [مقدار]
UNION ALL SELECT N'تاریخ قرارداد (شمسی)', CONCAT(@cy, '/', FORMAT(@cm, '00'), '/', FORMAT(@cd, '00'))
UNION ALL SELECT N'پرسنل', CAST((SELECT COUNT(*) FROM @people) AS NVARCHAR(10))
UNION ALL SELECT N'درخواست وام', CAST((SELECT COUNT(*) FROM @loans) AS NVARCHAR(10));

SELECT co.Name AS [شرکت], lr.Id AS [درخواست], pe.FullName AS [پرسنل], lr.Amount AS [مبلغ], lr.Status AS [وضعیت]
FROM panel.LoanRequests lr JOIN panel.Personnel pe ON pe.Id = lr.PersonnelId JOIN panel.Companies co ON co.Id = lr.CompanyId
WHERE co.Id IN (@alfa, @beta, @gamma) ORDER BY lr.Id;
GO

DROP FUNCTION panel.TestData_ToJalali;
GO
PRINT N'دادهٔ تست Facility ثبت شد.';

/* =====================================================================
   پاک‌سازی دادهٔ تست (فقط همین سه شرکت و هرچه به آن‌ها وصل است) — برای اجرا کامنت را بردارید
   =====================================================================
BEGIN TRAN;
DECLARE @ids TABLE (Id BIGINT);
INSERT @ids SELECT Id FROM panel.Companies WHERE NationalId IN ('10101010101', '20202020202', '30303030303');
DELETE FROM panel.ChangeLogs       WHERE CompanyId IN (SELECT Id FROM @ids);
DELETE FROM panel.LoanBatches      WHERE CompanyId IN (SELECT Id FROM @ids);   -- درخواست‌ها، مستندات و تاریخچه cascade
DELETE FROM panel.Personnel        WHERE CompanyId IN (SELECT Id FROM @ids);   -- تاریخچهٔ پرسنل cascade
DELETE FROM panel.PersonnelImports WHERE CompanyId IN (SELECT Id FROM @ids);
DELETE FROM panel.CompanyCancellations WHERE CompanyId IN (SELECT Id FROM @ids);
DELETE FROM panel.BalanceDocuments WHERE CompanyId IN (SELECT Id FROM @ids);
DELETE FROM panel.Companies        WHERE Id IN (SELECT Id FROM @ids);          -- طرح‌ها و میانگین‌ها cascade
COMMIT;
*/
