using IdentityService.Domain.Entity.ActiveTokens;
using IdentityService.Domain.Models;

namespace IdentityService.Contracts.Identities.Repositories.ActiveTokens;

public interface IActiveTokenCommandRepository
{
    Task<long> InsertActivationCodeAsync(ActivationCodeModel token, CancellationToken cancellationToken);
    Task<long> InsertActiveTokenAsync(ActiveTokenModel token, CancellationToken cancellationToken);
    Task<long> InsertActiveTokenBeforeLoginAsync(ActiveTokenModelBeforeLogin token, CancellationToken cancellationToken);
    
    Task<bool> DeactivateActivationCodeAsync(ActivationCodeModel token, CancellationToken cancellationToken);
    Task<bool> DeactivateActiveTokenAsync(ActiveTokenModel token, CancellationToken cancellationToken);
    Task<bool> DeactivateActiveTokenBeforeLoginAsync(ActiveTokenModelBeforeLogin token, CancellationToken cancellationToken);

    Task<bool> DeactivateActivationCodeByMobileNumber(string mobileNumber, CancellationToken cancellationToken);
}