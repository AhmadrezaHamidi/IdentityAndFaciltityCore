/* =====================================================================
   Facility — دادهٔ تست بیشتر برای همهٔ تب‌ها (فقط دیتابیس توسعه/تست، نه عملیاتی)
   ---------------------------------------------------------------------
   پیش‌نیاز: facility_panel_migrations.sql و 02_test_data.sql
   بعد از این: identityservice/Database/08b_more_test_data.sql روی Identity (کاربران همین شرکت‌ها)

   ۷ شرکت دیگر در ۴ شعبه، هر کدام در یک وضعیت:
     دلتا     150002  فعال، روز انتقال = امروز، ۲ طرح، مدیر و منابع انسانی دارد
     اپسیلون  150003  فعال، امروز روز انتقال نیست
     زتا      150004  فعال، ۳ طرح، بارگذاری گروهی پرسنل، مدیر و منابع انسانی دارد
     اتا      150004  قرارداد همین ماه (هنوز مهلت معرفی پرسنل نرسیده)
     تتا      150002  قرارداد لغوشده (با نامهٔ لغو)
     یوتا     150003  قرارداد منقضی
     کاپا     150001  شعبهٔ دیجیتال
   ۴۴ پرسنل (تأییدشده، در انتظار، ردشده، حذف‌شده) و ۳۲ درخواست وام در همهٔ وضعیت‌ها:
     در انتظار مدیر، رد مدیر، در انتظار اعتبارات، رد اعتبارات، در انتظار شعبهٔ دیجیتال، رد دیجیتال،
     تأییدشده با هر چهار مرحلهٔ پس از تأیید، لغوشده؛ دسته‌های چندنفره با مبلغ یکسان و مبلغ جدا.
   گزارش تغییرات برای همهٔ این‌ها ساخته می‌شود.

   ثبت‌کننده‌ها کاربران تست Identity‌اند (شناسه همان شناسهٔ Identity توسعه؛ فقط برای نمایش و فیلتر).
   فایل نامهٔ لغو تتا فقط ردیف دارد (محتوایش در Minio نیست؛ دانلودش «پیدا نشد» می‌دهد).

   idempotent: اگر شرکت دلتا (شناسهٔ ملی 40404040404) هست، کاری نمی‌کند.
   پاک‌سازی: بخش آخر فایل (کامنت).
   ===================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID('panel.TestData3_ToJalali') IS NOT NULL DROP FUNCTION panel.TestData3_ToJalali;
GO
CREATE FUNCTION panel.TestData3_ToJalali (@d DATE) RETURNS TABLE AS RETURN
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

/* کد ملی نمونهٔ معتبر: 00 + n + 1234 + رقم کنترل (مثل FacilityPanelSampleData) */
IF OBJECT_ID('panel.TestData3_NationalCode') IS NOT NULL DROP FUNCTION panel.TestData3_NationalCode;
GO
CREATE FUNCTION panel.TestData3_NationalCode (@n INT) RETURNS TABLE AS RETURN
SELECT Code = f.s + CAST(CASE WHEN r.v < 2 THEN r.v ELSE 11 - r.v END AS VARCHAR(1))
FROM (SELECT s = '00' + CAST(@n AS VARCHAR(3)) + '1234') f
CROSS APPLY (SELECT v = (10 * CAST(SUBSTRING(f.s, 1, 1) AS INT) + 9 * CAST(SUBSTRING(f.s, 2, 1) AS INT) + 8 * CAST(SUBSTRING(f.s, 3, 1) AS INT)
                        + 7 * CAST(SUBSTRING(f.s, 4, 1) AS INT) + 6 * CAST(SUBSTRING(f.s, 5, 1) AS INT) + 5 * CAST(SUBSTRING(f.s, 6, 1) AS INT)
                        + 4 * CAST(SUBSTRING(f.s, 7, 1) AS INT) + 3 * CAST(SUBSTRING(f.s, 8, 1) AS INT) + 2 * CAST(SUBSTRING(f.s, 9, 1) AS INT)) % 11) r;
GO

IF EXISTS (SELECT 1 FROM panel.Companies WHERE NationalId = '40404040404' AND DeletedAt IS NULL)
BEGIN
    PRINT N'دادهٔ تست بیشتر از قبل هست (شرکت دلتا). برای ساخت دوباره، اول بخش پاک‌سازی را اجرا کنید.';
    RETURN;
END;
IF NOT EXISTS (SELECT 1 FROM panel.Companies WHERE NationalId = '10101010101')
    THROW 50301, N'اول 02_test_data.sql را اجرا کنید.', 1;

BEGIN TRAN;

DECLARE @now DATETIME2 = SYSUTCDATETIME();
DECLARE @today DATE = CAST(SYSDATETIME() AS DATE);
DECLARE @ty INT, @tm INT, @td INT;
SELECT @ty = jy, @tm = jm, @td = jd FROM panel.TestData3_ToJalali(@today);

DECLARE @days TABLE (d DATE PRIMARY KEY, jy INT, jm INT, jd INT);
INSERT @days SELECT x.d, j.jy, j.jm, j.jd
FROM (SELECT d = DATEADD(DAY, -n, @today) FROM (SELECT TOP (600) n = ROW_NUMBER() OVER (ORDER BY (SELECT 1)) - 1 FROM sys.all_objects a CROSS JOIN sys.all_objects b) t) x
CROSS APPLY panel.TestData3_ToJalali(x.d) j;

DECLARE @scheme INT = (SELECT TOP (1) SchemeId FROM panel.SchemeSettings ORDER BY SchemeId);
DECLARE @plans TABLE (k VARCHAR(10) PRIMARY KEY, Id INT, Title NVARCHAR(60));
INSERT @plans
SELECT v.k, p.Id, v.Title
FROM (VALUES ('q12', 1, 4, 12, N'قرض‌الحسنه ۴٪ — ۱۲ ماهه'), ('q24', 1, 4, 24, N'قرض‌الحسنه ۴٪ — ۲۴ ماهه'),
             ('q36', 1, 4, 36, N'قرض‌الحسنه ۴٪ — ۳۶ ماهه'), ('m12', 2, 5, 12, N'مرابحه ۵٪ — ۱۲ ماهه'),
             ('m10', 2, 10, 12, N'مرابحه ۱۰٪ — ۱۲ ماهه')) v(k, ContractType, Rate, TermMonths, Title)
