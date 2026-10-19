namespace IdentityService.Domain.Authorization;

/// <summary>
/// کلیدهای مجوز ماژول «مدیریت کاربران» پنل.
///
/// هر کلید باید یک ردیف PanelMenu با همین PermissionKey داشته باشد
/// (Database/02_seed_permissions_and_matrix.sql آن‌ها را می‌سازد).
///
/// کلید منتشرشده هرگز تغییر نام نمی‌دهد و حذف نمی‌شود — فقط غیرفعال می‌شود.
/// عوض کردن یک کلید یعنی هر نقشی که آن را داشت، بی‌صدا دسترسی‌اش را از دست می‌دهد.
/// </summary>
public static class AdminPermissionKeys
{
    public const string PermissionsRead = "admin.permissions.read";
    public const string PermissionsWrite = "admin.permissions.write";

    public const string RolesRead = "admin.roles.read";
    public const string RolesWrite = "admin.roles.write";

    public const string GroupsRead = "admin.groups.read";
    public const string GroupsWrite = "admin.groups.write";

    public const string UsersRead = "admin.users.read";
    public const string UsersWrite = "admin.users.write";
    public const string UsersAssign = "admin.users.assign";
    public const string UsersResetPassword = "admin.users.reset-password";
    public const string UsersRevokeSessions = "admin.users.revoke-sessions";

    public const string AuditRead = "admin.audit.read";
}
