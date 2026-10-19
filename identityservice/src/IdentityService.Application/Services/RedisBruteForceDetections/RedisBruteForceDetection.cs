using IdentityService.Contracts.Services.BruteForceDetection;
using IdentityService.Contracts.Services.BruteForceDetection.Enums;
using IdentityService.Contracts.Services.BruteForceDetection.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace IdentityService.Application.Services.RedisBruteForceDetections;

public class RedisBruteForceDetection(
    IConnectionMultiplexer connectionMultiplexer,
    IOptions<BruteForceOptions> options) : IBruteForceDetection
{
    private readonly IDatabase _redis = connectionMultiplexer.GetDatabase();
    private readonly string _luaScript = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Scripts", "CheckLockoutByPhoneAndIp.lua"));
    public async Task<EBruteForceStatus> IsBruteForceDetectedAsync(string phoneNumber, string ip, CancellationToken cancellationToken)
    {
        // 1. Check Global Lock in Redis
        if (await _redis.KeyExistsAsync($"lock:{phoneNumber}"))
            return EBruteForceStatus.Banned;

        // 2. Atomic Rate Limit Check via Lua
        var isAllowed = await _redis.ScriptEvaluateAsync(_luaScript,
            [$"attempts:mob:{phoneNumber}", $"attempts:ip:{ip}", $"lock:{phoneNumber}"], [options.Value.MaxAttempts, options.Value.LockSeconds]);

        return (int)isAllowed == 0 ? EBruteForceStatus.TooManyRequests : EBruteForceStatus.Ok;
    }
}