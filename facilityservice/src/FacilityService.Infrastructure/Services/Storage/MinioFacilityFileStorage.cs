using FacilityService.Contracts.Persistence;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace FacilityService.Infrastructure.Services.Storage;

/// <summary>
/// محتوای فایل‌های پنل در Minio. کلید: facility-panel/yyyy/MM/{guid}{ext}؛ نام اصلی و نوع فایل در جدول panel.Files است.
/// اگر پروژهٔ Facility سرویس Minio خودش را دارد، می‌توانی همان IMinioClient را تزریق کنی (سازندهٔ دوم).
/// </summary>
public sealed class MinioFacilityFileStorage(IMinioClient minio, string bucket) : IFacilityFileStorage
{
    public const string Prefix = "facility-panel";

    private readonly SemaphoreSlim _bucketLock = new(1, 1);
    private bool _bucketReady;

    public async Task<string> PutAsync(string fileName, string contentType, byte[] content, CancellationToken ct)
    {
        await EnsureBucketAsync(ct);
        var key = $"{Prefix}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{Path.GetExtension(fileName).ToLowerInvariant()}";
        using var stream = new MemoryStream(content, writable: false);
        await minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucket).WithObject(key).WithStreamData(stream).WithObjectSize(content.LongLength)
            .WithContentType(string.IsNullOrEmpty(contentType) ? "application/octet-stream" : contentType), ct);
        return key;
    }

    public async Task<byte[]?> GetAsync(string storageKey, CancellationToken ct)
    {
        if (!storageKey.StartsWith(Prefix + "/", StringComparison.Ordinal)) return null;
        try
        {
            using var buffer = new MemoryStream();
            await minio.GetObjectAsync(new GetObjectArgs().WithBucket(bucket).WithObject(storageKey)
                .WithCallbackStream((s, token) => s.CopyToAsync(buffer, token)), ct);
            return buffer.ToArray();
        }
        catch (ObjectNotFoundException)
        {
            return null;
        }
    }

    private async Task EnsureBucketAsync(CancellationToken ct)
    {
        if (_bucketReady) return;
        await _bucketLock.WaitAsync(ct);
        try
        {
            if (!await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), ct))
                await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), ct);
            _bucketReady = true;
        }
        finally
        {
            _bucketLock.Release();
        }
    }
}
