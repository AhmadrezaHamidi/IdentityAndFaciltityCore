using IdentityService.Domain.Models;

namespace IdentityService.Contracts.Identities.Repositories.ActiveTokens;

public interface IActiveTokenQueryRepository
{
    Task<bool> VerifyActivationCodeAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken);
}