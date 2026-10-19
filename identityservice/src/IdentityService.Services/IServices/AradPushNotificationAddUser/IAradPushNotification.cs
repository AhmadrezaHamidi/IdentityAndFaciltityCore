using IdentityService.Services.ViewModels.AradPushNotification;
using OmidBank.Common.InternalThreadParty.Arad.Response;

namespace IdentityService.Services.IServices.AradPushNotificationAddUser
{
    public interface IAradPushNotification : IBaseService
    {
        Task<GetUserListResponse> GetUserLis(double? timeout);
        Task<AddUserResponse> AddUser(AradAddUserViewModel model, double? timeout);
        Task<RemoveUserDevicesResonse> RemoveUser(string mobileNumber, string fcmToken, double? timeout);
        Task<GetUserDeviceListResponse> GetUserDeviceList(string mobile, double? timeout);
    }
}