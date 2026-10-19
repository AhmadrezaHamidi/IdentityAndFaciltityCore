using OmidBank.Common.Models;

namespace IdentityService.Domain.Models
{
    public class GeneratePersonOpenAccountModel : BaseViewModel
    {
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
    }
}