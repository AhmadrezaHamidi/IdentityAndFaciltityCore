using IdentityService.Domain.Entity.UserManagement.UserManagementKycTransaction;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.UserManagement;

public class UserManagementKycTransactionRepository(ApplicationDBContext applicationDbContext) :
    BaseGenericRepository<ApplicationDBContext, UserManagementKycTransactionEntity, int>(applicationDbContext),
    IUserManagementKycTransactionRepository
{

    public async Task<UserManagementKycTransactionEntity?> GetByKycTokenAsync(string token, CancellationToken cancellationToken)
    {
        var entity = await applicationDbContext.UserManagementKycTransaction.Where(x => x.Token == token).FirstOrDefaultAsync(cancellationToken);
        return entity;
    }

    public async Task<UserManagementKycTransactionEntity?> GetByKycUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        var entity = await applicationDbContext.UserManagementKycTransaction.Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
        return entity;
    }
}
