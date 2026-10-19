/* =====================================================================
   فهرست‌های داینامیک به طرح تعلق می‌گیرند (سند فرانت، موارد ۱۱ تا ۱۴ فرم طرح)

   قبلاً Reasons یک فهرست سراسری بود، یعنی دلیل لغو «نگین میثاق» در کشویی
   «شجره طیبه» هم ظاهر می‌شد. حالا هر دلیل یک SchemeId دارد.

   ⚠ این اسکریپت جای Migration را نمی‌گیرد: Migration باید با
   `dotnet ef migrations add ReasonsPerScheme` ساخته شود تا Snapshot مدل هم
   به‌روز شود. این‌جا برای محیطی است که همان حالا باید اجرا شود، و دادهٔ
   موجود را هم جابه‌جا می‌کند.

   قابل اجرای چندباره (idempotent).
   ===================================================================== */

SET XACT_ABORT ON;
BEGIN TRAN;

/* ---------- ۱) ستون، با مقدار موقت صفر ---------- */

IF COL_LENGTH('panel.Reasons', 'SchemeId') IS NULL
    ALTER TABLE panel.Reasons ADD SchemeId INT NOT NULL CONSTRAINT DF_Reasons_SchemeId DEFAULT (0);
GO

/* ---------- ۲) دلایل موجود به طرح پیش‌فرض ---------- */
/* طرح پیش‌فرض: کم‌شناسه‌ترین طرح فعال. اگر چند طرح هست، همه به همین یکی می‌روند و
   بعد از اجرا باید در فرم هر طرح فهرست‌هایش را مرور کنید — چون داده‌ای وجود ندارد
   که بگوید کدام دلیل مال کدام طرح بوده. */

DECLARE @defaultScheme INT = (SELECT MIN(Id) FROM panel.Schemes WHERE IsActive = 1);

IF @defaultScheme IS NULL
    THROW 50001, N'هیچ طرح فعالی وجود ندارد؛ ابتدا یک طرح بسازید.', 1;

UPDATE panel.Reasons SET SchemeId = @defaultScheme WHERE SchemeId = 0;
GO

/* ---------- ۳) حالا که همه مقدار دارند، پیش‌فرض صفر برداشته می‌شود ---------- */

IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_Reasons_SchemeId')
    ALTER TABLE panel.Reasons DROP CONSTRAINT DF_Reasons_SchemeId;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Reasons_Schemes_SchemeId')
    ALTER TABLE panel.Reasons
        ADD CONSTRAINT FK_Reasons_Schemes_SchemeId FOREIGN KEY (SchemeId) REFERENCES panel.Schemes(Id);
GO

/* ---------- ۴) ایندکس‌ها ---------- */
/* کشویی‌ها همیشه با (طرح، نوع) خوانده می‌شوند */

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Reasons_Type_IsActive_SortOrder' AND object_id = OBJECT_ID('panel.Reasons'))
    DROP INDEX IX_Reasons_Type_IsActive_SortOrder ON panel.Reasons;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Reasons_SchemeId_Type_IsActive_SortOrder' AND object_id = OBJECT_ID('panel.Reasons'))
    CREATE INDEX IX_Reasons_SchemeId_Type_IsActive_SortOrder ON panel.Reasons(SchemeId, [Type], IsActive, SortOrder);
GO

/* عنوان تکراری در یک طرح و یک نوع پذیرفته نمی‌شود؛ غیرفعال‌ها بیرون‌اند تا بتوان
   عنوانی را که قبلاً حذف نرم شده دوباره ساخت. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Reasons_SchemeId_Type_Title' AND object_id = OBJECT_ID('panel.Reasons'))
    CREATE UNIQUE INDEX UX_Reasons_SchemeId_Type_Title ON panel.Reasons(SchemeId, [Type], Title) WHERE IsActive = 1;
GO

COMMIT;
GO
