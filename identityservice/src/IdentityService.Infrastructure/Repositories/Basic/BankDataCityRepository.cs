using IdentityService.Domain.Entity.Basic.BankDataCity;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.Basic;

public class BankDataCityRepository : BaseGenericRepository<ApplicationDBContext, BankDataCityEntity, int>, IBankDataCityRepository
{
    public BankDataCityRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
    }

    public async Task<List<BankDataCityEntity>> GetByBankStateIdAsync(string bankStateId, CancellationToken cancellationToken)
        => await _entityContext.Where(a => a.BankStateId == bankStateId).ToListAsync(cancellationToken);

    public async Task<string> GetProvienceIdAsync(string bankId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(bankId))
            return null;
        var result = await _entityContext.FirstOrDefaultAsync(a => a.BankId == bankId);
        return result?.BankStateId;
    }
}