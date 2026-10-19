namespace IdentityService.Services.ViewModels.InternalAuth
{
    public class FinalAuthenticationStatusViewModel : BaseViewModel
    {
        public Enums.AuthenticationStateEnum state;
        public Enums.AuthenticationResponseReasonEnum reason;
    }
}