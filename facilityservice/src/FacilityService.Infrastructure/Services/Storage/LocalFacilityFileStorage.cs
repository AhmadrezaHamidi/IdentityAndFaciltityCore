using FacilityService.Contracts.Persistence;

namespace FacilityService.Infrastructure.Services.Storage;

/// <summary>
/// ذخیرهٔ محتوای فایل روی دیسک. ⚠ پروژهٔ Facility از Minio استفاده می‌کند؛ IFacilityFileStorage را با سرویس Minio موجود پیاده کن.
/// </summary>
public sealed class LocalFacilityFileStorage(string root) : IFacilityFileStorage
{
    public async Task<string> PutAsync(string fileName, string contentType, byte[] content, CancellationToken ct)
    {
        var key = $"{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{Path.GetExtension(fileName).ToLowerInvariant()}";
        var path = Path.Combine(root, key);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllBytesAsync(path, content, ct);
        return key;
    }

    public async Task<byte[]?> GetAsync(string storageKey, CancellationToken ct)
    {
        var path = Path.GetFullPath(Path.Combine(root, storageKey));
        if (!path.StartsWith(Path.GetFullPath(root), StringComparison.Ordinal) || !File.Exists(path)) return null;
        return await File.ReadAllBytesAsync(path, ct);
    }
}
