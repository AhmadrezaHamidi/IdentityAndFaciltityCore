using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityService.Infrastructure.Migrations
{
    /// <summary>
    /// دادهٔ اولیهٔ صفحهٔ اول نمونهٔ پنل (اطلاعات پایه): طرح «نگین میثاق»، تنظیمات، ۳۰ طرح جدول درصد قابل استفاده
    /// (ترتیب buildPlanMatrix نمونه: قرض‌الحسنه، سپس هر نرخ مرابحه با همهٔ مدت‌ها) و فهرست‌های دلایل.
    /// نرخ‌ها و درصدها «درصد» هستند. اجرای دوباره چیزی اضافه نمی‌کند. همان FacilityPanelSeed (برای SQLite محلی).
    /// </summary>
    public partial class SeedNeginMisaghScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [panel].[Schemes] WHERE [Code] = N'NEGIN')
BEGIN
    DECLARE @scheme int;
    INSERT INTO [panel].[Schemes] ([Code], [Title], [IsActive]) VALUES (N'NEGIN', N'نگین میثاق', 1);
    SET @scheme = SCOPE_IDENTITY();

    INSERT INTO [panel].[SchemeSettings] ([SchemeId], [ContractTypes], [QardHasanRate], [MurabahaRates], [RepaymentTermsMonths],
        [SalaryFromSepahMultiplierYes], [SalaryFromSepahMultiplierNo], [ContractDurationMonths], [GraceMonthsAfterExpiry],
        [MaxPlansPerCompany], [PersonnelIntroductionDelayMonths], [WaitingPeriodDays], [DigitalApprovalEnabled], [Version])
    VALUES (@scheme, '[1,2]', 4, '[5,10,15,20,23]', '[12,24,36,48,60]', 1.10, 1.00, 12, 4, 5, 1, 0, 0, 1);

    INSERT INTO [panel].[SchemePlans] ([SchemeId], [ContractType], [Rate], [TermMonths], [UsablePercent], [IsActive]) VALUES
    (@scheme, 1, 4, 12, 8, 1),
    (@scheme, 1, 4, 24, 6, 1),
    (@scheme, 1, 4, 36, 5, 1),
    (@scheme, 1, 4, 48, 4, 1),
    (@scheme, 1, 4, 60, 3, 1),
    (@scheme, 2, 5, 12, 10, 1),
    (@scheme, 2, 5, 24, 8, 1),
    (@scheme, 2, 5, 36, 6, 1),
    (@scheme, 2, 5, 48, 4, 1),
    (@scheme, 2, 5, 60, 3, 1),
    (@scheme, 2, 10, 12, 15, 1),
    (@scheme, 2, 10, 24, 10, 1),
    (@scheme, 2, 10, 36, 8, 1),
    (@scheme, 2, 10, 48, 6, 1),
    (@scheme, 2, 10, 60, 5, 1),
    (@scheme, 2, 15, 12, 25, 1),
    (@scheme, 2, 15, 24, 15, 1),
    (@scheme, 2, 15, 36, 10, 1),
    (@scheme, 2, 15, 48, 8, 1),
    (@scheme, 2, 15, 60, 6, 1),
    (@scheme, 2, 20, 12, 35, 1),
    (@scheme, 2, 20, 24, 20, 1),
    (@scheme, 2, 20, 36, 15, 1),
    (@scheme, 2, 20, 48, 10, 1),
    (@scheme, 2, 20, 60, 8, 1),
    (@scheme, 2, 23, 12, 45, 1),
    (@scheme, 2, 23, 24, 25, 1),
    (@scheme, 2, 23, 36, 20, 1),
    (@scheme, 2, 23, 48, 15, 1),
    (@scheme, 2, 23, 60, 10, 1);
END;

IF NOT EXISTS (SELECT 1 FROM [panel].[Reasons])
    INSERT INTO [panel].[Reasons] ([Type], [Title], [IsActive], [SortOrder]) VALUES
    (1, N'عدم تمدید به درخواست شرکت', 1, 0),
    (1, N'لغو قرارداد توسط مدیریت شعب منطقه', 1, 1),
    (2, N'عدم کنترل سقف فردی', 1, 0),
    (2, N'پایین بودن رتبه اعتباری', 1, 1),
    (3, N'رتبه اعتباری غیرمجاز', 1, 0),
    (3, N'عدم کفایت سقف تسهیلات بر اساس اعتبارسنجی', 1, 1),
    (4, N'انصراف متقاضی', 1, 0),
    (4, N'ترک خدمت', 1, 1),
    (5, N'اطلاعات نادرست', 1, 0),
    (5, N'عدم احراز شرایط', 1, 1),
    (6, N'تصویر بنکو', 1, 0),
    (6, N'نامه', 1, 1),
    (6, N'گردش حساب', 1, 2);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // فقط اگر هنوز شرکتی ثبت نشده (وگرنه دادهٔ پایهٔ استفاده‌شده پاک نمی‌شود)
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [panel].[Companies])
BEGIN
    DELETE FROM [panel].[Reasons];
    DELETE p FROM [panel].[SchemePlans] p JOIN [panel].[Schemes] s ON s.[Id] = p.[SchemeId] WHERE s.[Code] = N'NEGIN';
    DELETE x FROM [panel].[SchemeSettings] x JOIN [panel].[Schemes] s ON s.[Id] = x.[SchemeId] WHERE s.[Code] = N'NEGIN';
    DELETE FROM [panel].[Schemes] WHERE [Code] = N'NEGIN';
END;
");
        }
    }
}
