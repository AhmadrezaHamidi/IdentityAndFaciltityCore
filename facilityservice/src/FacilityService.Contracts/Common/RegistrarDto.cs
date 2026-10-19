using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Common;

/// <summary>«ثبت‌کننده» (نام و کد کاربر + نام و کد شعبه) — نمونهٔ پنل روی شرکت و درخواست نشان می‌دهد.</summary>
public sealed record RegistrarDto(int PanelUserId, string UserName, string? UserCode, string? BranchName, string? BranchCode);
