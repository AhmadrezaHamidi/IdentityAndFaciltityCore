using IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;

namespace IdentityService.Domain.Entity.UserManagement.UserManagmentBiometric
{
    public interface IUserManagementBiometricRepository : IBaseGenericRepository<UserManagmentBiometricEntity, int>
    {
        Task<UserManagmentBiometricEntity> GetByUserManagmentIdAsync(int userManagmentId, CancellationToken cancellationToken);
        Task DeActiveBiometricToken(long userManagmentId, CancellationToken cancellationToken);
    }
}