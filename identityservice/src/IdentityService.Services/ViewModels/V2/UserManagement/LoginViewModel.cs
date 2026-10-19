namespace IdentityService.Services.ViewModels.V2.UserManagement
{
    public class LoginViewModel
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public string MobileNumber { get; set; }
        public int? Type { get; set; }
        public string Ip { get; set; }
        public string DeviceInfo { get; set; }
        public string UserToken { get; set; }
    }
}