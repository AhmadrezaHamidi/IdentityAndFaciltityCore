namespace IdentityService.Services.ViewModels.Parameters
{
    public class AuthKycParameterViewModel : BaseViewModel
    {
        public string NationalId { get; set; }
        public string MobileNumber { get; set; }
        public string NationalIdSerial { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string DownloadLink { get; set; }
    }
}
