using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FacilityService.Infrastructure.DBContext;

/// <summary>
/// فقط برای ابزار dotnet ef (ساخت Migration و اسکریپت). اتصال واقعی لازم نیست مگر برای «database update»:
///   FACILITY_PANEL_CONNECTION="Server=...;Database=Facility;..." dotnet ef database update -p src/FacilityService.Infrastructure
/// </summary>
public sealed class FacilityPanelDbContextFactory : IDesignTimeDbContextFactory<FacilityPanelDbContext>
{
    public FacilityPanelDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("FACILITY_PANEL_CONNECTION")
                         ?? "Server=localhost;Database=Facility;Trusted_Connection=True;TrustServerCertificate=True";
        return new FacilityPanelDbContext(new DbContextOptionsBuilder<FacilityPanelDbContext>()
            .UseSqlServer(connection, FacilityPanelModel.SqlServer).Options);
    }
}
