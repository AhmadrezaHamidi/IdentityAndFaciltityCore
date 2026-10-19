using IdentityService.Services.ViewModels.AradPushNotification;

namespace IdentityService.Services.ViewModels
{
    public class VerifyActivationCodeResponseViewModel : BaseViewModel
    {
        public int PersonId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? CityId { get; set; }
        public string CityTitle { get; set; }
        public string CityEnTitle { get; set; }
        public bool IsActive { get; set; }
        public string AvatarName { get; set; }
        public string MobileNumber { get; set; }
        public string Token { get; set; }
        public string InvitedCode { get; set; }
        public string NationalCode { get; set; }
        public string CdnUrl { get; set; }
        public bool IsActiveProfile { get; set; }
        public ResSendNotificationDataViewModel?  NotificationDataViewModel { get; set; }

    }
}
