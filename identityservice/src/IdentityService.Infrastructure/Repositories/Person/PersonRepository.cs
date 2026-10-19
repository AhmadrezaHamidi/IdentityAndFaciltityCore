using IdentityService.Domain.Entity.Person;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.Person;

public class PersonRepository : BaseGenericRepository<ApplicationDBContext, PersonEntity, int>, IPersonRepository
{
    public PersonRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
    }


    public async Task<PersonEntity> GetByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
    => await _entityContext.FirstOrDefaultAsync(a => a.NationalCode == nationalCode, cancellationToken);

    public async Task<PersonEntity> GetByIdAsync(int id, CancellationToken cancellationToken)
   => await _entityContext.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

}
