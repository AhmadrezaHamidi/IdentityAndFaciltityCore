using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Common;

/// <summary>فایل ذخیره‌شده برای نمایش.</summary>
public sealed record StoredFileDto(long Id, string FileName, long Size, DateTime UploadedAt, string UploadedBy);
