//using IdentityService.Domain.Entity;
//using IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;
//using IdentityService.Domain.Entity.UserManagement.UserManagmentBiometric;
//using IdentityService.Domain.IRepositories;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using OmidBank.Common.Cryptography;
//using OmidBank.Common.Extentions;
//using OmidBank.Common.InternalThreadParty.OmidTech;
//using OmidBank.Common.InternalThreadParty.OmidTech.Request;
//using OmidBank.Common.JwtHelper;
//using static OmidBank.Common.Enums;

//namespace IdentityService.Application.Handler.UserManagement;

//public class BiometricLoginHandler : IBaseHandler<BiometricLoginRequest, BaseMessageResponse<BiometricLoginResponse>>
//{
//    private readonly IUserManagementRepository _userManagementRepository;
//    private readonly IUserRepository _userRepository;
//    private readonly IConfiguration _configuration;
//    private readonly IUserManagementTokenLogRepository _userManagementTokenLogRepository;
//    private readonly IUserManagementBiometricRepository _userManagmentBiometricRepository;
//    private readonly int _accessTokenExpierTime;
//    private readonly int _refreshTokenExpierTime;
//    private readonly ILogger<BiometricLoginHandler> _logger;
//    private readonly IJwtService _jwtService;
//    private readonly bool _isDevelopment;
//    private readonly IEncryption _encryptionManager;
//    private readonly string _tokenDevelopment;


//    public BiometricLoginHandler(IUserManagementRepository userManagementRepository, IUserRepository userRepository,
//        IConfiguration configuration, IUserManagementTokenLogRepository userManagementTokenLogRepository, ILogger<BiometricLoginHandler> logger,
//    IJwtService jwtService, IUserManagementBiometricRepository userManagmentBiometricRepository, IEncryption encryptionManager)
//    {
//        _userManagementRepository = userManagementRepository;
//        _userRepository = userRepository;
//        _configuration = configuration;
//        _userManagementTokenLogRepository = userManagementTokenLogRepository;
//        _accessTokenExpierTime = int.Parse(configuration["AccessTokenExpierTime"]);
//        _accessTokenExpierTime = int.Parse(configuration["RefreshTokenExpierTime"]);
//        _jwtService = jwtService;
//        _isDevelopment = bool.Parse(configuration["IsDevelopment"]);
//        _userManagmentBiometricRepository = userManagmentBiometricRepository;
//        _encryptionManager = encryptionManager;
//        _logger = logger;
//        _tokenDevelopment = configuration["TokenDevelopment"];

//    }

//    public async Task<BaseMessageResponse<BiometricLoginResponse>> Handle(BiometricLoginRequest request, CancellationToken cancellationToken)
//    {

//        var accessToken = "";
//        var refreshToken = "";
//        var response = MessageResponse.GenerateMessageResponse<BiometricLoginResponse>(EResponseStatusCode.DoneSuccessfully, new());
//        var omidTechProvider = new OmidTechProvider(_configuration["BaseOmidTechUrl"], _configuration["ApiKeyOmidTech"]);

//        var userManagementInfo = await _userManagementRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);

//        if (userManagementInfo is not null)

//        {
//            var biometricKey = await _userManagmentBiometricRepository.GetByUserManagmentIdAsync(userManagementInfo.Id, cancellationToken);
//            if (string.IsNullOrWhiteSpace(biometricKey.BiometricToken) || request.BiometricToken != biometricKey.BiometricToken)
//                return MessageResponse.GenerateMessageResponse<BiometricLoginResponse>(EResponseStatusCode.BadRequest);


//            var decryptBiomerikToken = _encryptionManager.DecryptAES(request.BiometricToken, biometricKey.BiometricKey, request.RequestInfo);
//            if (decryptBiomerikToken == null)
//                return MessageResponse.GenerateMessageResponse<BiometricLoginResponse>(EResponseStatusCode.BadRequest);

//            var biometrikTokenModel = decryptBiomerikToken.Split("$$");
//            if (request.RequestInfo.DeviceInfo != biometrikTokenModel[2])
//                return MessageResponse.GenerateMessageResponse<BiometricLoginResponse>(EResponseStatusCode.BadRequest);

