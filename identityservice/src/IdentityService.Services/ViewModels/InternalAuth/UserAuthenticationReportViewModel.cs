namespace IdentityService.Services.ViewModels.InternalAuth
{
    public class UserAuthenticationReportViewModel : BaseViewModel
    {
        public StatusViewModel status { get; set; }
        public string sessionId { get; set; }
        public UserInformationViewModel userInformation { get; set; }
        public FinalAuthenticationStatusViewModel authenticationStatus { get; set; }
    }
}