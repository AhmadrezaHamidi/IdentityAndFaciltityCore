// ─────────────────────────────────────────────────────────────────────────────
// نمونهٔ Program.cs پنل Facility — فقط خطوط [Facility] را به Program.cs فعلی اضافه کن.
// ─────────────────────────────────────────────────────────────────────────────
using FacilityService.Api;
using FacilityService.Api.Swagger;
using FacilityService.Infrastructure;
using FacilityService.Infrastructure.DBContext;
using FacilityService.Infrastructure.Seed;
using FacilityService.Infrastructure.Services.Storage;
using FacilityService.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// … تنظیمات فعلی پروژه: DbContext، MediatR، Minio، ApiVersioning …

builder.Services.AddFacilityPanelAuthorization(builder.Configuration);   // [Facility] مجوز از Identity
builder.Services.AddFacilityPanelServices(builder.Configuration);         // [Facility] IoC: Handlerها + تبدیل خطای قاعده + DbContext و فایل‌ها (اسکریپت: Database/Facility)
builder.Services.AddFacilitySwagger();                                    // [Facility] Swagger + راهنمای Flowها
builder.Services.AddFacilityPanelErrorHandling();                         // [Facility] قالب یکسان و فارسی خطاها (ورودی نامعتبر، خطای پیش‌بینی‌نشده)

var app = builder.Build();

app.UseFacilityPanelErrorHandling();                                      // [Facility] باید اولین middleware باشد

if (!app.Environment.IsProduction())
    app.UseFacilitySwagger();                                             // [Facility]
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("FacilityPanel:ApplyMigrationsOnStartup"))
    await app.Services.MigrateFacilityPanelDatabaseAsync();               // [Facility] Migrationها (SQL Server) / ساخت جداول (SQLite محلی)
if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("FacilityPanel:SeedSampleData"))
    await app.Services.SeedFacilityPanelSampleDataAsync();                // فقط توسعه: ۳ شرکت، ۱۴ پرسنل، ۸ درخواست در همهٔ وضعیت‌ها

app.UseRouting();
app.MapControllers();
app.Run();

public partial class Program;
