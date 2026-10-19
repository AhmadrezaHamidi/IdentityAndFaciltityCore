namespace IdentityService.Domain.Entity.UserManagement.LoginLogOutHistory;

public interface ILoginLogoutHistoryRepository : IBaseGenericRepository<LoginLogoutHistoryEntity, int>
{
    Task<List<LoginLogoutHistoryEntity>?> GetListByUserIdAsync(int userId, int take, CancellationToken cancellationToken);
    /// <summary>
    /// جدیدترین رویدادهای ورود/خروج در کل سیستم — برای Live Log داشبورد ادمین.
    /// بدون فیلتر کاربر، بر خلاف <see cref="GetListByUserIdAsync"/>.
    /// </summary>
    Task<List<LoginLogoutHistoryEntity>> GetRecentAsync(int take, CancellationToken cancellationToken);
}