using Microsoft.EntityFrameworkCore;

namespace FacilityService.Contracts.Persistence;

/// <summary>
/// دسترسی Handlerهای پنل به دیتابیس Facility. پیاده‌سازی: FacilityService.Infrastructure.AdminPanel.FacilityPanelDbContext
/// (یا DbContext فعلی پروژه که این DbSetها را اضافه کند). هر Handler فقط یک SaveChanges دارد ← هر Command اتمیک است
/// (قاعدهٔ ۳: تصمیم گروهی همه یا هیچ).
/// </summary>
public interface IFacilityPanelDbContext
{
    DbSet<Scheme> Schemes { get; }
    DbSet<SchemeSettings> SchemeSettings { get; }
    DbSet<SchemePlan> SchemePlans { get; }
    DbSet<Reason> Reasons { get; }
    DbSet<StoredFile> Files { get; }
    DbSet<Company> Companies { get; }
    DbSet<BalanceDocument> BalanceDocuments { get; }
    DbSet<Personnel> Personnel { get; }
    DbSet<PersonnelImportBatch> PersonnelImports { get; }
    DbSet<LoanBatch> LoanBatches { get; }
    DbSet<LoanRequest> LoanRequests { get; }
    DbSet<ChangeLog> ChangeLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>محتوای فایل‌ها (Minio در پروژهٔ اصلی). متادیتا در جدول Files است.</summary>
public interface IFacilityFileStorage
{
    /// <returns>کلید ذخیره‌سازی</returns>
    Task<string> PutAsync(string fileName, string contentType, byte[] content, CancellationToken ct);

    Task<byte[]?> GetAsync(string storageKey, CancellationToken ct);
}
