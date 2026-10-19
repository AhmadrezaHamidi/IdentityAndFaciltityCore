namespace IdentityService.Domain.Models.User
{
    public class KycModel : BaseModel
    {
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
        public string NationalCodeSeries { get; set; }
        public string VideoLink { get; set; }
        public string BirthdateFarsi { get; set; }
        public string BirthdateDatetime { get; set; }
        public string Gender { get; set; }
        public string AuthenticationState { get; set; }
        public string AuthenticationResponseReason { get; set; }
    }
}
