using FacilityService.Contracts.Persistence;
using FacilityService.Infrastructure;
using FacilityService.Infrastructure.DBContext;
using FacilityService.Infrastructure.Seed;
using FacilityService.Infrastructure.Services.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FacilityService.Tests.Handlers;

/// <summary>
/// ذخیرهٔ فایل در Minio (یا هر سرور سازگار با S3). اجرا:
///   FACILITY_TEST_S3=localhost:9000 FACILITY_TEST_S3_KEY=... FACILITY_TEST_S3_SECRET=... dotnet test --filter MinioStorageTests
/// </summary>
public class MinioStorageTests
{
    private static IFacilityFileStorage Storage()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            ["ConnectionStrings:FacilityPanel"] = "Data Source=unused",
            ["FacilityPanel:Provider"] = "Sqlite",
            ["FacilityPanel:Minio:Endpoint"] = Environment.GetEnvironmentVariable("FACILITY_TEST_S3"),
            ["FacilityPanel:Minio:AccessKey"] = Environment.GetEnvironmentVariable("FACILITY_TEST_S3_KEY") ?? "test",
            ["FacilityPanel:Minio:SecretKey"] = Environment.GetEnvironmentVariable("FACILITY_TEST_S3_SECRET") ?? "testsecret",
            ["FacilityPanel:Minio:Bucket"] = "facility-panel-test",
            ["FacilityPanel:Minio:Region"] = "us-east-1",
        }).Build();
        return new ServiceCollection().AddFacilityPanelData(config).BuildServiceProvider().GetRequiredService<IFacilityFileStorage>();
    }

    [EnvFact("FACILITY_TEST_S3")]
    public async Task Put_and_get_round_trip_through_minio()
    {
        var storage = Storage();
        Assert.IsType<MinioFacilityFileStorage>(storage);
        var content = System.Text.Encoding.UTF8.GetBytes("نامهٔ لغو قرارداد");

        var key = await storage.PutAsync("نامه.PDF", "application/pdf", content, default);
        Assert.StartsWith("facility-panel/", key);
        Assert.EndsWith(".pdf", key);
        Assert.Equal(content, await storage.GetAsync(key, default));

        Assert.Null(await storage.GetAsync("facility-panel/2026/10/missing.pdf", default));
        Assert.Null(await storage.GetAsync("../etc/passwd", default));
    }

    [Fact]
    public void Without_minio_endpoint_files_go_to_local_disk()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            ["ConnectionStrings:FacilityPanel"] = "Data Source=unused",
        }).Build();
        var storage = new ServiceCollection().AddFacilityPanelData(config).BuildServiceProvider().GetRequiredService<IFacilityFileStorage>();
        Assert.IsType<LocalFacilityFileStorage>(storage);
    }
}
