using IdentityService.Domain.Entity.Basic.BankData;
using IdentityService.Domain.Entity.Basic.BankData.Dto;

namespace IdentityService.Services.IServices.Admin;

public interface IBankDataService : IBaseService
{
    Task<BankDataTotalResult> CallBankDataAsync(string baseUrl, string apiKey, CancellationToken cancellationToken, HttpClient httpClient); 
}