JOIN panel.SchemePlans p ON p.SchemeId = @scheme AND p.ContractType = v.ContractType AND p.Rate = v.Rate AND p.TermMonths = v.TermMonths AND p.IsActive = 1;
IF (SELECT COUNT(*) FROM @plans) <> 5 THROW 50302, N'یکی از طرح‌های اعتباری پایه پیدا نشد (facility_panel_migrations.sql).', 1;

DECLARE @reasons TABLE (Type TINYINT, n INT, Id INT, Title NVARCHAR(100), PRIMARY KEY (Type, n));
INSERT @reasons SELECT Type, ROW_NUMBER() OVER (PARTITION BY Type ORDER BY SortOrder, Id), Id, Title FROM panel.Reasons WHERE IsActive = 1;

/* ── ثبت‌کننده‌ها (کاربران تست Identity؛ 08b همین‌ها را می‌سازد) ── */
DECLARE @reg TABLE (k VARCHAR(20) PRIMARY KEY, UserId INT, Json NVARCHAR(400));
INSERT @reg (k, UserId, Json)
SELECT k, id, CONCAT(N'{"panelUserId":', id, N',"userName":"', name, N'","userCode":"', code, N'","branchName":"', bname, N'","branchCode":"', bcode, N'"}')
FROM (VALUES
 ('admin',          1, N'مدیر سامانه',                '001', N'ستاد مرکزی',       '150000'),
 ('digital',        2, N'کارشناس شعبهٔ دیجیتال',      '002', N'شعبهٔ دیجیتال',    '150001'),
 ('branch',         3, N'کارشناس شعبه بازار',         '399', N'شعبه بازار تهران', '150002'),
 ('branch2',        4, N'کارشناس شعبه ونک',           '402', N'شعبه ونک',         '150003'),
 ('branch3',        8, N'کارشناس شعبه تجریش',         '404', N'شعبه تجریش',       '150004'),
 ('branch.bazar2',  9, N'کارشناس اعتبارات شعبه بازار', '398', N'شعبه بازار تهران', '150002'),
 ('manager.delta', 10, N'مدیر شرکت دلتا',             '511', N'شعبه بازار تهران', '150002'),
 ('hr.delta',      11, N'منابع انسانی دلتا',          '512', N'شعبه بازار تهران', '150002'),
 ('manager.zeta',  12, N'مدیر شرکت زتا',              '521', N'شعبه تجریش',       '150004'),
 ('hr.zeta',       13, N'منابع انسانی زتا',           '522', N'شعبه تجریش',       '150004'),
 ('digital2',      16, N'کارشناس دوم شعبهٔ دیجیتال',  '003', N'شعبهٔ دیجیتال',    '150001')
) v(k, id, name, code, bname, bcode);

/* ── ۱) شرکت‌ها ── Status: 1 فعال، 2 لغوشده، 3 منقضی */
DECLARE @co TABLE (k VARCHAR(10) PRIMARY KEY, o INT IDENTITY(1, 1), Name NVARCHAR(100), NationalId VARCHAR(11), BranchCode VARCHAR(50), MonthsAgo INT,
                   SameDay BIT, Sepah BIT, Reg VARCHAR(20), Status TINYINT, Base BIGINT,
                   Manager VARCHAR(20), Credit VARCHAR(20), Hr VARCHAR(20), Digital VARCHAR(20),
                   ContractDate DATE NULL, Opening DATE NULL, cy INT NULL, cm INT NULL, Id BIGINT NULL);
INSERT @co (k, Name, NationalId, BranchCode, MonthsAgo, SameDay, Sepah, Reg, Status, Base, Manager, Credit, Hr, Digital) VALUES
 ('delta',   N'شرکت دلتا',    '40404040404', '150002',  5, 1, 1, 'branch',  1, 4000000000, 'manager.delta', 'branch.bazar2', 'hr.delta', 'digital'),
 ('epsilon', N'شرکت اپسیلون', '50505050505', '150003',  2, 0, 0, 'branch2', 1, 2000000000, 'admin',         'branch2',       'admin',    'digital'),
 ('zeta',    N'شرکت زتا',     '60606060606', '150004',  4, 1, 1, 'branch3', 1, 8000000000, 'manager.zeta',  'branch3',       'hr.zeta',  'digital'),
 ('eta',     N'شرکت اتا',     '70707070707', '150004',  0, 1, 0, 'branch3', 1, 0,          'admin',         'branch3',       'admin',    'digital'),
 ('theta',   N'شرکت تتا',     '80808080808', '150002',  8, 0, 1, 'branch',  2, 1500000000, 'admin',         'branch',        'admin',    'digital'),
 ('iota',    N'شرکت یوتا',    '90909090909', '150003', 17, 0, 1, 'branch2', 3, 2500000000, 'admin',         'branch2',       'admin',    'digital'),
 ('kappa',   N'شرکت کاپا',    '12121212121', '150001',  6, 1, 0, 'admin',   1, 3000000000, 'admin',         'admin',         'admin',    'digital2');

-- تاریخ قرارداد = همان روز امروز، MonthsAgo ماه شمسی پیش (اگر ماه کوتاه‌تر است، آخرین روزش)
UPDATE c SET ContractDate = (SELECT MAX(d.d) FROM @days d
                             WHERE d.jy = (@ty * 12 + @tm - 1 - c.MonthsAgo) / 12 AND d.jm = (@ty * 12 + @tm - 1 - c.MonthsAgo) % 12 + 1 AND d.jd <= @td),
             cy = (@ty * 12 + @tm - 1 - c.MonthsAgo) / 12, cm = (@ty * 12 + @tm - 1 - c.MonthsAgo) % 12 + 1
