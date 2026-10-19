using IdentityService.Domain.Entity.UserManagement.ChangeUserActiveStateHistory;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.UserManagement;

public class UserActiveChangeHistoryRepository : BaseGenericRepository<ApplicationDBContext, ChangeUserActiveStateHistory, int>, IChangeUserActiveStateRepository
{
    public UserActiveChangeHistoryRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
    }

}
