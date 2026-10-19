using System.Threading;
using IdentityService.Domain.Entity.Basic.BankData.Dto;

namespace IdentityService.Domain.Entity.Basic.BankData;

public interface IBankDataRepository : IBaseGenericRepository<BankDataEntity, int>
{
    Task<List<GetBankDataItemDto>> GetBankDataAsync(Enums.OmidTechBasicDataCategoryType categoryType,
        CancellationToken cancellationToken);

    Task<bool> AddBankDataAsync(BankDataTotalResult bankDataTotalResult, CancellationToken cancellationToken);
    Task<bool> DeleteBankDataAsync(CancellationToken cancellationToken); 
}