using IdentityService.Domain.Entity.Basic.BankDataProvince;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.Basic;

public class BankDataProvinceRepository : BaseGenericRepository<ApplicationDBContext, BankDataProvinceEntity, int>, IBankDataProvinceRepository
{
    public BankDataProvinceRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
    }

    public Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<BankDataProvinceEntity> GetAsync(long id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}