namespace FacilityService.Domain.Entity.Common;

/// <summary>متادیتای فایل؛ محتوا در فضای ذخیره‌سازی (Minio) با StorageKey.</summary>
public class StoredFile
{
    public long Id { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long Size { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public DateTime UploadedAt { get; private set; }
    public string UploadedBy { get; private set; } = null!;

    private StoredFile() { }

    public StoredFile(string fileName, string contentType, long size, string storageKey, DateTime uploadedAt, string uploadedBy)
    {
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        StorageKey = storageKey;
        UploadedAt = uploadedAt;
        UploadedBy = uploadedBy;
    }
}