//            var loginUserManagementRequest = new LoginUserManagement
//            {
//                UserName = biometrikTokenModel[0],
//                Password = ***;

//            var userManagementTokenLog = new UserManagementTokenLogEntity();
//            userManagementTokenLog.AccessToken = "";
//            userManagementTokenLog.RefreshToken = "";
//            userManagementTokenLog.ExpierDate = DateTime.Now.AddMinutes(_accessTokenExpierTime);
//            userManagementTokenLog.IsActive = true;
//            userManagementTokenLog.Type = (int)UserLoginType.LoginWithPhoneNumber;
//            userManagementTokenLog.Ip = request.RequestInfo.IP;
//            userManagementTokenLog.DeviceInfo = request.RequestInfo.IP;
//            userManagementTokenLog.UserId = request.CurrentUser.Id;

//            var mobile = new List<string>();
//            mobile.Add(request.RequestInfo.MobileNumber);
//            var password = ***;

//            var result = await _userManagementRepository.LoginUserModern(loginUserManagementRequest.UserName, password, loginUserManagementRequest.MobileNumber, cancellationToken);
//            if (result is null)
//            {
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
//                await _userManagementTokenLogRepository.SaveChangeAsync(cancellationToken);
//                return MessageResponse.GenerateMessageResponse<BiometricLoginResponse>(EResponseStatusCode.BadRequest);
//            }

//            if (!_isDevelopment)
//            {
//                accessToken = _jwtService.GenerateAsyncUserManagment(request.CurrentUser.Id, _accessTokenExpierTime, result.User.Person.NationalCode,
//           result.User.Person.MobileNumber);
//                refreshToken = _jwtService.GenerateAsyncUserManagment(request.CurrentUser.Id, _refreshTokenExpierTime,
//                   result.User.Person.NationalCode, result.User.Person.MobileNumber);
//            }
//            else
//            {
//                accessToken = _tokenDevelopment;
//                refreshToken = _tokenDevelopment;
//            }
//            await _userManagementTokenLogRepository.UpdateTokenLogWithUserId(request.CurrentUser.Id, cancellationToken);
//            userManagementTokenLog.AccessToken = accessToken;
//            userManagementTokenLog.RefreshToken = refreshToken;
//            await _userManagementTokenLogRepository.AddAsync(userManagementTokenLog, cancellationToken);
//            await _userManagementTokenLogRepository.SaveChangeAsync(cancellationToken);
//            response.Data.AccessToken = accessToken;
//            response.Data.RefreshToken = refreshToken;
//            response.Data.StatusCode = 1;
//            response.Data.BiometricToken = request.BiometricToken;


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
//            if (request.CurrentUser.BankCustomerNumber == null)
//            {
//                var getCustomerInfoResponse = await omidTechProvider.SearchRealCustomerByNationalCode(new GetPersonByNationalCodeRequest
//                { NationalCode = request.CurrentUser.NationalCode, UMToken = accessToken }, cancellationToken);
//                if (getCustomerInfoResponse != null && getCustomerInfoResponse.IsSuccess)
//                {
//                    await _userRepository.UpdateUserCustomerNumber(request.CurrentUser.Id,
//                        long.Parse(getCustomerInfoResponse.ResultData.CustomerNumber), cancellationToken);
//                    response.Data.IsBankCustomer = true;
//                }
//                else
//                    response.Data.IsBankCustomer = false;
//            }
//            else
//                response.Data.IsBankCustomer = true;

//            return response;
//        }
//        return MessageResponse.GenerateMessageResponse<BiometricLoginResponse>(EResponseStatusCode.BadRequest);
//    }
//}
//public record BiometricLoginRequest([Required] string BiometricToken) : BaseCommand<BaseMessageResponse<BiometricLoginResponse>>;

//public class BiometricLoginResponse
//{
//    public string AccessToken { get; set; }
//    public string RefreshToken { get; set; }
//    public string BiometricToken { get; set; }
//    public int StatusCode { get; set; }
//    public bool IsBankCustomer { get; set; }
//}

