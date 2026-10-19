namespace IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog
{
    public interface IUserManagementTokenLogRepository : IBaseGenericRepository<UserManagementTokenLogEntity, long>
    {
        Task<UserManagementTokenLogEntity?> FindByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken);
        Task<UserManagementTokenLogEntity?> FindByUserIdAndTokenAsync(int userId, string accessToken, CancellationToken cancellationToken);
        Task<UserManagementTokenLogEntity?> FindByTokenAsync(string accessToken, CancellationToken cancellationToken);
        Task<UserManagementTokenLogEntity?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
        //Task UpdateTokenLogWithId(long id, CancellationToken cancellationToken);
        Task<List<UserManagementTokenLogEntity>?> GetListByUserIdAsync(int userId, int take , CancellationToken cancellationToken);
        Task<UserManagementTokenLogEntity?> FindByUserIdAndTokenCheckIsActiveAsync(int userId, string accessToken, CancellationToken cancellationToken);
        Task UpdateTokenLogWithUserId(long userId, CancellationToken cancellationToken);

    }
}