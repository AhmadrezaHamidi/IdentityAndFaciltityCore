using IdentityService.Domain.Models;

namespace IdentityService.Contracts.Admin.Repositories.Common;

public interface IActiveTokenBaseQueryRepository
{
    Task<bool> VerifyActivationCodeAsync(ActivationCodeModel activationCode, long validTimeSeconds, CancellationToken cancellationToken);

}