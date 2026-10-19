using IdentityService.Contracts.Identities.Repositories.ActiveTokens;

namespace IdentityService.Infrastructure.Repositories.Identities.ActiveTokens;

public class ActiveTokenQueryRepository(IActiveTokenRepository activeTokenRepository) : IActiveTokenQueryRepository
{
    public async Task<bool> VerifyActivationCodeAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken)
    {
        return await activeTokenRepository.VerifyActivationCodeAsync(activationCode, cancellationToken);
    }
}