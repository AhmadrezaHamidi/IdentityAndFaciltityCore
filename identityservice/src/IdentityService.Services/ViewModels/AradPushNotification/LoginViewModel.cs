namespace IdentityService.Services.ViewModels.AradPushNotification
{
    public class LoginViewModel : BaseViewModel
    {
        private string _password;
        public string UserName { get; set; }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = ***;
                //CommonHelper.CreateMD5(value);
            }
        }
    }
}
