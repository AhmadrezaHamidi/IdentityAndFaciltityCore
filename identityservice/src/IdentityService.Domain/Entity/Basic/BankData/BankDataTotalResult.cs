using System.Collections.Specialized;
using IdentityService.Domain.Models;

namespace IdentityService.Domain.Entity.Basic.BankData.Dto;

public class BankDataTotalResult : BaseModel
{
    public BankDataResponse BankDataResponse { get; set; } = new BankDataResponse();
    public Enums.OmidTechBasicDataCategoryType CategoryTypeId { get; set; }
    public List<Tuple<Enums.OmidTechBasicDataCategoryType, string, string, string>> BankDataList { get; set; } = new();
}