FROM @co c;
-- روز افتتاح حساب: همان روز (امروز روز انتقال است) یا روز دیگری از همان ماه
UPDATE c SET Opening = CASE WHEN c.SameDay = 1 THEN c.ContractDate
                            ELSE (SELECT d.d FROM @days d WHERE d.jy = c.cy AND d.jm = c.cm AND d.jd = @td % 28 + 1) END
FROM @co c;
IF EXISTS (SELECT 1 FROM @co WHERE ContractDate IS NULL OR Opening IS NULL) THROW 50303, N'محاسبهٔ تاریخ قرارداد ناموفق بود.', 1;

-- به همان ترتیب بالا (روی دیتابیس تازه: دلتا=4، اپسیلون=5، زتا=6، اتا=7، تتا=8، یوتا=9، کاپا=10)
DECLARE @k VARCHAR(10);
DECLARE c CURSOR LOCAL FAST_FORWARD FOR SELECT k FROM @co ORDER BY o;
OPEN c; FETCH NEXT FROM c INTO @k;
WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT panel.Companies (SchemeId, Name, NationalId, AgreementNumber, AccountNumber, ContractDate, AccountOpeningDate, SalaryFromSepah,
                            BranchCode, Status, Registrar, CreatedAt, BalancesVersion, Version)
    SELECT @scheme, s.Name, s.NationalId, 'NM-' + LEFT(s.NationalId, 3), '0100' + LEFT(s.NationalId, 6), s.ContractDate, s.Opening, s.Sepah,
           s.BranchCode, s.Status, r.Json, DATEADD(HOUR, 9, CAST(s.ContractDate AS DATETIME2)), 2, CASE WHEN s.Status = 2 THEN 2 ELSE 1 END
    FROM @co s JOIN @reg r ON r.k = s.Reg WHERE s.k = @k;
    UPDATE @co SET Id = SCOPE_IDENTITY() WHERE k = @k;
    FETCH NEXT FROM c INTO @k;
END;
CLOSE c; DEALLOCATE c;

INSERT panel.CompanyPlans (CompanyId, PlanId)
SELECT c.Id, p.Id
FROM (VALUES ('delta', 'q12'), ('delta', 'q24'), ('epsilon', 'm12'), ('zeta', 'q12'), ('zeta', 'q36'), ('zeta', 'm10'),
             ('eta', 'q12'), ('theta', 'q12'), ('iota', 'q12'), ('iota', 'q24'), ('kappa', 'q24')) v(co, pl)
JOIN @co c ON c.k = v.co JOIN @plans p ON p.k = v.pl;

-- میانگین موجودی ماه‌های گذشتهٔ قرارداد (حداکثر ۱۲ ماه)
INSERT panel.MonthlyBalances (CompanyId, Month, Amount, EditedBy, EditedAt)
SELECT c.Id,
       FORMAT((c.cy * 12 + c.cm - 1 + m.i) / 12, '0000') + '/' + FORMAT((c.cy * 12 + c.cm - 1 + m.i) % 12 + 1, '00'),
       c.Base + c.Base * ((m.i * 13) % 7) / 10, r.Json, DATEADD(DAY, 1, DATEADD(MONTH, m.i, CAST(c.ContractDate AS DATETIME2)))
FROM @co c
JOIN @reg r ON r.k = c.Reg
JOIN (VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11)) m(i) ON m.i < c.MonthsAgo;

-- لغو قرارداد تتا (نامهٔ لغو: فقط ردیف فایل)
DECLARE @letter TABLE (Id BIGINT);
INSERT panel.Files (FileName, ContentType, Size, StorageKey, UploadedAt, UploadedBy)
OUTPUT inserted.Id INTO @letter
VALUES (N'نامه-لغو-قرارداد-تتا.pdf', 'application/pdf', 48213, 'test-data/theta-cancel-letter.pdf', DATEADD(DAY, -20, @now), N'کارشناس شعبه بازار');
INSERT panel.CompanyCancellations (CompanyId, CancelDate, ReasonId, ReasonTitle, Description, LetterFileId, CancelledBy, CancelledAt)
SELECT c.Id, DATEADD(DAY, -20, @today), rs.Id, rs.Title, N'شرکت طی نامهٔ رسمی درخواست عدم تمدید قرارداد را داد.', (SELECT Id FROM @letter), r.Json, DATEADD(DAY, -20, @now)
FROM @co c JOIN @reg r ON r.k = 'branch' JOIN @reasons rs ON rs.Type = 1 AND rs.n = 1
WHERE c.k = 'theta';

/* ── ۲) پرسنل ── Status: 1 در انتظار مدیر، 2 تأییدشده، 3 ردشده، 4 حذف‌شده */
DECLARE @pe TABLE (Code VARCHAR(20) PRIMARY KEY, Co VARCHAR(10), Name NVARCHAR(60), Status TINYINT, HireDate DATE, DaysAgo INT,
                   Note NVARCHAR(200) NULL, Imported BIT, n INT NULL, Id BIGINT NULL);
