using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FacilityService.Contracts.Employees.ReadModels;

/// <param name="Priority">اولویت تخصیص؛ عدد کوچک‌تر = اولویت بالاتر. فهرست با همین مرتب می‌شود.</param>
/// <param name="IsLocked">در درخواست وام استفاده شده ← ویرایش و حذف مسدود</param>
public sealed record PersonnelListItemDto(
    long Id, int Priority, string FullName, string NationalCode, string Mobile, string PersonnelCode, DateOnly HireDate,
    PersonnelStatus Status, string? RejectReason, DateTime CreatedAt, DateTime? DecidedAt, RegistrarDto Registrar, bool IsLocked);
