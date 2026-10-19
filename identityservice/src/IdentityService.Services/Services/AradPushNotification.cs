using IdentityService.Services.IServices.AradPushNotificationAddUser;
using IdentityService.Services.ViewModels.AradPushNotification;
using OmidBank.Common.InternalThreadParty.Arad;
using OmidBank.Common.InternalThreadParty.Arad.Request;
using OmidBank.Common.InternalThreadParty.Arad.Response;

namespace IdentityService.Services.Services
{
    public class AradPushNotification : BaseService, IAradPushNotification
    {
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<AradPushNotification> _logger;
        private readonly IConfiguration _configuration;

        private readonly string _baseUrl;
        private readonly string _aradPushNotificationXAPIKEY;
        private readonly string _portPushNotification;
        private readonly AradProvider _aradProvider;

        public AradPushNotification(IRequestHelper requestHelper, ILogger<AradPushNotification> logger, IConfiguration configuration)
        {
            _requestHelper = requestHelper;
            _logger = logger;
            _configuration = configuration;
            _baseUrl = _configuration["BaseUrlAradPushNotification"];
            _aradPushNotificationXAPIKEY = configuration["AradPushNotificationXAPIKEY"];
            _portPushNotification = configuration["PushNotificationPort"];
            _aradProvider = new AradProvider(_baseUrl, _aradPushNotificationXAPIKEY, "1");
        }

        public async Task<AddUserResponse> AddUser(AradAddUserViewModel model, double? timeout)
        {
            try
            {
                var resAradAddUser = await _aradProvider.AddUser(new AddUserRequest
                {
                    ApplicationVersion = model.ApplicationVersion,

                    FcmToken = model.FcmToken,
                    Mobile = model.Mobile,
                    Type = model.Type,
                    OsName = model.OsName,
                    BrowserName = model.BrowserName,
                    DeviceName = model.DeviceName,
                    DeviceVersion = model.DeviceVersion
                }, timeout);

                return resAradAddUser;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }

        public async Task<GetUserListResponse> GetUserLis(double? timeout)
        {
            try
            {
                var resRemoveUserDevices = await _aradProvider.GetUserList(new GetUserListRequest() { PageNo = 1, PageSize = 1000 }, timeout, new CancellationToken());
                return resRemoveUserDevices;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }

        public async Task<GetUserDeviceListResponse> GetUserDeviceList(string mobile, double? timeout)
        {
            try
            {
                var resGetUserDeviceList = await _aradProvider.GetUserDeviceList(new GetUserDeviceListRequest() { Mobile = mobile }, timeout, new CancellationToken());
                return resGetUserDeviceList;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }

        public async Task<RemoveUserDevicesResonse> RemoveUser(string mobileNumber, string fcmToken, double? timeout)
        {
            try
            {
                var resRemoveUserDevices = await _aradProvider.RemoveUserDevices(new RemoveUserDevicesRequest() { Mobile = mobileNumber, FcmToken = fcmToken }, timeout, new CancellationToken());
                return resRemoveUserDevices;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }
    }
}