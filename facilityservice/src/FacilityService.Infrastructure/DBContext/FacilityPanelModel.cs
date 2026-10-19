using FacilityService.Contracts.Persistence;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FacilityService.Infrastructure.DBContext;

/// <summary>
/// مدل EF پنل: تنظیم هر Entity در Configuration/<Context>/<Entity>Configuration.cs؛ توضیح ستون‌ها در FacilityPanelComments.
/// اگر DbContext فعلی پروژه را ترجیح می‌دهی: DbSetها را اضافه کن و FacilityPanelModel.Configure(modelBuilder) را صدا بزن.
/// </summary>
public static class FacilityPanelModel
{
    public const string Schema = "panel";

    /// <summary>تنظیم مشترک SQL Server (برنامه، dotnet ef و تست‌ها): جدول تاریخچهٔ Migrationها در همان اسکیمای panel.</summary>
    public static void SqlServer(Microsoft.EntityFrameworkCore.Infrastructure.SqlServerDbContextOptionsBuilder sql) => sql
        .MigrationsHistoryTable("__EFMigrationsHistory", Schema)
        .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);

    internal static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    internal static readonly ValueConverter<Registrar, string> RegistrarConverter = new(
        v => JsonSerializer.Serialize(v, Json),
        v => JsonSerializer.Deserialize<Registrar>(v, Json)!);

    public static void Configure(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(typeof(FacilityPanelModel).Assembly,
            t => t.Namespace != null && t.Namespace.StartsWith("FacilityService.Infrastructure.Configuration"));
        FacilityPanelComments.Apply(b);
    }


    internal static void Versioned<T>(EntityTypeBuilder<T> e) where T : VersionedEntity
    {
        e.Property(x => x.Version).IsConcurrencyToken();
        e.Ignore(x => x.RowVersion);
    }

    internal static void RegistrarColumn(PropertyBuilder<Registrar> p) =>
        p.HasConversion(RegistrarConverter).HasMaxLength(400).IsRequired();

    internal static void JsonList<T>(PropertyBuilder<List<T>> p, int maxLength) =>
        p.HasConversion(
                v => JsonSerializer.Serialize(v, Json),
                v => JsonSerializer.Deserialize<List<T>>(v, Json) ?? new List<T>(),
                new ValueComparer<List<T>>((a, c) => a!.SequenceEqual(c!), v => v.Aggregate(0, (h, x) => HashCode.Combine(h, x)), v => v.ToList()))
            .HasMaxLength(maxLength).IsUnicode(false).IsRequired();
}
