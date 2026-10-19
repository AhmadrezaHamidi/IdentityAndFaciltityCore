using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.UserManagementKycLog;

public class UserManagementRepository : BaseGenericRepository<ApplicationDBContext, UserManagmentEntity, int>,
    IUserManagementRepository
{
    private readonly ApplicationDBContext _applicationDbContext;

    public UserManagementRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task AddAsync2(UserManagmentEntity entity, CancellationToken cancellationToken)
    {
        await _applicationDbContext.Database.ExecuteSqlInterpolatedAsync($@"
    INSERT INTO UserManagment
    (
        UserId,
        CreateDate,
        PasswordUpdateDate,
        TakeCertificateDate,
        TosanRegister,
        AutoUpdateCif,
        TryCountAutoUpdateCif,
        AdditionalDescription
    )
    SELECT
        {entity.UserId},
        {entity.CreateDate},
        {entity.PasswordUpdateDate},
        {entity.TakeCertificateDate},
        {entity.TosanRegister},
        {entity.AutoUpdateCif},
        {entity.TryCountAutoUpdateCif},
        {entity.AdditionalDescription}
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM UserManagment
        WHERE UserId = {entity.UserId}
    );
");
    }

    public async Task<UserManagmentEntity> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        => await _entityContext.Include(a => a.User).ThenInclude(a => a.Person)
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);


    //public async Task<bool> CheckExistByUserNameAsync(string username, CancellationToken cancellationToken)
    //    => await _entityContext.AnyAsync(a => a.UserName == username, cancellationToken);

    //public async Task<UserManagmentEntity?> CheckExistByUserNameAndPasswordAsync(int userid,string username, string password, CancellationToken cancellationToken)
    //    => _entityContext.FirstOrDefault(a => a.UserId==userid && a.UserName == username && a.Password =***;

    public bool CheckExistByUserId(int userId)
        => _entityContext.Any(a => a.UserId == userId);

    //public async Task<UserManagmentEntity> GetMobileNumberByUserName(string userName, CancellationToken cancellationToken)
    //    => await _applicationDbContext.UserManagment.Where(a => a.UserName == userName)
    //        .Include(a => a.User)
    //        .ThenInclude(a => a.Person)
    //        .AsNoTracking()
    //        .FirstOrDefaultAsync(cancellationToken);

    //public async Task<UserManagmentEntity> LoginUserModern(string userName, string password, string mobileNumber, CancellationToken cancellationToken)
    //    => await _applicationDbContext.UserManagment.Include(a => a.User)
    //        .ThenInclude(a => a.Person)
    //        .Where(a => a.UserName == userName && a.Password =***;

    public async Task LogoutUM(string mobileNumber, CancellationToken cancellationToken)
        => await _applicationDbContext.UserManagementTokenLog
            .Include(a => a.User)
            .ThenInclude(a => a.Person)
            .Where(a => a.User.Person.MobileNumber == mobileNumber)
            .ExecuteUpdateAsync(a =>
                    a.SetProperty(c => c.IsActive, false),
                cancellationToken);

    //public async Task<UserManagmentEntity> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
    //    => await _entityContext.FirstOrDefaultAsync(a => a.UserName == userName, cancellationToken);
}