namespace FacilityService.Domain.Entity.Common;

/// <summary>«ثبت‌کننده / تصمیم‌گیرنده»: نام و کد کاربر + نام و کد شعبه در لحظهٔ عملیات (عکس فوری، نه کلید خارجی).</summary>
public sealed record Registrar(int PanelUserId, string UserName, string? UserCode, string? BranchName, string? BranchCode);
