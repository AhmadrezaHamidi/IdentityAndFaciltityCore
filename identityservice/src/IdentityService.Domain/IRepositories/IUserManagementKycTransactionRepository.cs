using IdentityService.Domain.Entity.UserManagement.UserManagementKycTransaction;

namespace IdentityService.Domain.IRepositories
{
    public interface IUserManagementKycTransactionRepository : IBaseGenericRepository<UserManagementKycTransactionEntity, int>
    {
        Task<UserManagementKycTransactionEntity?> GetByKycTokenAsync(string token, CancellationToken cancellationToken);
        Task<UserManagementKycTransactionEntity?> GetByKycUserIdAsync(int userId, CancellationToken cancellationToken);
    }
}
