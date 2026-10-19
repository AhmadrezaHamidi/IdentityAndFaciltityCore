using OmidBank.Common.Models;

namespace IdentityService.Services.ViewModels.Response
{
    public class VerifyCodeLoginResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? CityId { get; set; }
        public string CityTitle { get; set; }
        public string CityEnTitle { get; set; }
        public int? ProvinceId { get; set; }
        public string ProvinceTitle { get; set; }
        public string ProvinceEnTitle { get; set; }
        public string AvatarUrl { get; set; }
        public string MobileNumber { get; set; }
        public string Token { get; set; }
        public string InvitedCode { get; set; }
        public string NationalCode { get; set; }
        public string MyInvitedCode { get; set; }
        public bool ChangePasswordWallet { get; set; }
        public long? OmidBankWalletAccountNumber { get; set; }
        public ResSendNotificationDataViewModel? NotificationDataViewModel { get; set; }
    }

    public class ResSendNotificationDataViewModel : BaseViewModel
    {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}