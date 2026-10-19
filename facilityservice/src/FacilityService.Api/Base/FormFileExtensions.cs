
namespace FacilityService.Api.Base;

internal static class FormFileExtensions
{
    /// <summary>IFormFile ← FileUpload (همان الگوی BulkUserImportController). سقف حجم در Handler با FileRules چک می‌شود.</summary>
    public static async Task<FileUpload?> ToUploadAsync(this IFormFile? file, CancellationToken ct)
    {
        if (file is null || file.Length == 0) return null;
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, ct);
        return new FileUpload(stream.ToArray(), Path.GetFileName(file.FileName), file.ContentType);
    }

    public static async Task<IReadOnlyList<FileUpload>> ToUploadsAsync(this IEnumerable<IFormFile>? files, CancellationToken ct)
    {
        var list = new List<FileUpload>();
        foreach (var f in files ?? [])
            if (await f.ToUploadAsync(ct) is { } u) list.Add(u);
        return list;
    }
}