INSERT @pe (Code, Co, Name, Status, HireDate, DaysAgo, Note, Imported) VALUES
 ('D401', 'delta',   N'محمد کاظمی',     2, '2019-05-12', 120, NULL, 0),
 ('D402', 'delta',   N'فاطمه رستمی',    2, '2020-02-01', 120, NULL, 0),
 ('D403', 'delta',   N'سعید یزدانی',    2, '2018-11-20', 120, NULL, 0),
 ('D404', 'delta',   N'لیلا نجفی',      2, '2021-07-15', 110, NULL, 0),
 ('D405', 'delta',   N'مهدی اکبری',     2, '2017-03-03', 110, NULL, 0),
 ('D406', 'delta',   N'الهام طاهری',    2, '2022-01-09', 110, NULL, 0),
 ('D407', 'delta',   N'بهروز سلیمانی',  1, '2023-06-01',   2, NULL, 0),
 ('D408', 'delta',   N'شیرین مرادی',    1, '2023-08-14',   1, NULL, 0),
 ('D409', 'delta',   N'کاوه بهرامی',    3, '2023-02-27',  30, N'عدم احراز شرایط', 0),
 ('D410', 'delta',   N'پریسا خلیلی',    4, '2020-10-10', 100, N'ترک خدمت از ابتدای ماه', 0),
 ('D411', 'delta',   N'علیرضا شاکری',   2, '2019-09-30',  60, NULL, 0),
 ('D412', 'delta',   N'مرضیه انصاری',   2, '2021-04-18',  60, NULL, 0),
 ('D413', 'delta',   N'پیمان عزیزی',    2, '2016-12-05',  60, NULL, 0),
 ('D414', 'delta',   N'سارا مختاری',    2, '2022-05-22',  60, NULL, 0),
 ('E501', 'epsilon', N'جواد حیدری',     2, '2020-06-06',  50, NULL, 0),
 ('E502', 'epsilon', N'سمیرا زمانی',    2, '2021-09-01',  50, NULL, 0),
 ('E503', 'epsilon', N'فرشید امینی',    2, '2019-01-16',  50, NULL, 0),
 ('E504', 'epsilon', N'نسرین قربانی',   1, '2023-03-11',   3, NULL, 0),
 ('Z601', 'zeta',    N'آرش توکلی',      2, '2018-04-04', 110, NULL, 0),
 ('Z602', 'zeta',    N'مینا صفری',      2, '2019-08-19', 110, NULL, 0),
 ('Z603', 'zeta',    N'داوود جلالی',    2, '2017-10-28', 110, NULL, 0),
 ('Z604', 'zeta',    N'هانیه کریمیان',  2, '2020-12-12', 100, NULL, 0),
 ('Z605', 'zeta',    N'یاسر محمودی',    2, '2021-02-02', 100, NULL, 0),
 ('Z606', 'zeta',    N'رویا عسگری',     2, '2016-07-07', 100, NULL, 0),
 ('Z607', 'zeta',    N'بابک فتحی',      2, '2022-03-21',  90, NULL, 0),
 ('Z608', 'zeta',    N'طاهره نیک‌نام',  2, '2018-06-30',  90, NULL, 0),
 ('Z609', 'zeta',    N'عرفان شجاعی',    1, '2023-05-05',   4, NULL, 1),
 ('Z610', 'zeta',    N'گلاره سبحانی',   1, '2023-05-20',   4, NULL, 1),
 ('Z611', 'zeta',    N'سینا مقدم',      3, '2023-04-01',   4, N'اطلاعات نادرست', 1),
 ('H701', 'eta',     N'کامران نادری',   2, '2021-11-11',   1, NULL, 0),
 ('H702', 'eta',     N'ترانه فرجی',     1, '2022-08-08',   1, NULL, 0),
 ('T801', 'theta',   N'مسعود یگانه',    2, '2015-05-15', 230, NULL, 0),
 ('T802', 'theta',   N'نیلوفر شکوهی',   2, '2019-03-23', 230, NULL, 0),
 ('T803', 'theta',   N'اکبر رنجبر',     2, '2018-09-09', 230, NULL, 0),
 ('I901', 'iota',    N'حمید پورمحمدی',  2, '2014-02-14', 500, NULL, 0),
 ('I902', 'iota',    N'شبنم کیانی',     2, '2017-12-01', 500, NULL, 0),
 ('I903', 'iota',    N'رامین اسدی',     2, '2016-04-27', 480, NULL, 0),
 ('K101', 'kappa',   N'وحید نصیری',     2, '2019-07-01', 170, NULL, 0),
 ('K102', 'kappa',   N'آزاده رفیعی',    2, '2020-10-20', 170, NULL, 0),
 ('K103', 'kappa',   N'مجید حاتمی',     2, '2018-01-08', 170, NULL, 0),
 ('K104', 'kappa',   N'سحر یوسفی',      1, '2023-07-07',   2, NULL, 0),
 ('K105', 'kappa',   N'رسول جهانگیری',  2, '2017-05-25', 160, NULL, 0),
 ('K106', 'kappa',   N'مهسا ابراهیمی',  3, '2023-01-13',  20, N'عدم احراز شرایط', 0),
 ('Z612', 'zeta',    N'نوید پاکزاد',    2, '2020-09-09',  90, NULL, 0);
UPDATE p SET n = 300 + x.rn FROM @pe p JOIN (SELECT Code, rn = ROW_NUMBER() OVER (ORDER BY Code) FROM @pe) x ON x.Code = p.Code;

-- بارگذاری گروهی زتا: ۵ ردیف، ۳ پذیرفته (Z609..Z611)، ۲ خطا
DECLARE @import TABLE (Id BIGINT);
INSERT panel.PersonnelImports (CompanyId, FileName, TotalRows, Accepted, Rejected, ErrorsJson, Registrar, CreatedAt)
OUTPUT inserted.Id INTO @import
SELECT c.Id, N'پرسنل-زتا-مهر.xlsx', 5, 3, 2,
       N'[{"row":4,"column":"کد ملی","message":"کد ملی نامعتبر است."},{"row":6,"column":"شماره همراه","message":"شماره همراه باید ۱۱ رقم و با ۰۹ شروع شود."}]',
       r.Json, DATEADD(DAY, -4, @now)
FROM @co c JOIN @reg r ON r.k = c.Hr WHERE c.k = 'zeta';

DECLARE @peIds TABLE (Code VARCHAR(20), Id BIGINT);
MERGE panel.Personnel AS t
USING (SELECT p.*, c.Id AS CompanyId, rr.Json AS RegJson,
              Priority = ROW_NUMBER() OVER (PARTITION BY p.Co ORDER BY p.Code), nc.Code AS NationalCode,
              CreatedAt = DATEADD(DAY, -p.DaysAgo, @now)
       FROM @pe p JOIN @co c ON c.k = p.Co JOIN @reg rr ON rr.k = c.Hr
       CROSS APPLY panel.TestData3_NationalCode(p.n) nc) AS s ON 1 = 0
