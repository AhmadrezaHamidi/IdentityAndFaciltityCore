namespace IdentityService.Services.ViewModels.Parameters
{
    public class MediaCenterServerSettingsViewModel : BaseViewModel
    {
        public bool IsMediaCenterServer { get; set; }
        public string MediaCenterBaseFolderPath { get; set; }
        public string MediaCenterDirectAddress { get; set; }
        public string MediaCenterServerAddress { get; set; }
        public string MediaCenterServerUserName { get; set; }
        public string MediaCenterServerPassword { get; set; }
        public string OnlineMediaCenterAdress { get; set; }
    }
}
