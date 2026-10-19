//using IdentityService.Application.Handler.UserManagement;
//using IdentityService.Domain.Entity;
//using IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using OmidBank.Common.Extentions;
//using OmidBank.Common.InternalThreadParty.OmidTech;
//using OmidBank.Common.InternalThreadParty.OmidTech.Request;
//using OmidBank.Common.JwtHelper;
//using System.Text.Json;
//using static OmidBank.Common.Enums;

//namespace IdentityService.Application.Handler.Shared
//{
//    internal class UserManagementHelper
//    {
//        private readonly IConfiguration _configuration;
//        private readonly IUserManagementTokenLogRepository _userManagementTokenLogRepository;
//        private readonly int _accessTokenExpierTime;
//        private readonly int _refreshTokenExpierTime;
//        private readonly ILogger<BiometricLoginHandler> _logger;
//        private readonly bool _isDevelopment;
//        private readonly string _tokenDevelopment;



//        public UserManagementHelper(IConfiguration configuration, IUserManagementTokenLogRepository userManagementTokenLogRepository,
//            ILogger<BiometricLoginHandler> logger, IJwtService jwtService)
//        {

//            _configuration = configuration;
//            _userManagementTokenLogRepository = userManagementTokenLogRepository;
//            _accessTokenExpierTime = int.Parse(configuration["AccessTokenExpierTime"]);
//            _accessTokenExpierTime = int.Parse(configuration["RefreshTokenExpierTime"]);
//            _isDevelopment = bool.Parse(configuration["IsDevelopment"]);
//            _logger = logger;
//            _tokenDevelopment = configuration["TokenDevelopment"];
//        }

//        public async Task<BaseMessageResponse<BiometricLoginResponse>> LoginUserManagement(OmidTechProvider omidTechProvider, LoginUserManagement request,
//            IUserManagementRepository userManagementRepository, IJwtService _jwtService,
//            CancellationToken cancellationToken)
//        {
//            _logger.LogError($"LoginUMRequest  " + JsonSerializer.Serialize(request));
//            var accessToken = "";
//            var refreshToken = "";
//            var response = MessageResponse.GenerateMessageResponse<BiometricLoginResponse>(EResponseStatusCode.DoneSuccessfully, new());
//            var userManagementTokenLog = new UserManagementTokenLogEntity();
//            userManagementTokenLog.AccessToken = "";
//            userManagementTokenLog.RefreshToken = "";
//            userManagementTokenLog.ExpierDate = DateTime.Now.AddMinutes(_accessTokenExpierTime);
//            userManagementTokenLog.IsActive = true;
//            userManagementTokenLog.Type = (int)UserLoginType.LoginWithPhoneNumber;
//            userManagementTokenLog.Ip = request.Ip;
//            userManagementTokenLog.DeviceInfo = request.DeviceInfo;
//            userManagementTokenLog.UserId = request.UserId;

//            var mobile = new List<string>();
//            mobile.Add(request.MobileNumber);
//            var password = ***;
//            _logger.LogError($"password" + JsonSerializer.Serialize(password));

//            var result = await userManagementRepository.LoginUserModern(request.UserName, password, request.MobileNumber, cancellationToken);
//            _logger.LogError($"Here 3" + JsonSerializer.Serialize(result));
//            if (result is null)
//            {
//                _logger.LogError($"Here 4");
//                if (!_isDevelopment)
//                {
//                    var sendSmsRequest = new SendMessageRequest
//                    {
//                        MobileNumbers = mobile,
//                        MessageBody = $"امید بانک سپه \n ورود ناموفق به بانکداری دیجیتال \n {TimeStampExtention.GetDatePersian(DateTime.Now)} {TimeStampExtention.GetTimePersian(DateTime.Now)}",
//                        SmsType = "omidBoom",
//                        Scheduler = null,
//                        TerminalType = 18
//                    };

//                    await omidTechProvider.SendSms(sendSmsRequest, cancellationToken);
//                }

//                await _userManagementTokenLogRepository.AddAsync(userManagementTokenLog, cancellationToken);
//                return MessageResponse.GenerateMessageResponse<BiometricLoginResponse>(EResponseStatusCode.BadRequest);
//            }

//            if (!_isDevelopment)
//            {
//                accessToken = _jwtService.GenerateAsyncUserManagment(request.UserId, _accessTokenExpierTime, result.User.Person.NationalCode,
//           result.User.Person.MobileNumber);
//                refreshToken = _jwtService.GenerateAsyncUserManagment(request.UserId, _refreshTokenExpierTime,
//                   result.User.Person.NationalCode, result.User.Person.MobileNumber);
//            }
//            else
//            {
//                accessToken = _tokenDevelopment;
//                refreshToken = _tokenDevelopment;
//            }

//            userManagementTokenLog.AccessToken = accessToken;
//            userManagementTokenLog.RefreshToken = refreshToken;
//            await _userManagementTokenLogRepository.AddAsync(userManagementTokenLog, cancellationToken);

//            response.Data.AccessToken = accessToken;
//            response.Data.RefreshToken = refreshToken;
//            if (!_isDevelopment)
//            {
//                var model = new SendMessageRequest
//                {
//                    MobileNumbers = mobile,
//                    MessageBody = $"امید بانک سپه \n ورود به بانکداری دیجیتال \n {TimeStampExtention.GetDatePersian(DateTime.Now)} {TimeStampExtention.GetTimePersian(DateTime.Now)}",
//                    SmsType = "omidBoom",
//                    Scheduler = null,
//                    TerminalType = 18
//                };
//                await omidTechProvider.SendSms(model, cancellationToken);
//            }
//            _logger.LogError($"here 5 :{JsonSerializer.Serialize(response)}");

//            return response;
//        }

//    }
//}
//public class LoginUserManagementResponse
//{
//    public string AccessToken { get; set; }
//    public string RefreshToken { get; set; }
//    public string BiometrikToken { get; set; }
//    public int StatusCode { get; set; }
//}

//public class LoginUserManagement
//{
//    public string UserName { get; set; }
//    public string Password { get; set; }
//    public int Type { get; set; }
//    public string DeviceInfo { get; set; }
//    public string Ip { get; set; }
//    public int UserId { get; set; }
//    public string MobileNumber { get; set; }
//}


