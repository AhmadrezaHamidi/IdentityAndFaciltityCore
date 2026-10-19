namespace IdentityService.Domain.Entity;

public interface IUserManagementRepository : IBaseGenericRepository<UserManagmentEntity, int>
{
    Task<UserManagmentEntity?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
  //  Task<bool> CheckExistByUserNameAsync(string username, CancellationToken cancellationToken);
  //  Task<UserManagmentEntity?> CheckExistByUserNameAndPasswordAsync(int userid,string username, string password, CancellationToken cancellationToken);
    bool CheckExistByUserId(int userId);
   // Task<UserManagmentEntity> LoginUserModern(string userName, string password, string mobileNumber, CancellationToken cancellationToken);
  //  Task<UserManagmentEntity> GetMobileNumberByUserName(string userName, CancellationToken cancellationToken);
    Task LogoutUM(string mobileNumber, CancellationToken cancellationToken);
  //  Task<UserManagmentEntity?> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
}