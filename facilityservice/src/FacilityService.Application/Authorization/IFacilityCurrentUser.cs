namespace FacilityService.Application.Authorization;

/// <summary>کاربر پنل جاری که فیلتر [RequireFacilityPermission] از Identity گرفته است.</summary>
public sealed record FacilityPanelUser(
    int PanelUserId,
    string UserName,
    string? FullName,
    string? UserCode,
    string? BranchCode,
    string? BranchName,
    int PermissionVersion,
    PanelPermissions Permissions)
{
    /// <summary>«ثبت‌کننده» که روی شرکت/درخواست ذخیره می‌شود (نمونهٔ پنل: نام و کد کاربر + نام و کد شعبه).</summary>
    public Registrar ToRegistrar() => new(PanelUserId, FullName ?? UserName, UserCode, BranchName, BranchCode);
}

public interface IFacilityCurrentUser
{
    FacilityPanelUser Value { get; }
}

/// <summary>Handlerها برای چک روی منبع هدف از این استفاده می‌کنند.</summary>
public static class FacilityAccess
{
    public const string ForbiddenMessage = "به این مورد دسترسی ندارید.";

    /// <summary>مثال: if (!user.CanOn(FacilityPermissionKeys.LoansCancel, company.BranchCode, company.Id)) return 403;</summary>
    public static bool CanOn(this FacilityPanelUser user, string key, string? branchCode, object? companyId) =>
        user.Permissions.Can(key, new FacilityResource(branchCode, companyId?.ToString()));
}