WHEN NOT MATCHED THEN
    INSERT (CompanyId, Priority, FullName, NationalCode, Mobile, PersonnelCode, HireDate, Status, RejectReason, CreatedAt, DecidedAt,
            Registrar, ImportBatchId, DeleteDescription, Version)
    VALUES (s.CompanyId, s.Priority, s.Name, s.NationalCode, '0913' + CAST(s.n AS VARCHAR(3)) + '0000', s.Code, s.HireDate, s.Status,
            CASE WHEN s.Status = 3 THEN s.Note END, s.CreatedAt, CASE WHEN s.Status <> 1 THEN DATEADD(DAY, 1, s.CreatedAt) END,
            s.RegJson, CASE WHEN s.Imported = 1 THEN (SELECT Id FROM @import) END, CASE WHEN s.Status = 4 THEN s.Note END,
            CASE s.Status WHEN 1 THEN 1 WHEN 4 THEN 3 ELSE 2 END)
OUTPUT s.Code, inserted.Id INTO @peIds;
UPDATE p SET Id = i.Id FROM @pe p JOIN @peIds i ON i.Code = p.Code;

-- تاریخچه: معرفی، تصمیم مدیر، حذف
INSERT panel.PersonnelHistory (PersonnelId, [From], [To], Reason, Description, [By], [At])
SELECT p.Id, NULL, 1, NULL, NULL, r.Json, DATEADD(DAY, -p.DaysAgo, @now)
FROM @pe p JOIN @co c ON c.k = p.Co JOIN @reg r ON r.k = c.Hr
UNION ALL
SELECT p.Id, 1, CASE WHEN p.Status = 4 THEN 2 ELSE p.Status END, CASE WHEN p.Status = 3 THEN p.Note END, NULL, r.Json,
       DATEADD(DAY, 1 - p.DaysAgo, @now)
FROM @pe p JOIN @co c ON c.k = p.Co JOIN @reg r ON r.k = c.Manager WHERE p.Status <> 1
UNION ALL
SELECT p.Id, 2, 4, NULL, p.Note, r.Json, DATEADD(DAY, 10 - p.DaysAgo, @now)
FROM @pe p JOIN @co c ON c.k = p.Co JOIN @reg r ON r.k = c.Hr WHERE p.Status = 4;

/* ── ۳) درخواست‌های وام
   Status: 1 در انتظار مدیر، 2 رد مدیر، 3 در انتظار اعتبارات، 4 رد اعتبارات، 5 در انتظار شعبهٔ دیجیتال، 6 رد دیجیتال، 7 تأییدشده، 8 لغوشده
   Post (مرحلهٔ پس از تأیید): 1 تشکیل پرونده، 2 دریافت از امید بانک، 3 دریافت در شعبه، 4 عدم اعطای غیرحضوری
   درخواست‌هایی که از «شعبهٔ دیجیتال» گذشته‌اند وقتی ثبت شده‌اند که آن مرحله فعال بوده است. ── */
DECLARE @lb TABLE (b INT PRIMARY KEY, Co VARCHAR(10), PlanKey VARCHAR(10), Mode TINYINT, Reg VARCHAR(20), DaysAgo INT, Description NVARCHAR(200) NULL, Id BIGINT NULL);
INSERT @lb (b, Co, PlanKey, Mode, Reg, DaysAgo, Description) VALUES
 ( 1, 'delta',   'q12', 1, 'hr.delta',      45, N'وام قرض‌الحسنهٔ نوبت اول'),
 ( 2, 'delta',   'q24', 2, 'hr.delta',      20, NULL),
 ( 3, 'delta',   'q12', 1, 'hr.delta',       0, N'درخواست ماه جاری'),
 ( 4, 'delta',   'q24', 2, 'manager.delta',  3, NULL),
 ( 5, 'epsilon', 'm12', 1, 'admin',         12, NULL),
 ( 6, 'zeta',    'q12', 1, 'hr.zeta',       60, N'نوبت اول طرح'),
 ( 7, 'zeta',    'q36', 2, 'hr.zeta',       30, NULL),
 ( 8, 'zeta',    'm10', 1, 'manager.zeta',   5, NULL),
 ( 9, 'zeta',    'q12', 1, 'hr.zeta',        0, NULL),
 (10, 'theta',   'q12', 1, 'admin',        120, NULL),
 (11, 'iota',    'q12', 1, 'admin',        300, NULL),
 (12, 'iota',    'q24', 2, 'admin',        200, NULL),
 (13, 'kappa',   'q24', 1, 'admin',         25, NULL),
 (14, 'kappa',   'q24', 2, 'admin',          6, NULL);

DECLARE @lr TABLE (n INT PRIMARY KEY, b INT, Code VARCHAR(20), Amount BIGINT, Status TINYINT, Post TINYINT NULL, ViaDigital BIT, Note NVARCHAR(300) NULL,
                   PaymentOrder INT, Id BIGINT NULL);
