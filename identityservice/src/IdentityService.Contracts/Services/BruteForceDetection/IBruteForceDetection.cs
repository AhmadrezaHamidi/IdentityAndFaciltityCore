using IdentityService.Contracts.Services.BruteForceDetection.Enums;

namespace IdentityService.Contracts.Services.BruteForceDetection;

public interface IBruteForceDetection
{
    public Task<EBruteForceStatus> IsBruteForceDetectedAsync(string phoneNumber, string ip, CancellationToken cancellationToken);
}