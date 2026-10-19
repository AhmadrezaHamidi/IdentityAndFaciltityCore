namespace IdentityService.Services.ViewModels
{
    public class PanelAuthenticateRequestViewModel : BaseViewModel
    {
        public string Token { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}
