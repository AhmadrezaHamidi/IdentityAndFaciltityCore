using IdentityService.Contracts.Identities.Repositories.ActiveTokens;
using IdentityService.Domain.Entity.ActiveTokens;

namespace IdentityService.Infrastructure.Repositories.Identities.ActiveTokens;

public class ActiveTokenCommandRepository(IActiveTokenRepository activeTokenRepository) : IActiveTokenCommandRepository
{
    public async Task<long> InsertActivationCodeAsync(ActivationCodeModel token, CancellationToken cancellationToken)
    {
        await activeTokenRepository.AddActivationCodeAsync(token, cancellationToken);
        return -1;
    }

    public async Task<long> InsertActiveTokenAsync(ActiveTokenModel token, CancellationToken cancellationToken)
    {
        await activeTokenRepository.InsertTokenAsync(token, cancellationToken);
        return -1;
    }
    
    public async Task<long> InsertActiveTokenBeforeLoginAsync(ActiveTokenModelBeforeLogin token, CancellationToken cancellationToken)
    {
        await activeTokenRepository.InsertTokenBeforeLoginAsync(token, cancellationToken);
        return -1;
    }

    public async Task<bool> DeactivateActivationCodeAsync(ActivationCodeModel token, CancellationToken cancellationToken)
    {
        await activeTokenRepository.UpdateActivationCodeAsync(token, cancellationToken);
        return true;
    }

    public async Task<bool> DeactivateActiveTokenAsync(ActiveTokenModel token, CancellationToken cancellationToken)
    {
        await activeTokenRepository.DeActiveTokenAsync(token.MobileNumber, null, cancellationToken);
        return true;
    }
    
    public async Task<bool> DeactivateActiveTokenBeforeLoginAsync(ActiveTokenModelBeforeLogin token, CancellationToken cancellationToken)
    {
        await activeTokenRepository.DeActiveTokenBeforeLoginAsync(token.MobileNumber, null, cancellationToken);
        return true;
    }

    public async Task<bool> DeactivateActivationCodeByMobileNumber(string mobileNumber, CancellationToken cancellationToken)
    {
        await activeTokenRepository.UpdateActivationCodeByMobileNumber(mobileNumber, cancellationToken);
        return true;
    }
}