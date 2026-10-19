using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Common;

/// <summary>فایل آپلودی که کنترلر از IFormFile می‌سازد (همان الگوی UploadExcelCommand).</summary>
public sealed record FileUpload(byte[] Content, string FileName, string ContentType)
{
    public long Length => Content.LongLength;
}
