namespace IdentityService.Domain.Entity.Basic.BankDataCity
{
    public interface IBankDataCityRepository : IBaseGenericRepository<BankDataCityEntity, int>
    {
        Task<List<BankDataCityEntity>> GetByBankStateIdAsync(string bankStateId, CancellationToken cancellationToken);

        Task<string> GetProvienceIdAsync(string bankId, CancellationToken cancellationToken);
    }
}