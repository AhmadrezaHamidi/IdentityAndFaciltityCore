using IdentityService.Domain.Models;

namespace IdentityService.Contracts.Identities.Repositories.Common;

public interface IActiveTokenBaseQueryRepository
{
    Task<bool> VerifyActivationCodeAsync(ActivationCodeModel activationCode, long validTimeSeconds, CancellationToken cancellationToken);

}