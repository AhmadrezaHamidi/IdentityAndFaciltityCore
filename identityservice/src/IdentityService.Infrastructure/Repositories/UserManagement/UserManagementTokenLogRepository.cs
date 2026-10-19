using IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.UserManagement;

public class UserManagementTokenLogRepository : BaseGenericRepository<ApplicationDBContext, UserManagementTokenLogEntity, int>, IUserManagementTokenLogRepository
{
    private readonly ApplicationDBContext _applicationDbContext;

    public UserManagementTokenLogRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<UserManagementTokenLogEntity?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        => await _entityContext.FirstOrDefaultAsync(a => a.UserId == userId && a.IsActive == true, cancellationToken);

    public async Task<List<UserManagementTokenLogEntity>?> GetListByUserIdAsync(int userId, int take, CancellationToken cancellationToken)
        => await _applicationDbContext.UserManagementTokenLog
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Id)
            .Take(take).ToListAsync(cancellationToken);


    //public async Task UpdateTokenLogWithId(long id, CancellationToken cancellationToken)
    //    => await _applicationDbContext.UserManagementTokenLog.Where(a => a.Id == id)
    //        .ExecuteUpdateAsync(a =>
    //                a.SetProperty(a => a.IsActive, false),
    //            cancellationToken);

    public Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<UserManagementTokenLogEntity?> FindByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken)
        => await _entityContext
            .Include(a => a.User)
            .ThenInclude(a => a.Person)
            .FirstOrDefaultAsync(a => a.User.Person.MobileNumber == mobileNumber, cancellationToken);

    public async Task<UserManagementTokenLogEntity?> FindByUserIdAndTokenAsync(int userId, string accessToken, CancellationToken cancellationToken)
        => await _entityContext
            .Include(a => a.User)
            .ThenInclude(a => a.Person)
            .FirstOrDefaultAsync(a => a.UserId == userId && a.AccessToken == accessToken, cancellationToken);

    public async Task<UserManagementTokenLogEntity?> FindByTokenAsync(string accessToken, CancellationToken cancellationToken)
        => await _entityContext
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AccessToken == accessToken && a.IsActive == true, cancellationToken);


    public async Task<UserManagementTokenLogEntity?> FindByUserIdAndTokenCheckIsActiveAsync(int userId, string accessToken, CancellationToken cancellationToken)
        => await _entityContext
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId && a.AccessToken == accessToken && a.IsActive == true, cancellationToken);

    public Task<UserManagementTokenLogEntity> GetAsync(long id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateTokenLogWithUserId(long userId, CancellationToken cancellationToken)
    => await _applicationDbContext.UserManagementTokenLog.Where(a => a.UserId == userId)
        .ExecuteUpdateAsync(a =>
                a.SetProperty(a => a.IsActive, false),
            cancellationToken);


}