INSERT @lr (n, b, Code, Amount, Status, Post, ViaDigital, Note, PaymentOrder) VALUES
 ( 1,  1, 'D401',  50000000, 7, 3,    0, NULL, 1),
 ( 2,  1, 'D402',  50000000, 7, 2,    0, NULL, 2),
 ( 3,  1, 'D403',  50000000, 7, 4,    0, N'متقاضی برای اعطای حضوری به شعبه ارجاع شد', 3),
 ( 4,  2, 'D404',  80000000, 5, NULL, 1, NULL, 1),
 ( 5,  2, 'D405',  45000000, 6, NULL, 1, N'مدارک شغلی ناقص است', 2),
 ( 6,  2, 'D406',  70000000, 7, 1,    1, NULL, 3),
 ( 7,  3, 'D411',  40000000, 1, NULL, 0, NULL, 1),
 ( 8,  3, 'D412',  40000000, 1, NULL, 0, NULL, 2),
 ( 9,  4, 'D413',  65000000, 3, NULL, 0, NULL, 1),
 (10,  4, 'D414',  55000000, 8, NULL, 0, N'کارمند از دریافت وام انصراف داد', 2),
 (11,  5, 'E501',  30000000, 3, NULL, 0, NULL, 1),
 (12,  5, 'E502',  30000000, 4, NULL, 0, N'رتبهٔ اعتباری متقاضی پایین است', 2),
 (13,  5, 'E503',  30000000, 1, NULL, 0, NULL, 3),
 (14,  6, 'Z601',  70000000, 7, 3,    0, NULL, 1),
 (15,  6, 'Z602',  70000000, 7, 3,    0, NULL, 2),
 (16,  6, 'Z603',  70000000, 7, 2,    0, NULL, 3),
 (17,  7, 'Z604', 120000000, 7, 1,    0, NULL, 1),
 (18,  7, 'Z605',  90000000, 2, NULL, 0, N'مبلغ از سقف فردی بیشتر است', 2),
 (19,  7, 'Z606', 100000000, 7, NULL, 0, NULL, 3),
 (20,  8, 'Z607',  50000000, 3, NULL, 0, NULL, 1),
 (21,  8, 'Z608',  50000000, 3, NULL, 0, NULL, 2),
 (22,  9, 'Z605',  35000000, 1, NULL, 0, NULL, 1),
 (23,  9, 'Z612',  35000000, 1, NULL, 0, NULL, 2),
 (24, 10, 'T801',  40000000, 7, 3,    0, NULL, 1),
 (25, 10, 'T802',  40000000, 7, 3,    0, NULL, 2),
 (26, 10, 'T803',  40000000, 8, NULL, 0, N'متقاضی منصرف شد', 3),
 (27, 11, 'I901',  50000000, 7, 3,    0, NULL, 1),
 (28, 11, 'I902',  50000000, 7, 3,    0, NULL, 2),
 (29, 12, 'I903',  75000000, 7, 2,    0, NULL, 1),
 (30, 13, 'K101',  45000000, 7, 1,    0, NULL, 1),
 (31, 13, 'K102',  45000000, 5, NULL, 1, NULL, 2),
 (32, 14, 'K103',  60000000, 7, 2,    1, NULL, 1),
 (33, 14, 'K105',  55000000, 3, NULL, 0, NULL, 2);

DECLARE @lbIds TABLE (b INT, Id BIGINT);
MERGE panel.LoanBatches AS t
USING (SELECT l.*, c.Id AS CompanyId, c.BranchCode, p.Id AS PlanId, r.Json
       FROM @lb l JOIN @co c ON c.k = l.Co JOIN @plans p ON p.k = l.PlanKey JOIN @reg r ON r.k = l.Reg) AS s ON 1 = 0
WHEN NOT MATCHED THEN
    INSERT (CompanyId, PlanId, Mode, BaseDate, Description, Registrar, CreatedAt, BranchCode)
    VALUES (s.CompanyId, s.PlanId, s.Mode, DATEADD(DAY, -s.DaysAgo, @today), s.Description, s.Json,
            DATEADD(HOUR, CASE WHEN s.DaysAgo = 0 THEN -4 ELSE 0 END, DATEADD(DAY, -s.DaysAgo, @now)), s.BranchCode)
OUTPUT s.b, inserted.Id INTO @lbIds;
UPDATE l SET Id = i.Id FROM @lb l JOIN @lbIds i ON i.b = l.b;

-- مراحل هر درخواست به ترتیب (s): از، به، مرحلهٔ پس از تأیید، دلیل، توضیح، تصمیم‌گیرنده
DECLARE @steps TABLE (n INT, s INT, [From] TINYINT NULL, [To] TINYINT, Stage TINYINT NULL, Reason NVARCHAR(100) NULL,
                      Description NVARCHAR(300) NULL, Reg VARCHAR(20), PRIMARY KEY (n, s));
INSERT @steps
SELECT x.n, x.s, x.[From], x.[To], x.Stage, x.Reason, x.Description, x.Reg
FROM @lr r
JOIN @lb b ON b.b = r.b
JOIN @co c ON c.k = b.Co
CROSS APPLY (
    -- مدیر شرکت
    SELECT r.n, 2, 1, CASE r.Status WHEN 2 THEN 2 WHEN 8 THEN 8 ELSE 3 END, NULL,
           CASE r.Status WHEN 2 THEN (SELECT Title FROM @reasons WHERE Type = 2 AND n = 1) WHEN 8 THEN (SELECT Title FROM @reasons WHERE Type = 4 AND n = 1) END,
           CASE WHEN r.Status IN (2, 8) THEN r.Note END, c.Manager
    WHERE r.Status <> 1
    UNION ALL   -- اعتبارات
    SELECT r.n, 3, 3, CASE WHEN r.Status = 4 THEN 4 WHEN r.Status IN (5, 6) OR (r.Status = 7 AND r.ViaDigital = 1) THEN 5 ELSE 7 END, NULL,
           CASE WHEN r.Status = 4 THEN (SELECT Title FROM @reasons WHERE Type = 2 AND n = 2) END,
           CASE WHEN r.Status = 4 THEN r.Note END, c.Credit
    WHERE r.Status IN (4, 5, 6, 7)
    UNION ALL   -- شعبهٔ دیجیتال
    SELECT r.n, 4, 5, CASE WHEN r.Status = 6 THEN 6 ELSE 7 END, NULL,
           CASE WHEN r.Status = 6 THEN (SELECT Title FROM @reasons WHERE Type = 2 AND n = 1) END,
           CASE WHEN r.Status = 6 THEN r.Note END, c.Digital
    WHERE r.Status = 6 OR (r.Status = 7 AND r.ViaDigital = 1)
    UNION ALL   -- مرحله‌های پس از تأیید: ۱ تا Post (برای ۴: تشکیل پرونده و بعد عدم اعطا)
    SELECT r.n, 4 + st.k, 7, 7, CASE WHEN r.Post = 4 AND st.k = 2 THEN 4 ELSE st.k END,
           CASE WHEN r.Post = 4 AND st.k = 2 THEN (SELECT Title FROM @reasons WHERE Type = 3 AND n = 1) END,
           CASE WHEN r.Post = 4 AND st.k = 2 THEN r.Note END, c.Credit
    FROM (VALUES (1), (2), (3)) st(k)
    WHERE r.Status = 7 AND r.Post IS NOT NULL AND st.k <= CASE WHEN r.Post = 4 THEN 2 ELSE r.Post END
) x(n, s, [From], [To], Stage, Reason, Description, Reg);
-- ردیف اول: ثبت درخواست
INSERT @steps (n, s, [From], [To], Stage, Reason, Description, Reg)
SELECT r.n, 1, NULL, 1, NULL, NULL, NULL, b.Reg FROM @lr r JOIN @lb b ON b.b = r.b;

