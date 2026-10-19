namespace IdentityService.Services.ViewModels.AradPushNotification
{
    public class AradAddUserViewModel : BaseViewModel
    {
        public  string Mobile { get; set; }
        public int Type { get; set; }
        public  string OsName { get; set; }
        public  string BrowserName { get; set; }
        public  string DeviceName { get; set; }
        public  string DeviceVersion { get; set; }
        public  string ApplicationVersion { get; set; }
        public  string FcmToken { get; set; }
    }
}
