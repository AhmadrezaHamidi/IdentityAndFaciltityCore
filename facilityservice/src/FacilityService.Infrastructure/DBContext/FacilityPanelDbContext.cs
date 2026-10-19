using System.Text.Json;
using FacilityService.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FacilityService.Infrastructure.DBContext;

/// <summary>
/// جداول پنل Facility در اسکیمای «panel». Migrationها: Migrations/؛ اسکریپت DBA: Database/Facility/facility_panel_migrations.sql
/// اگر DbContext فعلی پروژه را ترجیح می‌دهی: DbSetها را اضافه کن و FacilityPanelModel.Configure(modelBuilder) را صدا بزن.
/// </summary>
public class FacilityPanelDbContext(DbContextOptions<FacilityPanelDbContext> options) : DbContext(options), IFacilityPanelDbContext
{
    public DbSet<Scheme> Schemes => Set<Scheme>();
    public DbSet<SchemeSettings> SchemeSettings => Set<SchemeSettings>();
    public DbSet<SchemePlan> SchemePlans => Set<SchemePlan>();
    public DbSet<Reason> Reasons => Set<Reason>();
    public DbSet<StoredFile> Files => Set<StoredFile>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<BalanceDocument> BalanceDocuments => Set<BalanceDocument>();
    public DbSet<Personnel> Personnel => Set<Personnel>();
    public DbSet<PersonnelImportBatch> PersonnelImports => Set<PersonnelImportBatch>();
    public DbSet<LoanBatch> LoanBatches => Set<LoanBatch>();
    public DbSet<LoanRequest> LoanRequests => Set<LoanRequest>();
    public DbSet<ChangeLog> ChangeLogs => Set<ChangeLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) => FacilityPanelModel.Configure(modelBuilder);
}