DECLARE @lrIds TABLE (n INT, Id BIGINT);
MERGE panel.LoanRequests AS t
USING (SELECT r.*, b.Id AS BatchId, c.Id AS CompanyId, c.BranchCode, p.Id AS PlanId, rg.Json, pe.Id AS PersonnelId,
              CreatedAt = DATEADD(HOUR, CASE WHEN b.DaysAgo = 0 THEN -4 ELSE 0 END, DATEADD(DAY, -b.DaysAgo, @now)),
              Step = CASE WHEN b.DaysAgo >= 5 THEN 1440 ELSE 30 END,
              LastStatusStep = (SELECT MAX(s.s) FROM @steps s WHERE s.n = r.n AND s.[From] <> s.[To]),
              StepCount = (SELECT COUNT(*) - 1 FROM @steps s WHERE s.n = r.n),
              FinalReason = (SELECT TOP (1) s.Reason FROM @steps s WHERE s.n = r.n AND s.Reason IS NOT NULL ORDER BY s.s DESC)
       FROM @lr r JOIN @lb b ON b.b = r.b JOIN @co c ON c.k = b.Co JOIN @plans p ON p.k = b.PlanKey JOIN @reg rg ON rg.k = b.Reg
       JOIN @pe pe ON pe.Code = r.Code) AS s ON 1 = 0
WHEN NOT MATCHED THEN
    INSERT (BatchId, CompanyId, BranchCode, PersonnelId, PlanId, Amount, PaymentOrder, CreatedAt, DecidedAt, Status, PostStage, Description, Registrar, Version)
    VALUES (s.BatchId, s.CompanyId, s.BranchCode, s.PersonnelId, s.PlanId, s.Amount, s.PaymentOrder, s.CreatedAt,
            CASE WHEN s.LastStatusStep IS NOT NULL THEN DATEADD(MINUTE, s.Step * (s.LastStatusStep - 1), s.CreatedAt) END,
            s.Status, s.Post,
            CASE WHEN s.Status IN (2, 4, 6, 8) OR s.Post = 4 THEN s.FinalReason + N' — ' + s.Note END,
            s.Json, 1 + s.StepCount)
OUTPUT s.n, inserted.Id INTO @lrIds;
UPDATE r SET Id = i.Id FROM @lr r JOIN @lrIds i ON i.n = r.n;

INSERT panel.LoanRequestHistory (RequestId, [From], [To], Stage, Reason, Description, [By], [At])
SELECT r.Id, s.[From], s.[To], s.Stage, s.Reason, s.Description, rg.Json,
       DATEADD(MINUTE, CASE WHEN b.DaysAgo >= 5 THEN 1440 ELSE 30 END * (s.s - 1),
               DATEADD(HOUR, CASE WHEN b.DaysAgo = 0 THEN -4 ELSE 0 END, DATEADD(DAY, -b.DaysAgo, @now)))
FROM @steps s JOIN @lr r ON r.n = s.n JOIN @lb b ON b.b = r.b JOIN @reg rg ON rg.k = s.Reg;

/* ── ۴) گزارش تغییرات — Category: 1 شرکت، 2 پرسنل، 3 وام (همان عنوان‌های Handlerها) ── */
DECLARE @log TABLE ([At] DATETIME2, Reg VARCHAR(20), Category TINYINT, Action NVARCHAR(100), Details NVARCHAR(2000), Co VARCHAR(10));
-- شرکت
INSERT @log SELECT DATEADD(HOUR, 9, CAST(c.ContractDate AS DATETIME2)), c.Reg, 1, N'ثبت شرکت', N'ثبت شرکت جدید: ' + c.Name, c.k FROM @co c;
INSERT @log SELECT DATEADD(DAY, 1, DATEADD(MONTH, c.MonthsAgo - 1, CAST(c.ContractDate AS DATETIME2))), c.Reg, 1, N'ثبت میانگین موجودی',
                   c.Name + N': ' + TRANSLATE(CAST(CASE WHEN c.MonthsAgo > 12 THEN 12 ELSE c.MonthsAgo END AS VARCHAR(3)), '0123456789', N'۰۱۲۳۴۵۶۷۸۹') + N' ماه', c.k
            FROM @co c WHERE c.MonthsAgo > 0;
INSERT @log SELECT DATEADD(DAY, -20, @now), 'branch', 1, N'لغو قرارداد', N'لغو قرارداد شرکت: ' + c.Name + N' — ' + rs.Title, c.k
            FROM @co c JOIN @reasons rs ON rs.Type = 1 AND rs.n = 1 WHERE c.k = 'theta';
-- پرسنل
INSERT @log SELECT DATEADD(DAY, -p.DaysAgo, @now), c.Hr, 2, N'معرفی پرسنل', p.Name + N' (' + c.Name + N')', c.k
            FROM @pe p JOIN @co c ON c.k = p.Co WHERE p.Imported = 0;
INSERT @log SELECT DATEADD(DAY, -4, @now), c.Hr, 2, N'بارگذاری گروهی پرسنل', c.Name + N': ۳ نفر از ۵ ردیف', c.k FROM @co c WHERE c.k = 'zeta';
INSERT @log SELECT DATEADD(DAY, 1 - p.DaysAgo, @now), c.Manager, 2, CASE WHEN p.Status = 3 THEN N'رد پرسنل' ELSE N'تأیید پرسنل' END,
                   p.Name + CASE WHEN p.Status = 3 THEN N' — ' + p.Note ELSE N'' END, c.k
            FROM @pe p JOIN @co c ON c.k = p.Co WHERE p.Status <> 1;
