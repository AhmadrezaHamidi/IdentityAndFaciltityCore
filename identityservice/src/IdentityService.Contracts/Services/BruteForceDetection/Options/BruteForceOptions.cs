namespace IdentityService.Contracts.Services.BruteForceDetection.Options;

public class BruteForceOptions
{
    public int MaxAttempts { get; set; } = 5;
    public int LockSeconds { get; set; } = 600;
}