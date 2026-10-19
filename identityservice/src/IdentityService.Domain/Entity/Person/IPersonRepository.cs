

namespace IdentityService.Domain.Entity.Person;

public interface IPersonRepository : IBaseGenericRepository<PersonEntity, int>
{
    Task<PersonEntity> GetByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);

    Task<PersonEntity> GetByIdAsync(int id, CancellationToken cancellationToken);
}
