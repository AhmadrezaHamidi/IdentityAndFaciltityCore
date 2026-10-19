namespace IdentityService.Contracts.Services.BruteForceDetection.Enums;

public enum EBruteForceStatus
{
    Ok,
    TooManyRequests,
    Banned,
}