using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSchemaComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "SchemeSettings",
                schema: "panel",
                comment: "اطلاعات پایهٔ طرح — همان فرم صفحهٔ اول نمونه (دکمهٔ «ذخیره تنظیمات پایه»)");

            migrationBuilder.AlterTable(
                name: "Schemes",
                schema: "panel",
                comment: "طرح کلان — نگین میثاق، شجره طیبه، هم‌پیمان، …");

            migrationBuilder.AlterTable(
                name: "SchemePlans",
                schema: "panel",
                comment: "طرح قابل انتخاب — هر ردیف «جدول درصد قابل استفاده»؛ ۳۰ طرح در نمونه");

            migrationBuilder.AlterTable(
                name: "Reasons",
                schema: "panel",
                comment: "فهرست‌های داینامیک — دلایل و انواع مستند؛ حذف نرم");

            migrationBuilder.AlterTable(
                name: "PersonnelImports",
                schema: "panel",
                comment: "بارگذاری گروهی پرسنل — نتیجهٔ هر فایل اکسل/CSV (US-E2)");

            migrationBuilder.AlterTable(
                name: "PersonnelHistory",
                schema: "panel",
                comment: "تاریخچهٔ وضعیت پرسنل — US-E3: وضعیت‌ها با تاریخ و ساعت");

            migrationBuilder.AlterTable(
                name: "Personnel",
                schema: "panel",
                comment: "پرسنل — افراد معرفی‌شدهٔ شرکت؛ قفل پس از استفاده در وام");

            migrationBuilder.AlterTable(
                name: "MonthlyBalances",
                schema: "panel",
                comment: "میانگین موجودی ماهانه — ۱۲ ماه سال جاری قرارداد (US-B4)");

            migrationBuilder.AlterTable(
                name: "LoanRequests",
                schema: "panel",
                comment: "درخواست وام — درخواست یک نفر؛ جریان تأیید چندمرحله‌ای");

            migrationBuilder.AlterTable(
                name: "LoanRequestHistory",
                schema: "panel",
                comment: "تاریخچهٔ تصمیم‌ها — قاعدهٔ ۵: تاریخ، تصمیم‌گیرنده، وضعیت قبلی و جدید");

            migrationBuilder.AlterTable(
                name: "LoanRequestDocuments",
                schema: "panel",
                comment: "پیوست تصمیم — مصوبه/سند پیوست تصمیم اعتبارات");

            migrationBuilder.AlterTable(
                name: "LoanBatches",
                schema: "panel",
                comment: "ثبت تخصیص وام — چند درخواست هم‌طرح که با هم ثبت شدند (US-D3)");

            migrationBuilder.AlterTable(
                name: "Files",
                schema: "panel",
                comment: "فایل — متادیتای فایل؛ محتوا در Minio");

            migrationBuilder.AlterTable(
                name: "CompanyPlans",
                schema: "panel",
                comment: "طرح‌های تفاهم‌نامه — جدول واسط شرکت ↔ طرح (چند به چند)");

            migrationBuilder.AlterTable(
                name: "CompanyCancellations",
                schema: "panel",
                comment: "لغو قرارداد — مدال لغو (US-B6)؛ حداکثر یکی برای هر شرکت");

            migrationBuilder.AlterTable(
                name: "Companies",
                schema: "panel",
                comment: "شرکت — شرکت طرف تفاهم‌نامه؛ حذف نرم");

            migrationBuilder.AlterTable(
                name: "ChangeLogs",
                schema: "panel",
                comment: "گزارش تغییرات — US-A4: هر ثبت، ویرایش، لغو، حذف و تصمیم");

            migrationBuilder.AlterTable(
                name: "BalanceDocuments",
                schema: "panel",
                comment: "مستند میانگین موجودی — US-B5؛ حذف فقط ادمین");

            migrationBuilder.AlterColumn<int>(
                name: "WaitingPeriodDays",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                comment: "دورهٔ انتظار معرفی به بانک (روز)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                comment: "نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalaryFromSepahMultiplierYes",
                schema: "panel",
                table: "SchemeSettings",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                comment: "ضریب دریافت حقوق از سپه — بله (۱٫۱)",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "SalaryFromSepahMultiplierNo",
                schema: "panel",
                table: "SchemeSettings",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                comment: "ضریب دریافت حقوق از سپه — خیر (۱)",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "RepaymentTermsMonths",
                schema: "panel",
                table: "SchemeSettings",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                comment: "مدت‌های بازپرداخت (ماه)، JSON",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<decimal>(
                name: "QardHasanRate",
                schema: "panel",
                table: "SchemeSettings",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                comment: "نرخ قرض‌الحسنه (درصد) — bQarz؛ روی همهٔ طرح‌های قرض‌الحسنه",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "PersonnelIntroductionDelayMonths",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                comment: "مهلت مجاز شدن معرفی پرسنل پس از انعقاد (ماه)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MurabahaRates",
                schema: "panel",
                table: "SchemeSettings",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                comment: "نرخ‌های مرابحه، JSON: [5,10,15,20,23]",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "MaxPlansPerCompany",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                comment: "حداکثر طرح قابل انتخاب برای هر شرکت (۱ تا ۱۰)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "GraceMonthsAfterExpiry",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                comment: "مهلت استفاده از مانده پس از انقضا (ماه)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "DigitalApprovalEnabled",
                schema: "panel",
                table: "SchemeSettings",
                type: "bit",
                nullable: false,
                comment: "مرحلهٔ تأیید شعبهٔ دیجیتال فعال است؟",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "ContractTypes",
                schema: "panel",
                table: "SchemeSettings",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                comment: "انواع عقد مجاز، JSON: [1,2]",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "ContractDurationMonths",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                comment: "مدت قرارداد (ماه، ۱ تا ۱۲۰)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SchemeId",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                comment: "شناسهٔ طرح (۱ به ۱ با Schemes)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "panel",
                table: "Schemes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "نام طرح",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "panel",
                table: "Schemes",
                type: "bit",
                nullable: false,
                comment: "فعال/غیرفعال",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "panel",
                table: "Schemes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                comment: "کد یکتا؛ مثلاً NEGIN",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "panel",
                table: "Schemes",
                type: "int",
                nullable: false,
                comment: "شناسهٔ طرح (Identity)",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<decimal>(
                name: "UsablePercent",
                schema: "panel",
                table: "SchemePlans",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                comment: "درصد قابل استفاده از مجموع میانگین",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<int>(
                name: "TermMonths",
                schema: "panel",
                table: "SchemePlans",
                type: "int",
                nullable: false,
                comment: "مدت بازپرداخت (ماه)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SchemeId",
                schema: "panel",
                table: "SchemePlans",
                type: "int",
                nullable: false,
                comment: "طرح کلان",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                schema: "panel",
                table: "SchemePlans",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                comment: "نرخ (درصد)",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "panel",
                table: "SchemePlans",
                type: "bit",
                nullable: false,
                comment: "ردیف حذف‌شده از جدول غیرفعال می‌شود، نه پاک",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<byte>(
                name: "ContractType",
                schema: "panel",
                table: "SchemePlans",
                type: "tinyint",
                nullable: false,
                comment: "۱ قرض‌الحسنه، ۲ مرابحه",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "panel",
                table: "SchemePlans",
                type: "int",
                nullable: false,
                comment: "شناسهٔ طرح؛ بعد از ویرایش اطلاعات پایه ثابت می‌ماند",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "Type",
                schema: "panel",
                table: "Reasons",
                type: "tinyint",
                nullable: false,
                comment: "۱ لغو قرارداد، ۲ رد وام، ۳ عدم اعطای غیرحضوری، ۴ لغو درخواست، ۵ رد پرسنل، ۶ نوع مستند",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "panel",
                table: "Reasons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "عنوان",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                schema: "panel",
                table: "Reasons",
                type: "int",
                nullable: false,
                comment: "ترتیب نمایش",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "panel",
                table: "Reasons",
                type: "bit",
                nullable: false,
                comment: "فعال",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "panel",
                table: "Reasons",
                type: "int",
                nullable: false,
                comment: "شناسه",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "TotalRows",
                schema: "panel",
                table: "PersonnelImports",
                type: "int",
                nullable: false,
                comment: "تعداد ردیف",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Rejected",
                schema: "panel",
                table: "PersonnelImports",
                type: "int",
                nullable: false,
                comment: "ردشده",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "PersonnelImports",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "panel",
                table: "PersonnelImports",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                comment: "نام فایل",
                oldClrType: typeof(string),
                oldType: "nvarchar(260)",
                oldMaxLength: 260);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorsJson",
                schema: "panel",
                table: "PersonnelImports",
                type: "nvarchar(max)",
                nullable: false,
                comment: "خطای هر ردیف: [{row, column, message}]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "PersonnelImports",
                type: "datetime2",
                nullable: false,
                comment: "زمان",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "PersonnelImports",
                type: "bigint",
                nullable: false,
                comment: "شرکت",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Accepted",
                schema: "panel",
                table: "PersonnelImports",
                type: "int",
                nullable: false,
                comment: "ثبت‌شده",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "PersonnelImports",
                type: "bigint",
                nullable: false,
                comment: "شناسه",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "To",
                schema: "panel",
                table: "PersonnelHistory",
                type: "tinyint",
                nullable: false,
                comment: "وضعیت جدید",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                schema: "panel",
                table: "PersonnelHistory",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "دلیل",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PersonnelId",
                schema: "panel",
                table: "PersonnelHistory",
                type: "bigint",
                nullable: false,
                comment: "پرسنل",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<byte>(
                name: "From",
                schema: "panel",
                table: "PersonnelHistory",
                type: "tinyint",
                nullable: true,
                comment: "وضعیت قبلی",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "PersonnelHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "توضیح",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "By",
                schema: "panel",
                table: "PersonnelHistory",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<DateTime>(
                name: "At",
                schema: "panel",
                table: "PersonnelHistory",
                type: "datetime2",
                nullable: false,
                comment: "زمان",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "PersonnelHistory",
                type: "bigint",
                nullable: false,
                comment: "شناسه",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "panel",
                table: "Personnel",
                type: "int",
                nullable: false,
                comment: "نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                schema: "panel",
                table: "Personnel",
                type: "tinyint",
                nullable: false,
                comment: "۱ در انتظار مدیر، ۲ تأییدشده، ۳ ردشده، ۴ حذف‌شده",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                schema: "panel",
                table: "Personnel",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "علت رد",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "Personnel",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                schema: "panel",
                table: "Personnel",
                type: "int",
                nullable: false,
                comment: "اولویت تخصیص (۱ تا ۹۹۹)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PersonnelCode",
                schema: "panel",
                table: "Personnel",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                comment: "کد پرسنلی",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "NationalCode",
                schema: "panel",
                table: "Personnel",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: false,
                comment: "کد ملی؛ در هر شرکت یکتا",
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                schema: "panel",
                table: "Personnel",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                comment: "موبایل (۰۹…)",
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldUnicode: false,
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<long>(
                name: "ImportBatchId",
                schema: "panel",
                table: "Personnel",
                type: "bigint",
                nullable: true,
                comment: "اگر از فایل اکسل آمده",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "HireDate",
                schema: "panel",
                table: "Personnel",
                type: "date",
                nullable: false,
                comment: "تاریخ استخدام (آینده ممنوع)",
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                schema: "panel",
                table: "Personnel",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                comment: "نام و نام خانوادگی",
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60);

            migrationBuilder.AlterColumn<long>(
                name: "DeleteFileId",
                schema: "panel",
                table: "Personnel",
                type: "bigint",
                nullable: true,
                comment: "فایل مستند حذف",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteDescription",
                schema: "panel",
                table: "Personnel",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "توضیح حذف",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecidedAt",
                schema: "panel",
                table: "Personnel",
                type: "datetime2",
                nullable: true,
                comment: "زمان تصمیم مدیر",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "Personnel",
                type: "datetime2",
                nullable: false,
                comment: "زمان معرفی",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "Personnel",
                type: "bigint",
                nullable: false,
                comment: "شرکت",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "Personnel",
                type: "bigint",
                nullable: false,
                comment: "شناسه",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "EditedBy",
                schema: "panel",
                table: "MonthlyBalances",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EditedAt",
                schema: "panel",
                table: "MonthlyBalances",
                type: "datetime2",
                nullable: false,
                comment: "زمان ثبت/ویرایش",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                schema: "panel",
                table: "MonthlyBalances",
                type: "bigint",
                nullable: false,
                comment: "میانگین موجودی (ریال)",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Month",
                schema: "panel",
                table: "MonthlyBalances",
                type: "varchar(7)",
                unicode: false,
                maxLength: 7,
                nullable: false,
                comment: "ماه شمسی، مثل 1405/07",
                oldClrType: typeof(string),
                oldType: "varchar(7)",
                oldUnicode: false,
                oldMaxLength: 7);

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "MonthlyBalances",
                type: "bigint",
                nullable: false,
                comment: "شرکت",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "panel",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                comment: "نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                schema: "panel",
                table: "LoanRequests",
                type: "tinyint",
                nullable: false,
                comment: "۱ انتظار مدیر، ۲ رد مدیر، ۳ انتظار اعتبارات، ۴ رد اعتبارات، ۵ انتظار دیجیتال، ۶ رد دیجیتال، ۷ تأییدشده، ۸ لغوشده",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "LoanRequests",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<byte>(
                name: "PostStage",
                schema: "panel",
                table: "LoanRequests",
                type: "tinyint",
                nullable: true,
                comment: "پس از تأیید: ۱ تشکیل پرونده، ۲ دریافت از امید بانک، ۳ دریافت در شعبه، ۴ عدم اعطای غیرحضوری",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                schema: "panel",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                comment: "طرح (کپی از ثبت)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "PersonnelId",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                comment: "پرسنل",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentOrder",
                schema: "panel",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                comment: "ترتیب پرداخت (پیش‌فرض اولویت پرسنل)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "LoanRequests",
                type: "nvarchar(1200)",
                maxLength: 1200,
                nullable: true,
                comment: "دلیل و توضیح رد/لغو",
                oldClrType: typeof(string),
                oldType: "nvarchar(1200)",
                oldMaxLength: 1200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecidedAt",
                schema: "panel",
                table: "LoanRequests",
                type: "datetime2",
                nullable: true,
                comment: "زمان آخرین تصمیم",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "LoanRequests",
                type: "datetime2",
                nullable: false,
                comment: "زمان ثبت",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                comment: "شرکت (کپی از ثبت)",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                schema: "panel",
                table: "LoanRequests",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                comment: "کپی شعبهٔ شرکت، برای فیلتر دامنه بدون Join",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<long>(
                name: "BatchId",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                comment: "ثبت",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                comment: "مبلغ (۱۰ میلیون تا ۴ میلیارد ریال)",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                comment: "شناسهٔ درخواست",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "To",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "tinyint",
                nullable: false,
                comment: "وضعیت جدید",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<byte>(
                name: "Stage",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "tinyint",
                nullable: true,
                comment: "مرحلهٔ پس از تأیید (اگر تغییر مرحله بوده)",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "RequestId",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "bigint",
                nullable: false,
                comment: "درخواست",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "دلیل",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "From",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "tinyint",
                nullable: true,
                comment: "وضعیت قبلی",
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "توضیح",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "By",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<DateTime>(
                name: "At",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "datetime2",
                nullable: false,
                comment: "زمان",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "bigint",
                nullable: false,
                comment: "شناسه",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "FileId",
                schema: "panel",
                table: "LoanRequestDocuments",
                type: "bigint",
                nullable: false,
                comment: "فایل (یک فایل می‌تواند پیوست چند درخواست گروهی باشد)",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "RequestId",
                schema: "panel",
                table: "LoanRequestDocuments",
                type: "bigint",
                nullable: false,
                comment: "درخواست",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "LoanBatches",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                schema: "panel",
                table: "LoanBatches",
                type: "int",
                nullable: false,
                comment: "طرح",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "Mode",
                schema: "panel",
                table: "LoanBatches",
                type: "tinyint",
                nullable: false,
                comment: "۱ مبلغ یکسان، ۲ مبلغ جدا برای هر نفر",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "LoanBatches",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "توضیح",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "LoanBatches",
                type: "datetime2",
                nullable: false,
                comment: "زمان ثبت",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "LoanBatches",
                type: "bigint",
                nullable: false,
                comment: "شرکت",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                schema: "panel",
                table: "LoanBatches",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                comment: "کپی شعبهٔ شرکت",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BaseDate",
                schema: "panel",
                table: "LoanBatches",
                type: "date",
                nullable: false,
                comment: "تاریخ مبنا (فقط یک ماه گذشته)",
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "LoanBatches",
                type: "bigint",
                nullable: false,
                comment: "شناسهٔ ثبت",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedBy",
                schema: "panel",
                table: "Files",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                comment: "بارگذارکننده",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadedAt",
                schema: "panel",
                table: "Files",
                type: "datetime2",
                nullable: false,
                comment: "زمان بارگذاری",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "StorageKey",
                schema: "panel",
                table: "Files",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "کلید Minio: facility-panel/yyyy/MM/…",
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                schema: "panel",
                table: "Files",
                type: "bigint",
                nullable: false,
                comment: "حجم (بایت)",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "panel",
                table: "Files",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                comment: "نام اصلی",
                oldClrType: typeof(string),
                oldType: "nvarchar(260)",
                oldMaxLength: 260);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                schema: "panel",
                table: "Files",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                comment: "نوع فایل",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "Files",
                type: "bigint",
                nullable: false,
                comment: "شناسه",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                schema: "panel",
                table: "CompanyPlans",
                type: "int",
                nullable: false,
                comment: "طرح",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "CompanyPlans",
                type: "bigint",
                nullable: false,
                comment: "شرکت",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "ReasonTitle",
                schema: "panel",
                table: "CompanyCancellations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "عنوان دلیل در لحظهٔ لغو",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "ReasonId",
                schema: "panel",
                table: "CompanyCancellations",
                type: "int",
                nullable: false,
                comment: "دلیل از فهرست (نوع ۱)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "LetterFileId",
                schema: "panel",
                table: "CompanyCancellations",
                type: "bigint",
                nullable: false,
                comment: "فایل نامهٔ لغو (الزامی)",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "CompanyCancellations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "توضیح (۵ تا ۱۰۰۰)",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "CancelledBy",
                schema: "panel",
                table: "CompanyCancellations",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CancelledAt",
                schema: "panel",
                table: "CompanyCancellations",
                type: "datetime2",
                nullable: false,
                comment: "زمان ثبت لغو",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CancelDate",
                schema: "panel",
                table: "CompanyCancellations",
                type: "date",
                nullable: false,
                comment: "تاریخ لغو (۱۴۰۵/۰۱/۰۱ تا امروز)",
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "CompanyCancellations",
                type: "bigint",
                nullable: false,
                comment: "شرکت (۱ به ۰..۱)",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "panel",
                table: "Companies",
                type: "int",
                nullable: false,
                comment: "نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                schema: "panel",
                table: "Companies",
                type: "tinyint",
                nullable: false,
                comment: "۱ فعال، ۲ لغو قرارداد، ۳ منقضی، ۴ حذف‌شده",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "SchemeId",
                schema: "panel",
                table: "Companies",
                type: "int",
                nullable: false,
                comment: "طرح کلان",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "SalaryFromSepah",
                schema: "panel",
                table: "Companies",
                type: "bit",
                nullable: false,
                comment: "حقوق پرسنل از بانک سپه؟",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "Companies",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                schema: "panel",
                table: "Companies",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                comment: "شناسهٔ ملی ۱۱ رقمی؛ بین شرکت‌های حذف‌نشده یکتا",
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldUnicode: false,
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "panel",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "نام شرکت (فارسی، ۳ تا ۱۰۰)",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                schema: "panel",
                table: "Companies",
                type: "datetime2",
                nullable: true,
                comment: "زمان حذف نرم",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeleteDescription",
                schema: "panel",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "توضیح حذف",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                comment: "زمان ثبت",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ContractDate",
                schema: "panel",
                table: "Companies",
                type: "date",
                nullable: false,
                comment: "تاریخ انعقاد قرارداد",
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                schema: "panel",
                table: "Companies",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                comment: "شعبهٔ شرکت (دامنهٔ کاربر شعبه)",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "BalancesVersion",
                schema: "panel",
                table: "Companies",
                type: "int",
                nullable: false,
                comment: "نسخهٔ جدای جدول میانگین‌ها",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AgreementNumber",
                schema: "panel",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "شمارهٔ تفاهم‌نامه",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<long>(
                name: "AgreementFileId",
                schema: "panel",
                table: "Companies",
                type: "bigint",
                nullable: true,
                comment: "فایل تفاهم‌نامه",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "AccountOpeningDate",
                schema: "panel",
                table: "Companies",
                type: "date",
                nullable: false,
                comment: "افتتاح حساب سپرده؛ روزِ ماهش = روز مجاز انتقال اعتبار",
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                schema: "panel",
                table: "Companies",
                type: "varchar(24)",
                unicode: false,
                maxLength: 24,
                nullable: false,
                comment: "شمارهٔ حساب جاری (۸ تا ۲۴ رقم)",
                oldClrType: typeof(string),
                oldType: "varchar(24)",
                oldUnicode: false,
                oldMaxLength: 24);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "Companies",
                type: "bigint",
                nullable: false,
                comment: "شناسهٔ شرکت (در Identity = ScopeId دامنهٔ شرکت)",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "panel",
                table: "ChangeLogs",
                type: "int",
                nullable: false,
                comment: "کاربر پنل در Identity (بدون FK؛ سرویس دیگر)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "User",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                comment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه",
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RestoredAt",
                schema: "panel",
                table: "ChangeLogs",
                type: "datetime2",
                nullable: true,
                comment: "زمان بازگردانی",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestorePayload",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(max)",
                nullable: true,
                comment: "دادهٔ لازم برای بازگردانی",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RestoreKind",
                schema: "panel",
                table: "ChangeLogs",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                comment: "balances یا personnel-delete",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                comment: "جزئیات",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "ChangeLogs",
                type: "bigint",
                nullable: true,
                comment: "شرکت؛ null = تغییر اطلاعات پایه",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Category",
                schema: "panel",
                table: "ChangeLogs",
                type: "tinyint",
                nullable: false,
                comment: "۱ شرکت، ۲ پرسنل، ۳ وام، ۴ کاربر",
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                schema: "panel",
                table: "ChangeLogs",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                comment: "شعبهٔ شرکت (فیلتر دامنه)",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Before",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(max)",
                nullable: true,
                comment: "مقدار قبلی (JSON)",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "At",
                schema: "panel",
                table: "ChangeLogs",
                type: "datetime2",
                nullable: false,
                comment: "زمان",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "After",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(max)",
                nullable: true,
                comment: "مقدار جدید (JSON)",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "عملیات",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "ChangeLogs",
                type: "bigint",
                nullable: false,
                comment: "شناسه",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Month",
                schema: "panel",
                table: "BalanceDocuments",
                type: "varchar(7)",
                unicode: false,
                maxLength: 7,
                nullable: true,
                comment: "ماه مربوط (اختیاری)",
                oldClrType: typeof(string),
                oldType: "varchar(7)",
                oldUnicode: false,
                oldMaxLength: 7,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "FileId",
                schema: "panel",
                table: "BalanceDocuments",
                type: "bigint",
                nullable: false,
                comment: "فایل",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentTypeId",
                schema: "panel",
                table: "BalanceDocuments",
                type: "int",
                nullable: false,
                comment: "نوع مستند (دلیل نوع ۶: بنکو، نامه، …)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "BalanceDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "توضیح",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "BalanceDocuments",
                type: "bigint",
                nullable: false,
                comment: "شرکت",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "BalanceDocuments",
                type: "bigint",
                nullable: false,
                comment: "شناسه",
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "SchemeSettings",
                schema: "panel",
                oldComment: "اطلاعات پایهٔ طرح — همان فرم صفحهٔ اول نمونه (دکمهٔ «ذخیره تنظیمات پایه»)");

            migrationBuilder.AlterTable(
                name: "Schemes",
                schema: "panel",
                oldComment: "طرح کلان — نگین میثاق، شجره طیبه، هم‌پیمان، …");

            migrationBuilder.AlterTable(
                name: "SchemePlans",
                schema: "panel",
                oldComment: "طرح قابل انتخاب — هر ردیف «جدول درصد قابل استفاده»؛ ۳۰ طرح در نمونه");

            migrationBuilder.AlterTable(
                name: "Reasons",
                schema: "panel",
                oldComment: "فهرست‌های داینامیک — دلایل و انواع مستند؛ حذف نرم");

            migrationBuilder.AlterTable(
                name: "PersonnelImports",
                schema: "panel",
                oldComment: "بارگذاری گروهی پرسنل — نتیجهٔ هر فایل اکسل/CSV (US-E2)");

            migrationBuilder.AlterTable(
                name: "PersonnelHistory",
                schema: "panel",
                oldComment: "تاریخچهٔ وضعیت پرسنل — US-E3: وضعیت‌ها با تاریخ و ساعت");

            migrationBuilder.AlterTable(
                name: "Personnel",
                schema: "panel",
                oldComment: "پرسنل — افراد معرفی‌شدهٔ شرکت؛ قفل پس از استفاده در وام");

            migrationBuilder.AlterTable(
                name: "MonthlyBalances",
                schema: "panel",
                oldComment: "میانگین موجودی ماهانه — ۱۲ ماه سال جاری قرارداد (US-B4)");

            migrationBuilder.AlterTable(
                name: "LoanRequests",
                schema: "panel",
                oldComment: "درخواست وام — درخواست یک نفر؛ جریان تأیید چندمرحله‌ای");

            migrationBuilder.AlterTable(
                name: "LoanRequestHistory",
                schema: "panel",
                oldComment: "تاریخچهٔ تصمیم‌ها — قاعدهٔ ۵: تاریخ، تصمیم‌گیرنده، وضعیت قبلی و جدید");

            migrationBuilder.AlterTable(
                name: "LoanRequestDocuments",
                schema: "panel",
                oldComment: "پیوست تصمیم — مصوبه/سند پیوست تصمیم اعتبارات");

            migrationBuilder.AlterTable(
                name: "LoanBatches",
                schema: "panel",
                oldComment: "ثبت تخصیص وام — چند درخواست هم‌طرح که با هم ثبت شدند (US-D3)");

            migrationBuilder.AlterTable(
                name: "Files",
                schema: "panel",
                oldComment: "فایل — متادیتای فایل؛ محتوا در Minio");

            migrationBuilder.AlterTable(
                name: "CompanyPlans",
                schema: "panel",
                oldComment: "طرح‌های تفاهم‌نامه — جدول واسط شرکت ↔ طرح (چند به چند)");

            migrationBuilder.AlterTable(
                name: "CompanyCancellations",
                schema: "panel",
                oldComment: "لغو قرارداد — مدال لغو (US-B6)؛ حداکثر یکی برای هر شرکت");

            migrationBuilder.AlterTable(
                name: "Companies",
                schema: "panel",
                oldComment: "شرکت — شرکت طرف تفاهم‌نامه؛ حذف نرم");

            migrationBuilder.AlterTable(
                name: "ChangeLogs",
                schema: "panel",
                oldComment: "گزارش تغییرات — US-A4: هر ثبت، ویرایش، لغو، حذف و تصمیم");

            migrationBuilder.AlterTable(
                name: "BalanceDocuments",
                schema: "panel",
                oldComment: "مستند میانگین موجودی — US-B5؛ حذف فقط ادمین");

            migrationBuilder.AlterColumn<int>(
                name: "WaitingPeriodDays",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "دورهٔ انتظار معرفی به بانک (روز)");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalaryFromSepahMultiplierYes",
                schema: "panel",
                table: "SchemeSettings",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldComment: "ضریب دریافت حقوق از سپه — بله (۱٫۱)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SalaryFromSepahMultiplierNo",
                schema: "panel",
                table: "SchemeSettings",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldComment: "ضریب دریافت حقوق از سپه — خیر (۱)");

            migrationBuilder.AlterColumn<string>(
                name: "RepaymentTermsMonths",
                schema: "panel",
                table: "SchemeSettings",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200,
                oldComment: "مدت‌های بازپرداخت (ماه)، JSON");

            migrationBuilder.AlterColumn<decimal>(
                name: "QardHasanRate",
                schema: "panel",
                table: "SchemeSettings",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldComment: "نرخ قرض‌الحسنه (درصد) — bQarz؛ روی همهٔ طرح‌های قرض‌الحسنه");

            migrationBuilder.AlterColumn<int>(
                name: "PersonnelIntroductionDelayMonths",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "مهلت مجاز شدن معرفی پرسنل پس از انعقاد (ماه)");

            migrationBuilder.AlterColumn<string>(
                name: "MurabahaRates",
                schema: "panel",
                table: "SchemeSettings",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200,
                oldComment: "نرخ‌های مرابحه، JSON: [5,10,15,20,23]");

            migrationBuilder.AlterColumn<int>(
                name: "MaxPlansPerCompany",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "حداکثر طرح قابل انتخاب برای هر شرکت (۱ تا ۱۰)");

            migrationBuilder.AlterColumn<int>(
                name: "GraceMonthsAfterExpiry",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "مهلت استفاده از مانده پس از انقضا (ماه)");

            migrationBuilder.AlterColumn<bool>(
                name: "DigitalApprovalEnabled",
                schema: "panel",
                table: "SchemeSettings",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "مرحلهٔ تأیید شعبهٔ دیجیتال فعال است؟");

            migrationBuilder.AlterColumn<string>(
                name: "ContractTypes",
                schema: "panel",
                table: "SchemeSettings",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldComment: "انواع عقد مجاز، JSON: [1,2]");

            migrationBuilder.AlterColumn<int>(
                name: "ContractDurationMonths",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "مدت قرارداد (ماه، ۱ تا ۱۲۰)");

            migrationBuilder.AlterColumn<int>(
                name: "SchemeId",
                schema: "panel",
                table: "SchemeSettings",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "شناسهٔ طرح (۱ به ۱ با Schemes)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "panel",
                table: "Schemes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "نام طرح");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "panel",
                table: "Schemes",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "فعال/غیرفعال");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "panel",
                table: "Schemes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldComment: "کد یکتا؛ مثلاً NEGIN");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "panel",
                table: "Schemes",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "شناسهٔ طرح (Identity)")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<decimal>(
                name: "UsablePercent",
                schema: "panel",
                table: "SchemePlans",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldComment: "درصد قابل استفاده از مجموع میانگین");

            migrationBuilder.AlterColumn<int>(
                name: "TermMonths",
                schema: "panel",
                table: "SchemePlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "مدت بازپرداخت (ماه)");

            migrationBuilder.AlterColumn<int>(
                name: "SchemeId",
                schema: "panel",
                table: "SchemePlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "طرح کلان");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                schema: "panel",
                table: "SchemePlans",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldComment: "نرخ (درصد)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "panel",
                table: "SchemePlans",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "ردیف حذف‌شده از جدول غیرفعال می‌شود، نه پاک");

            migrationBuilder.AlterColumn<byte>(
                name: "ContractType",
                schema: "panel",
                table: "SchemePlans",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "۱ قرض‌الحسنه، ۲ مرابحه");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "panel",
                table: "SchemePlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "شناسهٔ طرح؛ بعد از ویرایش اطلاعات پایه ثابت می‌ماند")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "Type",
                schema: "panel",
                table: "Reasons",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "۱ لغو قرارداد، ۲ رد وام، ۳ عدم اعطای غیرحضوری، ۴ لغو درخواست، ۵ رد پرسنل، ۶ نوع مستند");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "panel",
                table: "Reasons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "عنوان");

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                schema: "panel",
                table: "Reasons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "ترتیب نمایش");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "panel",
                table: "Reasons",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "فعال");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                schema: "panel",
                table: "Reasons",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "شناسه")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "TotalRows",
                schema: "panel",
                table: "PersonnelImports",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "تعداد ردیف");

            migrationBuilder.AlterColumn<int>(
                name: "Rejected",
                schema: "panel",
                table: "PersonnelImports",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "ردشده");

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "PersonnelImports",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "panel",
                table: "PersonnelImports",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(260)",
                oldMaxLength: 260,
                oldComment: "نام فایل");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorsJson",
                schema: "panel",
                table: "PersonnelImports",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "خطای هر ردیف: [{row, column, message}]");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "PersonnelImports",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "PersonnelImports",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شرکت");

            migrationBuilder.AlterColumn<int>(
                name: "Accepted",
                schema: "panel",
                table: "PersonnelImports",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "ثبت‌شده");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "PersonnelImports",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسه")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "To",
                schema: "panel",
                table: "PersonnelHistory",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "وضعیت جدید");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                schema: "panel",
                table: "PersonnelHistory",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "دلیل");

            migrationBuilder.AlterColumn<long>(
                name: "PersonnelId",
                schema: "panel",
                table: "PersonnelHistory",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "پرسنل");

            migrationBuilder.AlterColumn<byte>(
                name: "From",
                schema: "panel",
                table: "PersonnelHistory",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true,
                oldComment: "وضعیت قبلی");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "PersonnelHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "توضیح");

            migrationBuilder.AlterColumn<string>(
                name: "By",
                schema: "panel",
                table: "PersonnelHistory",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<DateTime>(
                name: "At",
                schema: "panel",
                table: "PersonnelHistory",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "PersonnelHistory",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسه")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "panel",
                table: "Personnel",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                schema: "panel",
                table: "Personnel",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "۱ در انتظار مدیر، ۲ تأییدشده، ۳ ردشده، ۴ حذف‌شده");

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                schema: "panel",
                table: "Personnel",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "علت رد");

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "Personnel",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                schema: "panel",
                table: "Personnel",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "اولویت تخصیص (۱ تا ۹۹۹)");

            migrationBuilder.AlterColumn<string>(
                name: "PersonnelCode",
                schema: "panel",
                table: "Personnel",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldUnicode: false,
                oldMaxLength: 20,
                oldComment: "کد پرسنلی");

            migrationBuilder.AlterColumn<string>(
                name: "NationalCode",
                schema: "panel",
                table: "Personnel",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10,
                oldComment: "کد ملی؛ در هر شرکت یکتا");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                schema: "panel",
                table: "Personnel",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldUnicode: false,
                oldMaxLength: 11,
                oldComment: "موبایل (۰۹…)");

            migrationBuilder.AlterColumn<long>(
                name: "ImportBatchId",
                schema: "panel",
                table: "Personnel",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "اگر از فایل اکسل آمده");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "HireDate",
                schema: "panel",
                table: "Personnel",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldComment: "تاریخ استخدام (آینده ممنوع)");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                schema: "panel",
                table: "Personnel",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)",
                oldMaxLength: 60,
                oldComment: "نام و نام خانوادگی");

            migrationBuilder.AlterColumn<long>(
                name: "DeleteFileId",
                schema: "panel",
                table: "Personnel",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "فایل مستند حذف");

            migrationBuilder.AlterColumn<string>(
                name: "DeleteDescription",
                schema: "panel",
                table: "Personnel",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "توضیح حذف");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecidedAt",
                schema: "panel",
                table: "Personnel",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "زمان تصمیم مدیر");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "Personnel",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان معرفی");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "Personnel",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شرکت");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "Personnel",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسه")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "EditedBy",
                schema: "panel",
                table: "MonthlyBalances",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EditedAt",
                schema: "panel",
                table: "MonthlyBalances",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان ثبت/ویرایش");

            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                schema: "panel",
                table: "MonthlyBalances",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "میانگین موجودی (ریال)");

            migrationBuilder.AlterColumn<string>(
                name: "Month",
                schema: "panel",
                table: "MonthlyBalances",
                type: "varchar(7)",
                unicode: false,
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(7)",
                oldUnicode: false,
                oldMaxLength: 7,
                oldComment: "ماه شمسی، مثل 1405/07");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "MonthlyBalances",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شرکت");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "panel",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                schema: "panel",
                table: "LoanRequests",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "۱ انتظار مدیر، ۲ رد مدیر، ۳ انتظار اعتبارات، ۴ رد اعتبارات، ۵ انتظار دیجیتال، ۶ رد دیجیتال، ۷ تأییدشده، ۸ لغوشده");

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "LoanRequests",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<byte>(
                name: "PostStage",
                schema: "panel",
                table: "LoanRequests",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true,
                oldComment: "پس از تأیید: ۱ تشکیل پرونده، ۲ دریافت از امید بانک، ۳ دریافت در شعبه، ۴ عدم اعطای غیرحضوری");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                schema: "panel",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "طرح (کپی از ثبت)");

            migrationBuilder.AlterColumn<long>(
                name: "PersonnelId",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "پرسنل");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentOrder",
                schema: "panel",
                table: "LoanRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "ترتیب پرداخت (پیش‌فرض اولویت پرسنل)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "LoanRequests",
                type: "nvarchar(1200)",
                maxLength: 1200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1200)",
                oldMaxLength: 1200,
                oldNullable: true,
                oldComment: "دلیل و توضیح رد/لغو");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecidedAt",
                schema: "panel",
                table: "LoanRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "زمان آخرین تصمیم");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "LoanRequests",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان ثبت");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شرکت (کپی از ثبت)");

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                schema: "panel",
                table: "LoanRequests",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldComment: "کپی شعبهٔ شرکت، برای فیلتر دامنه بدون Join");

            migrationBuilder.AlterColumn<long>(
                name: "BatchId",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "ثبت");

            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "مبلغ (۱۰ میلیون تا ۴ میلیارد ریال)");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "LoanRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسهٔ درخواست")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<byte>(
                name: "To",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "وضعیت جدید");

            migrationBuilder.AlterColumn<byte>(
                name: "Stage",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true,
                oldComment: "مرحلهٔ پس از تأیید (اگر تغییر مرحله بوده)");

            migrationBuilder.AlterColumn<long>(
                name: "RequestId",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "درخواست");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "دلیل");

            migrationBuilder.AlterColumn<byte>(
                name: "From",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "tinyint",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldNullable: true,
                oldComment: "وضعیت قبلی");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "توضیح");

            migrationBuilder.AlterColumn<string>(
                name: "By",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<DateTime>(
                name: "At",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "LoanRequestHistory",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسه")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "FileId",
                schema: "panel",
                table: "LoanRequestDocuments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "فایل (یک فایل می‌تواند پیوست چند درخواست گروهی باشد)");

            migrationBuilder.AlterColumn<long>(
                name: "RequestId",
                schema: "panel",
                table: "LoanRequestDocuments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "درخواست");

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "LoanBatches",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                schema: "panel",
                table: "LoanBatches",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "طرح");

            migrationBuilder.AlterColumn<byte>(
                name: "Mode",
                schema: "panel",
                table: "LoanBatches",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "۱ مبلغ یکسان، ۲ مبلغ جدا برای هر نفر");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "LoanBatches",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "توضیح");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "LoanBatches",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان ثبت");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "LoanBatches",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شرکت");

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                schema: "panel",
                table: "LoanBatches",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldComment: "کپی شعبهٔ شرکت");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BaseDate",
                schema: "panel",
                table: "LoanBatches",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldComment: "تاریخ مبنا (فقط یک ماه گذشته)");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "LoanBatches",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسهٔ ثبت")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedBy",
                schema: "panel",
                table: "Files",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldComment: "بارگذارکننده");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadedAt",
                schema: "panel",
                table: "Files",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان بارگذاری");

            migrationBuilder.AlterColumn<string>(
                name: "StorageKey",
                schema: "panel",
                table: "Files",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldComment: "کلید Minio: facility-panel/yyyy/MM/…");

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                schema: "panel",
                table: "Files",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "حجم (بایت)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "panel",
                table: "Files",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(260)",
                oldMaxLength: 260,
                oldComment: "نام اصلی");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                schema: "panel",
                table: "Files",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldComment: "نوع فایل");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "Files",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسه")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                schema: "panel",
                table: "CompanyPlans",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "طرح");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "CompanyPlans",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شرکت");

            migrationBuilder.AlterColumn<string>(
                name: "ReasonTitle",
                schema: "panel",
                table: "CompanyCancellations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "عنوان دلیل در لحظهٔ لغو");

            migrationBuilder.AlterColumn<int>(
                name: "ReasonId",
                schema: "panel",
                table: "CompanyCancellations",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "دلیل از فهرست (نوع ۱)");

            migrationBuilder.AlterColumn<long>(
                name: "LetterFileId",
                schema: "panel",
                table: "CompanyCancellations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "فایل نامهٔ لغو (الزامی)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "CompanyCancellations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldComment: "توضیح (۵ تا ۱۰۰۰)");

            migrationBuilder.AlterColumn<string>(
                name: "CancelledBy",
                schema: "panel",
                table: "CompanyCancellations",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CancelledAt",
                schema: "panel",
                table: "CompanyCancellations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان ثبت لغو");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CancelDate",
                schema: "panel",
                table: "CompanyCancellations",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldComment: "تاریخ لغو (۱۴۰۵/۰۱/۰۱ تا امروز)");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "CompanyCancellations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شرکت (۱ به ۰..۱)");

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "panel",
                table: "Companies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "نسخهٔ همزمانی؛ با هر تغییر +۱ (در API: rowVersion)");

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                schema: "panel",
                table: "Companies",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "۱ فعال، ۲ لغو قرارداد، ۳ منقضی، ۴ حذف‌شده");

            migrationBuilder.AlterColumn<int>(
                name: "SchemeId",
                schema: "panel",
                table: "Companies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "طرح کلان");

            migrationBuilder.AlterColumn<bool>(
                name: "SalaryFromSepah",
                schema: "panel",
                table: "Companies",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "حقوق پرسنل از بانک سپه؟");

            migrationBuilder.AlterColumn<string>(
                name: "Registrar",
                schema: "panel",
                table: "Companies",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                schema: "panel",
                table: "Companies",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldUnicode: false,
                oldMaxLength: 11,
                oldComment: "شناسهٔ ملی ۱۱ رقمی؛ بین شرکت‌های حذف‌نشده یکتا");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "panel",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "نام شرکت (فارسی، ۳ تا ۱۰۰)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                schema: "panel",
                table: "Companies",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "زمان حذف نرم");

            migrationBuilder.AlterColumn<string>(
                name: "DeleteDescription",
                schema: "panel",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "توضیح حذف");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                schema: "panel",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان ثبت");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ContractDate",
                schema: "panel",
                table: "Companies",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldComment: "تاریخ انعقاد قرارداد");

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                schema: "panel",
                table: "Companies",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldComment: "شعبهٔ شرکت (دامنهٔ کاربر شعبه)");

            migrationBuilder.AlterColumn<int>(
                name: "BalancesVersion",
                schema: "panel",
                table: "Companies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "نسخهٔ جدای جدول میانگین‌ها");

            migrationBuilder.AlterColumn<string>(
                name: "AgreementNumber",
                schema: "panel",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "شمارهٔ تفاهم‌نامه");

            migrationBuilder.AlterColumn<long>(
                name: "AgreementFileId",
                schema: "panel",
                table: "Companies",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "فایل تفاهم‌نامه");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "AccountOpeningDate",
                schema: "panel",
                table: "Companies",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldComment: "افتتاح حساب سپرده؛ روزِ ماهش = روز مجاز انتقال اعتبار");

            migrationBuilder.AlterColumn<string>(
                name: "AccountNumber",
                schema: "panel",
                table: "Companies",
                type: "varchar(24)",
                unicode: false,
                maxLength: 24,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(24)",
                oldUnicode: false,
                oldMaxLength: 24,
                oldComment: "شمارهٔ حساب جاری (۸ تا ۲۴ رقم)");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "Companies",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسهٔ شرکت (در Identity = ScopeId دامنهٔ شرکت)")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "panel",
                table: "ChangeLogs",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "کاربر پنل در Identity (بدون FK؛ سرویس دیگر)");

            migrationBuilder.AlterColumn<string>(
                name: "User",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldComment: "عکس فوری کاربر (JSON): شناسه، نام، کد کاربر، نام و کد شعبه");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RestoredAt",
                schema: "panel",
                table: "ChangeLogs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "زمان بازگردانی");

            migrationBuilder.AlterColumn<string>(
                name: "RestorePayload",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "دادهٔ لازم برای بازگردانی");

            migrationBuilder.AlterColumn<string>(
                name: "RestoreKind",
                schema: "panel",
                table: "ChangeLogs",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "balances یا personnel-delete");

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldComment: "جزئیات");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "ChangeLogs",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "شرکت؛ null = تغییر اطلاعات پایه");

            migrationBuilder.AlterColumn<byte>(
                name: "Category",
                schema: "panel",
                table: "ChangeLogs",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldComment: "۱ شرکت، ۲ پرسنل، ۳ وام، ۴ کاربر");

            migrationBuilder.AlterColumn<string>(
                name: "BranchCode",
                schema: "panel",
                table: "ChangeLogs",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "شعبهٔ شرکت (فیلتر دامنه)");

            migrationBuilder.AlterColumn<string>(
                name: "Before",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "مقدار قبلی (JSON)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "At",
                schema: "panel",
                table: "ChangeLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "زمان");

            migrationBuilder.AlterColumn<string>(
                name: "After",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "مقدار جدید (JSON)");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                schema: "panel",
                table: "ChangeLogs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "عملیات");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "ChangeLogs",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسه")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Month",
                schema: "panel",
                table: "BalanceDocuments",
                type: "varchar(7)",
                unicode: false,
                maxLength: 7,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(7)",
                oldUnicode: false,
                oldMaxLength: 7,
                oldNullable: true,
                oldComment: "ماه مربوط (اختیاری)");

            migrationBuilder.AlterColumn<long>(
                name: "FileId",
                schema: "panel",
                table: "BalanceDocuments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "فایل");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentTypeId",
                schema: "panel",
                table: "BalanceDocuments",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "نوع مستند (دلیل نوع ۶: بنکو، نامه، …)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "panel",
                table: "BalanceDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "توضیح");

            migrationBuilder.AlterColumn<long>(
                name: "CompanyId",
                schema: "panel",
                table: "BalanceDocuments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شرکت");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                schema: "panel",
                table: "BalanceDocuments",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "شناسه")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
