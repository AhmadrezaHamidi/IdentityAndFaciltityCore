namespace IdentityService.Services.ViewModels
{
    public class ActivationUserViewModel : BaseViewModel
    {
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
        public string ActivationCode { get; set; }
        public string InvitedCode { get; set; }
        public string NotifyKey { get; set; }
    }
}
