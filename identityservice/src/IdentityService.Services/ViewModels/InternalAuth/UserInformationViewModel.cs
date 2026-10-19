namespace IdentityService.Services.ViewModels.InternalAuth
{
    public class UserInformationViewModel : BaseViewModel
    {
        public string nationalId { get; set; }
        public string nationalIdSerial { get; set; }
        public DateViewModel date { get; set; }
        public Enums.SexEnum sex { get; set; }
    }
}