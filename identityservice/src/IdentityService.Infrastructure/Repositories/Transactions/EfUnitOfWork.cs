using IdentityService.Contracts.Transaction;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.Transactions;

public class EfUnitOfWork(ApplicationDBContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}