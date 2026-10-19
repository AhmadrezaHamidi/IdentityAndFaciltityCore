using IdentityService.Contracts.Identities.Repositories.ActiveTokens;
using IdentityService.Contracts.Identities.Repositories.UsernameChangeActiveToken;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.Identities.UsernameChangeActiveTokens;

public class UsernameChangeActiveTokenQueryRepository(ApplicationDBContext dbContext) : IUsernameChangeActiveTokenQueryRepository
{
    public async Task<bool> VerifyActivationCodeAsync(ActivationCodeModel activationCode, long validTimeSeconds, CancellationToken cancellationToken)
    {
        var validTime = DateTime.Now.AddSeconds(-validTimeSeconds).ToTimeStampSeconds();
        var findCode = await dbContext.ChangeUsernameActiveCodes
            .Where(x => 
                x.MobileNumber == activationCode.MobileNumber &&
                x.Code == activationCode.Code &&
                x.IsActive == true &&
                x.TimeStamp > validTime)
            .AsNoTracking()
            .OrderByDescending(x => x.TimeStamp)
            .FirstOrDefaultAsync(cancellationToken);
        return findCode != null;
    }
}
