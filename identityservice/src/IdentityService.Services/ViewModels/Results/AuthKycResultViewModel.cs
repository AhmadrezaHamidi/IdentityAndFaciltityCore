namespace IdentityService.Services.ViewModels.Results
{
    public class AuthKycResultViewModel : BaseViewModel
    {
        public string SessionId { get; set; }
        public string Error { get; set; }
    }
}
