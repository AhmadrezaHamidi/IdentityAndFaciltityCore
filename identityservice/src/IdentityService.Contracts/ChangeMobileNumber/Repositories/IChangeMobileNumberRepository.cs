using IdentityService.Domain.Entity.ChangeMobileNumber;

namespace IdentityService.Contracts.ChangeMobileNumber.Repositories;

public interface IChangeMobileNumberRepository
{
    Task<ChangeMobileNumberEntity> InsertAsync(ChangeMobileNumberEntity changeMobileNumber, CancellationToken cancellationToken);
    Task<ChangeMobileNumberEntity?> RetrieveByTrackIdAsync(string trackId, CancellationToken cancellationToken);
    Task<bool> ChangeMobileNumbers(string oldMobileNumber, string newMobileNumber, string nationalCode, string referenceCode, EChangeMobileNumberStatus status, CancellationToken cancellationToken);
}