namespace IdentityService.Contracts.Transaction;

public interface ITransactionManager
{
    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken);
}