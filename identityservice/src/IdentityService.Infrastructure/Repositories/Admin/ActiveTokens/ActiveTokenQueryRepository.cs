using IdentityService.Contracts.Admin.Repositories.ActiveTokens;

namespace IdentityService.Infrastructure.Repositories.Admin.ActiveTokens;

public class ActiveTokenQueryRepository(IActiveTokenRepository activeTokenRepository) : IActiveTokenQueryRepository
{
    public async Task<bool> VerifyActivationCodeAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken)
    {
        return await activeTokenRepository.VerifyActivationCodeAsync(activationCode, cancellationToken);
    }
}