using static OmidBank.Common.Enums;

namespace IdentityService.Domain.Entity.Basic.BankData
{
    public partial class BankDataEntity
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string BankId { get; set; }
        public OmidTechBasicDataCategoryType CategoryTypeId { get; set; }
    }
}
