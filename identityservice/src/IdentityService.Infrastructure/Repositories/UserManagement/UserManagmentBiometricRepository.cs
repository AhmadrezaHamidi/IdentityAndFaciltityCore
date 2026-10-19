using IdentityService.Domain.Entity.UserManagement.UserManagmentBiometric;
using IdentityService.Infrastructure.Contexts;
using XAct.Users;

namespace IdentityService.Infrastructure.Repositories.UserManagement;

public class UserManagementBiometricRepository : BaseGenericRepository<ApplicationDBContext, UserManagmentBiometricEntity, int>, IUserManagementBiometricRepository
{
    private readonly ApplicationDBContext _applicationDbContext;

    public UserManagementBiometricRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;

    }

    public async Task<UserManagmentBiometricEntity> GetByUserManagmentIdAsync(int userManagmentId, CancellationToken cancellationToken)

        => await _applicationDbContext.UserManagmentBiometric.Where(a => a.UserManagmentId == userManagmentId && a.IsActive == true)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

    public async Task DeActiveBiometricToken(long userManagmentId, CancellationToken cancellationToken)
       => await _applicationDbContext.UserManagmentBiometric.Where(a => a.UserManagmentId == userManagmentId)
                   .ExecuteUpdateAsync(a =>
                a.SetProperty(a => a.IsActive, false),
            cancellationToken);
}
