using IdentityService.Contracts.Transaction;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.Transactions;

public class EfTransactionManager(ApplicationDBContext dbContext) : ITransactionManager
{
    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        await action(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}