using System.ComponentModel.DataAnnotations;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using FacilityService.Contracts.Common;
using FacilityService.Contracts.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.ChangeLogs.Queries.ExportChangeLogs;

/// <summary>فیلترها باید همان فیلترهای لیست باشند، وگرنه خروجی با جدولی که کاربر می‌بیند فرق می‌کند.</summary>
public sealed record ExportChangeLogsQuery : FacilityBaseQuery<FacilityBaseMessageResponse<FileUpload>>
{
    public ChangeLogCategory? Category { get; init; }
    public long? CompanyId { get; init; }
    public int? PanelUserId { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }

    /// <summary>جستجوی متنی در عملیات و جزئیات؛ همان Term لیست.</summary>
    public string? Term { get; init; }
}
