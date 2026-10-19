using AutoMapper;
using IdentityService.Contracts.Identities.Repositories.PasswordChangeActiveToken;
using IdentityService.Infrastructure.Contexts;
using IdentityService.Infrastructure.Entities.Identities;

namespace IdentityService.Infrastructure.Repositories.Identities.PasswordChangeActiveTokens;

public class PasswordChangeActiveTokenCommandRepository(
    ApplicationDBContext dbContext,
    IMapper mapper) : IPasswordChangeActiveTokenCommandRepository
{
    public async Task<long> InsertActivationCodeAsync(ActivationCodeModel token, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<ChangePasswordActiveCodes>(token);
        var item = await dbContext.ChangePasswordActiveCodes.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return item.Entity.Id;
    }

    public async Task<bool> DeactivateActivationCodeAsync(ActivationCodeModel token, CancellationToken cancellationToken)
    {
        var query = dbContext.ChangePasswordActiveCodes.Where(x => x.MobileNumber == token.MobileNumber);
        query = token.Code is null ? query.Where(x => x.IsActive == true) : query.Where(x => x.Code == token.Code);
        var entities = await query.ToListAsync(cancellationToken);
        foreach (var entity in entities)
            entity.IsActive = false;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}