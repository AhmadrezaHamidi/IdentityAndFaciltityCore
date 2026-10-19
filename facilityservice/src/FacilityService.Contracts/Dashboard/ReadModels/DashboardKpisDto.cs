using System.ComponentModel.DataAnnotations;

namespace FacilityService.Contracts.Dashboard.ReadModels;

/// <summary>۴ KPI سند: شرکت فعال، کل درخواست‌ها، جمع تأییدشده، جمع در انتظار.</summary>
public sealed record DashboardKpisDto(int ActiveCompanies, int TotalRequests, long ApprovedAmount, long PendingAmount);
