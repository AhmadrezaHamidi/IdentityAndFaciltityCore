namespace IdentityService.Services.ViewModels
{
    public class ResetPasswordViewModel : BaseViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RefrenceId { get; set; }
    }
}
