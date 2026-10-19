using FacilityService.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace FacilityService.Infrastructure;

/// <summary>
/// Program.cs:  builder.Services.AddFacilityPanelData(builder.Configuration);
/// تنظیمات:
///   ConnectionStrings:FacilityPanel (یا Default)
///   FacilityPanel:Provider   = SqlServer (پیش‌فرض) | Sqlite (فقط اجرای محلی؛ مثلاً "Data Source=facility-panel.db")
///   FacilityPanel:Minio      = { Endpoint: "minio:9000", AccessKey, SecretKey, Bucket: "facility-panel", UseSsl: false, Region }
///                              اگر Endpoint خالی باشد، فایل‌ها روی دیسک (FacilityPanel:FileRoot) — فقط برای توسعه
/// </summary>
public static class FacilityPanelInfrastructureRegistration
{
    public static IServiceCollection AddFacilityPanelData(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("FacilityPanel") ?? configuration.GetConnectionString("Default")
                         ?? throw new InvalidOperationException("ConnectionStrings:FacilityPanel تنظیم نشده است.");
        var sqlite = string.Equals(configuration["FacilityPanel:Provider"], "Sqlite", StringComparison.OrdinalIgnoreCase);
        services.AddDbContext<FacilityPanelDbContext>(o =>
        {
            if (sqlite) o.UseSqlite(connection);
            else o.UseSqlServer(connection, FacilityPanelModel.SqlServer);
        });
        services.AddScoped<IFacilityPanelDbContext>(sp => sp.GetRequiredService<FacilityPanelDbContext>());
        var minio = configuration.GetSection("FacilityPanel:Minio");
        if (!string.IsNullOrWhiteSpace(minio["Endpoint"]))
        {
            services.AddSingleton<IFacilityFileStorage>(_ =>
            {
                IMinioClient client = new MinioClient()
                    .WithEndpoint(minio["Endpoint"])
                    .WithCredentials(minio["AccessKey"], minio["SecretKey"])
                    .WithSSL(bool.TryParse(minio["UseSsl"], out var ssl) && ssl);
                if (!string.IsNullOrWhiteSpace(minio["Region"])) client = client.WithRegion(minio["Region"]);
                return new MinioFacilityFileStorage(client.Build(), minio["Bucket"] ?? "facility-panel");
            });
        }
        else
        {
            var root = configuration["FacilityPanel:FileRoot"] ?? Path.Combine(AppContext.BaseDirectory, "App_Data", "facility-files");
            services.AddSingleton<IFacilityFileStorage>(new LocalFacilityFileStorage(root));
        }
        return services;
    }

    /// <summary>
    /// به‌روزرسانی دیتابیس هنگام شروع برنامه:
    ///   SQL Server ← اعمال Migrationهای باقی‌مانده (جداول + دادهٔ اولیهٔ طرح نگین میثاق)
    ///   SQLite (فقط اجرای محلی) ← ساخت جداول از مدل و دادهٔ اولیه
    /// اسکریپت همین Migrationها برای DBA: Database/Facility/facility_panel_migrations.sql
    /// </summary>
    public static async Task MigrateFacilityPanelDatabaseAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FacilityPanelDbContext>();
        if (db.Database.IsSqlite())
        {
            await db.Database.EnsureCreatedAsync(ct);
            await FacilityPanelSeed.EnsureSeedAsync(db, ct);
            return;
        }
        await db.Database.MigrateAsync(ct);
    }

    /// <summary>
    /// فقط توسعه: اگر شرکتی ثبت نشده، دادهٔ نمونهٔ فرانت را می‌سازد (FacilityPanelSampleData) و فایل‌های تفاهم‌نامه را در ذخیره‌ساز (Minio) می‌گذارد.
    /// بعد از آن SeedShowcaseAsync بقیهٔ وضعیت‌ها (تأییدشده، ردشده، لغوشده، پرسنل بتا) را اضافه می‌کند.
    /// Program با تنظیم FacilityPanel:SeedSampleData = true صدا می‌زند (در appsettings.Development.json روشن است).
    /// </summary>
    public static async Task SeedFacilityPanelSampleDataAsync(this IServiceProvider services, CancellationToken ct = default)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<FacilityPanelDbContext>();
        if (await db.Companies.AnyAsync(ct)) return;
        var time = scope.ServiceProvider.GetService<TimeProvider>() ?? TimeProvider.System;
        var today = FacilityClock.Today(time);
        var now = time.GetUtcNow().UtcDateTime;
        var ids = await FacilityPanelSampleData.SeedAsync(db, today, now, scope.ServiceProvider.GetRequiredService<IFacilityFileStorage>(), ct);
        await FacilityPanelSampleData.SeedShowcaseAsync(db, ids, today, now, ct);
    }
}