INSERT @log SELECT DATEADD(DAY, 10 - p.DaysAgo, @now), c.Hr, 2, N'حذف پرسنل', p.Name + N' — ' + p.Note, c.k
            FROM @pe p JOIN @co c ON c.k = p.Co WHERE p.Status = 4;
-- وام
INSERT @log
SELECT DATEADD(HOUR, CASE WHEN b.DaysAgo = 0 THEN -4 ELSE 0 END, DATEADD(DAY, -b.DaysAgo, @now)), b.Reg, 3, N'ثبت درخواست وام',
       c.Name + N': ' + TRANSLATE(CAST(t.People AS VARCHAR(3)), '0123456789', N'۰۱۲۳۴۵۶۷۸۹') + N' نفر، ' + p.Title + N'، مجموع '
       + TRANSLATE(FORMAT(t.Total, 'N0', 'en-US'), '0123456789,', N'۰۱۲۳۴۵۶۷۸۹٬') + N' ریال', c.k
FROM @lb b JOIN @co c ON c.k = b.Co JOIN @plans p ON p.k = b.PlanKey
CROSS APPLY (SELECT People = COUNT(*), Total = SUM(Amount) FROM @lr r WHERE r.b = b.b) t;
INSERT @log
SELECT DATEADD(MINUTE, CASE WHEN b.DaysAgo >= 5 THEN 1440 ELSE 30 END * (s.s - 1),
               DATEADD(HOUR, CASE WHEN b.DaysAgo = 0 THEN -4 ELSE 0 END, DATEADD(DAY, -b.DaysAgo, @now))),
       s.Reg, 3,
       CASE WHEN s.[From] = s.[To] THEN N'تغییر مرحلهٔ پس از تأیید' WHEN s.[To] = 8 THEN N'لغو درخواست وام'
            WHEN s.[To] IN (2, 4, 6) THEN N'رد درخواست' ELSE N'تأیید درخواست' END,
       CASE WHEN s.[From] = s.[To] OR s.[To] = 8 THEN N'' ELSE CASE s.[From] WHEN 1 THEN N'مدیر شرکت' WHEN 3 THEN N'تأیید اعتبارات' ELSE N'شعبهٔ دیجیتال' END + N': ' END
       + N'درخواست ' + TRANSLATE(CAST(r.Id AS VARCHAR(20)), '0123456789', N'۰۱۲۳۴۵۶۷۸۹')
       + CASE WHEN s.Reason IS NOT NULL AND s.[From] <> s.[To] THEN CASE WHEN s.[To] = 8 THEN N': ' ELSE N' — ' END + s.Reason ELSE N'' END,
       c.k
FROM @steps s JOIN @lr r ON r.n = s.n JOIN @lb b ON b.b = r.b JOIN @co c ON c.k = b.Co
WHERE s.s > 1;

INSERT panel.ChangeLogs ([At], UserId, [User], Category, Action, Details, CompanyId, BranchCode)
SELECT l.[At], rg.UserId, rg.Json, l.Category, l.Action, l.Details, c.Id, c.BranchCode
FROM @log l JOIN @reg rg ON rg.k = l.Reg JOIN @co c ON c.k = l.Co
ORDER BY l.[At];

COMMIT;

/* ── گزارش ── */
SELECT c.Name AS [شرکت], c.Id AS [شناسه], c.BranchCode AS [شعبه],
       CASE c.Status WHEN 1 THEN N'فعال' WHEN 2 THEN N'لغوشده' ELSE N'منقضی' END AS [وضعیت],
       (SELECT COUNT(*) FROM @pe p WHERE p.Co = c.k) AS [پرسنل],
       (SELECT COUNT(*) FROM @lr r JOIN @lb b ON b.b = r.b WHERE b.Co = c.k) AS [درخواست وام]
FROM @co c ORDER BY c.Id;

SELECT N'شناسهٔ شرکت‌ها برای 08b_more_test_data.sql' AS [مورد],
       CONCAT(N'دلتا=', (SELECT Id FROM @co WHERE k = 'delta'), N'  زتا=', (SELECT Id FROM @co WHERE k = 'zeta')) AS [مقدار]
UNION ALL SELECT N'گزارش تغییرات', CAST((SELECT COUNT(*) FROM @log) AS NVARCHAR(10));
GO

DROP FUNCTION panel.TestData3_ToJalali;
DROP FUNCTION panel.TestData3_NationalCode;
GO
PRINT N'دادهٔ تست بیشتر Facility ثبت شد.';

/* =====================================================================
   پاک‌سازی همین دادهٔ تست (فقط همین هفت شرکت و هرچه به آن‌ها وصل است) — برای اجرا کامنت را بردارید
   =====================================================================
BEGIN TRAN;
DECLARE @ids TABLE (Id BIGINT);
INSERT @ids SELECT Id FROM panel.Companies
WHERE NationalId IN ('40404040404', '50505050505', '60606060606', '70707070707', '80808080808', '90909090909', '12121212121');
DECLARE @files TABLE (Id BIGINT);
INSERT @files SELECT LetterFileId FROM panel.CompanyCancellations WHERE CompanyId IN (SELECT Id FROM @ids);
DELETE FROM panel.ChangeLogs           WHERE CompanyId IN (SELECT Id FROM @ids);
DELETE FROM panel.LoanBatches          WHERE CompanyId IN (SELECT Id FROM @ids);   -- درخواست‌ها و تاریخچه cascade
DELETE FROM panel.Personnel            WHERE CompanyId IN (SELECT Id FROM @ids);   -- تاریخچهٔ پرسنل cascade
DELETE FROM panel.PersonnelImports     WHERE CompanyId IN (SELECT Id FROM @ids);
DELETE FROM panel.CompanyCancellations WHERE CompanyId IN (SELECT Id FROM @ids);
DELETE FROM panel.BalanceDocuments     WHERE CompanyId IN (SELECT Id FROM @ids);
DELETE FROM panel.Companies            WHERE Id IN (SELECT Id FROM @ids);          -- طرح‌ها و میانگین‌ها cascade
DELETE FROM panel.Files                WHERE Id IN (SELECT Id FROM @files);
COMMIT;
*/
