using IdentityService.Domain.Entity.UserManagement.LoginLogOutHistory;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.UserManagement;

public class LoginLogoutHistoryRepository: BaseGenericRepository<ApplicationDBContext,LoginLogoutHistoryEntity, int>, ILoginLogoutHistoryRepository
{
    private readonly ApplicationDBContext _applicationDbContext;

    public LoginLogoutHistoryRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<List<LoginLogoutHistoryEntity>?> GetListByUserIdAsync(int userId, int take, CancellationToken cancellationToken)
        => await _applicationDbContext.LoginLogoutHistory
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Id)
            .Take(take).ToListAsync(cancellationToken);


    public async Task<List<LoginLogoutHistoryEntity>> GetRecentAsync(int take, CancellationToken cancellationToken)
     => await _applicationDbContext.LoginLogoutHistory
         .OrderByDescending(a => a.Id)
         .Take(take)
         .ToListAsync(cancellationToken);
}