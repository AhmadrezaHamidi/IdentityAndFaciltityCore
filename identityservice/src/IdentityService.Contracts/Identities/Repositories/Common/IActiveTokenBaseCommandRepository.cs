using IdentityService.Domain.Models;

namespace IdentityService.Contracts.Identities.Repositories.Common;

public interface IActiveTokenBaseCommandRepository
{
    Task<long> InsertActivationCodeAsync(ActivationCodeModel token, CancellationToken cancellationToken);
    Task<bool> DeactivateActivationCodeAsync(ActivationCodeModel token, CancellationToken cancellationToken);
}