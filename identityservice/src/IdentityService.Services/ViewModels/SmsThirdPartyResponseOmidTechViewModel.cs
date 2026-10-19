namespace IdentityService.Services.ViewModels
{
    public class SmsThirdPartyResponseOmidTechViewModel : BaseViewModel
    {
        public bool isSuccess { get; set; }
        public string message { get; set; }
        public int rsCode { get; set; }
    }
}