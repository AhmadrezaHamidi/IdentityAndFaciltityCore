using System.Collections;
using IdentityService.Domain.Entity.Basic.BankData;
using IdentityService.Domain.Entity.Basic.BankData.Dto;
using IdentityService.Domain.Entity.Basic.BankDataCity;
using IdentityService.Domain.Entity.Basic.BankDataProvince;
using IdentityService.Infrastructure.Contexts;
using OmidBank.Common.Base;
using XAct;

namespace IdentityService.Infrastructure.Repositories.Basic;

public class BankDataRepository : BaseGenericRepository<ApplicationDBContext, BankDataEntity, int>, IBankDataRepository
{
    private readonly ApplicationDBContext _applicationDBContext;

    public BankDataRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDBContext = applicationDbContext;
    }


    public async Task<List<GetBankDataItemDto>> GetBankDataAsync(Enums.OmidTechBasicDataCategoryType categoryType, CancellationToken cancellationToken)
    {
        return await _applicationDBContext.BankData.Where(a => a.CategoryTypeId == categoryType).Select(a =>
            new GetBankDataItemDto()
            {
                BankId = a.BankId,
                Title = a.Title,
            }).ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteBankDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _applicationDBContext.BankData.ExecuteDeleteAsync(cancellationToken);
            await _applicationDBContext.BankDataCity.ExecuteDeleteAsync(cancellationToken);
            await _applicationDBContext.BankDataProvince.ExecuteDeleteAsync(cancellationToken);
            await _applicationDBContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> AddBankDataAsync(BankDataTotalResult bankDataTotalResult, CancellationToken cancellationToken)
    {
        try
        {
            var deleteBankData = DeleteBankDataAsync(cancellationToken);
            if (deleteBankData.Result)
            {
                foreach (var item in bankDataTotalResult.BankDataList)
                {
                    var result = await _applicationDBContext.BankData.AddAsync(new BankDataEntity()
                    {
                        BankId = item.Item2,
                        Title = item.Item3,
                        CategoryTypeId = item.Item1,
                    }, cancellationToken);

                    if (item.Item1 == Enums.OmidTechBasicDataCategoryType.CITY)
                    {
                        var resultCity = await _applicationDBContext.BankDataCity.AddAsync(new BankDataCityEntity()
                        {
                            BankId = item.Item2,
                            Title = item.Item3.Split('-')[0].Replace(@"\s+", ""),
                            BankStateId = item.Item4 ?? ""
                        }, cancellationToken);
                    }

                    if (item.Item1 == Enums.OmidTechBasicDataCategoryType.STATE)
                    {
                        var resultProvince = await _applicationDBContext.BankDataProvince.AddAsync(new BankDataProvinceEntity()
                        {
                            BankId = item.Item2,
                            Title = item.Item3,
                        }, cancellationToken);
                    }
                }

                await _applicationDBContext.SaveChangesAsync(cancellationToken);
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}