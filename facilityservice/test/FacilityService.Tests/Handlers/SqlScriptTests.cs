using FacilityService.Infrastructure;
using FacilityService.Infrastructure.DBContext;
using FacilityService.Infrastructure.Seed;
using FacilityService.Infrastructure.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FacilityService.Tests.Handlers;

/// <summary>
/// Migrationها باید با مدل EF یکی باشند و اسکریپت SQL ِ DBA با Migrationها. بعد از تغییر مدل:
///   dotnet ef migrations add &lt;Name&gt; -p src/FacilityService.Infrastructure -s src/FacilityService.Infrastructure -o AdminPanel/Migrations
///   UPDATE_SQL=1 dotnet test --filter SqlScriptTests
/// </summary>
public class SqlScriptTests
{
    private static FacilityPanelDbContext SqlServerContext() => new(new DbContextOptionsBuilder<FacilityPanelDbContext>()
        .UseSqlServer("Server=.;Database=Facility;Trusted_Connection=True", FacilityPanelModel.SqlServer).Options);

    private static string RepoFile(string relative)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "facilityservice", "FacilityService.sln"))) dir = dir.Parent;
        return Path.Combine(dir!.FullName, relative);
    }

    [Fact]
    public void Model_has_no_changes_missing_from_migrations()
    {
        using var db = SqlServerContext();
        var snapshot = db.GetService<IMigrationsAssembly>().ModelSnapshot?.Model
                       ?? throw new InvalidOperationException("ModelSnapshot نیست.");
        var initializer = db.GetService<IModelRuntimeInitializer>();
        if (snapshot is IMutableModel mutable) snapshot = mutable.FinalizeModel();
        snapshot = initializer.Initialize(snapshot);
        var differences = db.GetService<IMigrationsModelDiffer>().GetDifferences(
            snapshot.GetRelationalModel(), db.GetService<IDesignTimeModel>().Model.GetRelationalModel());
        Assert.True(differences.Count == 0,
            "مدل EF تغییری دارد که Migration ندارد: " + string.Join("، ", differences.Select(d => d.GetType().Name)));
    }

    [Fact]
    public void Migration_script_matches_migrations()
    {
        using var db = SqlServerContext();
        var script = "-- جداول و دادهٔ اولیهٔ پنل Facility (طرح نگین میثاق) — اسکریپت idempotent همهٔ Migrationها؛ اجرای دوباره بی‌اثر است.\n" +
                     "-- تولید: UPDATE_SQL=1 dotnet test --filter SqlScriptTests  (دستی ویرایش نکن)\n\n" +
                     db.GetService<IMigrator>().GenerateScript(options: MigrationsSqlGenerationOptions.Idempotent).Replace("\r\n", "\n").Trim() + "\n";
        var path = RepoFile("facilityservice/Database/facility_panel_migrations.sql");
        if (Environment.GetEnvironmentVariable("UPDATE_SQL") == "1") File.WriteAllText(path, script);
        Assert.True(File.Exists(path), "اسکریپت وجود ندارد؛ با UPDATE_SQL=1 بساز.");
        Assert.Equal(script, File.ReadAllText(path).Replace("\r\n", "\n"));
    }
}
