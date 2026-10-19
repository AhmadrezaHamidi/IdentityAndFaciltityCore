using IdentityService.Domain.Entity;

namespace IdentityService.Domain.Services.CustomerManagement;

/// <summary>
/// تب «مدیریت مشتریان» پنل ادمین — سه زیرمنو طبق فلوچارت Figma:
/// اطلاعات مشتری، مسدودسازی/فعال‌سازی حساب، تغییر وضعیت دسترسی به هر خدمت.
/// مستقیم با EF روی موجودیت‌های فعلی (User/Person) بدون Stored Procedure جدید.
/// </summary>
//public interface ICustomerManagementRepository
//{
//    /// <summary>«فیلتر با کد ملی» — طبق طرح.</summary>
//    Task<(List<UserEntity> Items, int Total)> SearchCustomersAsync(
//        string? nationalCode, int page, int pageSize, CancellationToken cancellationToken);

//    Task<UserEntity?> GetCustomerAsync(int userId, CancellationToken cancellationToken);

//    /// <summary>مسدودسازی/فعال‌سازی حساب — ستون <c>BlockByAdmin</c> موجود روی User.</summary>
//    Task<bool> SetBlockStatusAsync(int userId, bool blocked, CancellationToken cancellationToken);

//    Task<List<UserServiceAccessEntity>> GetServiceAccessAsync(int userId, CancellationToken cancellationToken);

//    Task SetServiceAccessAsync(
//        int userId, string serviceKey, bool isEnabled, int? updatedByPanelUserId, CancellationToken cancellationToken);
//}
