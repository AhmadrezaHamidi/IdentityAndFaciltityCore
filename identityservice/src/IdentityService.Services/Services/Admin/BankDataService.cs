using System.Net.Http;
using System.Net.Http.Headers;
using IdentityService.Domain.Entity.Basic.BankData;
using IdentityService.Domain.Entity.Basic.BankData.Dto;
using IdentityService.Services.IServices.Admin;
using Newtonsoft.Json;
using OmidBank.Common;
using OmidBank.Common.InternalThreadParty.OmidTech;
using OmidBank.Common.InternalThreadParty.OmidTech.Request;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace IdentityService.Services.Services.Admin;

public class BankDataService : BaseService, IBankDataService
{
    public BankDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public HttpClient _httpClient { set; get; }

    public async Task<BankDataTotalResult> CallBankDataAsync(string baseUrl, string apiKey, CancellationToken cancellationToken, HttpClient httpClient) 
    {
        try
        {
            _httpClient = httpClient;
            BankDataTotalResult totalResult = new BankDataTotalResult();
            OmidTechProvider omid = new OmidTechProvider(baseUrl, apiKey, _httpClient);

            foreach (var item in Enum.GetValues(typeof(Enums.OmidTechBasicDataCategoryType)))
            {
                var result = await omid.LoadCategoryType(new LoadCategoryTypeRequest()
                {
                    CategoryName = item.ToString()
                }, cancellationToken);

                if (result != null && result.IsSuccess)
                {
                    totalResult.CategoryTypeId = (Enums.OmidTechBasicDataCategoryType)item;
                    foreach (var element in result.ResultData)
                    {
                        totalResult.BankDataList.Add(new Tuple<Enums.OmidTechBasicDataCategoryType, string, string, string>((Enums.OmidTechBasicDataCategoryType)item, element.Id, element.Name, element.StateId));
                    }
                }
            }

            return totalResult;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}