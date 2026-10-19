//using IdentityService.Domain.Entity.UserManagementKycLog;
//using IdentityService.Domain.Entity;
//using Microsoft.Extensions.Configuration;
//using OmidBank.Common.InternalThreadParty.OmidTechAuthentication;
//using OmidBank.Common.Extentions;
//using IdentityService.Application.Handler.UserManagementKycLog;
//using static OmidBank.Common.Enums;
//using OmidBank.Common.InternalThreadParty.OmidTechAuthentication.Models;
//using IdentityService.Domain.Entity.Person;

//namespace IdentityService.Application.Handler.UserManagement;

//public class ForgetPasswordHandler : IBaseHandler<ForgetPasswordUMRequest, BaseMessageResponse<ForgetPasswordUMResponse>>
//{
//    private readonly IUserManagementRepository _userManagementRepository;
//    private readonly IUserManagementKycLogRepository _userManagementKycLogRepository;
//    private readonly IConfiguration _configuration;
//    private readonly IPersonRepository _personRepository;

//    public ForgetPasswordHandler(IUserManagementRepository userManagementRepository, IUserManagementKycLogRepository userManagementKycLogRepository, IConfiguration configuration, IPersonRepository personRepository)
//    {
//        _userManagementRepository = userManagementRepository;
//        _configuration = configuration;
//        _userManagementKycLogRepository = userManagementKycLogRepository;
//        _personRepository = personRepository;
//    }

//    public async Task<BaseMessageResponse<ForgetPasswordUMResponse>> Handle(ForgetPasswordUMRequest request, CancellationToken cancellationToken)
//    {
//        var response = MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.DoneSuccessfully, new());

//        if (!CommonHelper.IsValidPassword(request.NewPassword))
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.NotValidPassword);

//        var userManagement = await _userManagementRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);
//        if (userManagement is null)
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.ItemIsNotFound);

//        var kycLog = await _userManagementKycLogRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);
//        if (kycLog != null)
//        {
//            if (kycLog.KycStep == KycStep.GenerateLink && kycLog.KycType == KycType.ForgetPasswordUM)
//            {
//                if (kycLog.TokenExpierTime > DateTime.Now)
//                {
//                    response.Data.RedirectToUrl = kycLog.FarashenasaLink;
//                    return response;
//                }
//            }
//        }

//        var person = await _personRepository.GetByNationalCodeAsync(request.CurrentUser.NationalCode, cancellationToken);
//        OmidTechAuthenticationService omidTechService = new OmidTechAuthenticationService(_configuration["BaseOmidTechUrl"], _configuration["ApiKeyOmidTech"]);
//        var omidResponse = await omidTechService.SubmitAuthentication(new AuthenticationSubmitInformation()
//        {
//            NationalCode = request.CurrentUser.NationalCode,
//            Phone = person.MobileNumber,
//            BirthDate = (DateTime)person.BirthDate,
//            IdCardSerialNumber = person.NationalCardSeries,
//            UniqueKey = null,
//            AuthorizationType = "OFFLINE"
//        }, cancellationToken, true);

//        if (omidResponse == null)
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.CallErrorOmidTech);
//        if (omidResponse.ErrorCode != null)
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.CallErrorOmidTech);
//        if (omidResponse.RedirectUrl.IsNullOrEmpty())
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.CallErrorOmidTech);

//        var linkInfo = GetLinkInfoDetail(omidResponse.RedirectUrl, omidResponse.UniqueKey);
//        await HandleForgetPassword(userManagement, person, request.NewPassword, linkInfo, request.RequestInfo.AppId, cancellationToken);

//        response.Data.RedirectToUrl = linkInfo.FarashenasaLink;

//        return response;
//    }

//    private async Task HandleForgetPassword(UserManagmentEntity um, PersonEntity person, string password, LinkInfo linkInfo, AppEnum appId, CancellationToken cancellationToken)
//    {
//        var kycLog = await _userManagementKycLogRepository.GetByUserIdAsync(um.UserId, cancellationToken);
//        if (kycLog is null)
//        {
//            await _userManagementKycLogRepository.AddAsync(new UserManagementKycLogEntity()
//            {
//                UserName = um.UserName,
//                Password = ***;
//        }
//        else
//        {
//            kycLog.Token = linkInfo.Token;
//            kycLog.TokenExpierTime = DateTime.Now.AddMinutes(20);
//            kycLog.FarashenasaLink = linkInfo.FarashenasaLink;
//            kycLog.AppId = appId;

//            await _userManagementKycLogRepository.UpdateAsync(kycLog, cancellationToken);
//        }

//        await _userManagementKycLogRepository.SaveChangeAsync(cancellationToken);
//    }

//    private LinkInfo GetLinkInfoDetail(string url, string? token)
//    {
//        if (token is null)
//        {
//            int keyIndex = url.LastIndexOf("key=");
//            var keyToken = url.Substring(keyIndex);
//            token = keyToken.Replace("key=", "");
//        }

//        string[] urls = url.Split("&");

//        var callBackToServerUrl = $"{_configuration["ApplicationUrl"]}/v2/Authentication/UpdateUM?Key={token}";
//        return new LinkInfo($"{urls.FirstOrDefault()}&returnUrl={callBackToServerUrl}", token);
//    }
//}

//public record ForgetPasswordUMRequest([Required] string NewPassword) : BaseCommand<BaseMessageResponse<ForgetPasswordUMResponse>>;

//public class ForgetPasswordUMResponse
//{
//    public string RedirectToUrl { get; set; }
//}//using IdentityService.Domain.Entity.UserManagementKycLog;
//using IdentityService.Domain.Entity;
//using Microsoft.Extensions.Configuration;
//using OmidBank.Common.InternalThreadParty.OmidTechAuthentication;
//using OmidBank.Common.Extentions;
//using IdentityService.Application.Handler.UserManagementKycLog;
//using static OmidBank.Common.Enums;
//using OmidBank.Common.InternalThreadParty.OmidTechAuthentication.Models;
//using IdentityService.Domain.Entity.Person;

//namespace IdentityService.Application.Handler.UserManagement;

//public class ForgetPasswordHandler : IBaseHandler<ForgetPasswordUMRequest, BaseMessageResponse<ForgetPasswordUMResponse>>
//{
//    private readonly IUserManagementRepository _userManagementRepository;
//    private readonly IUserManagementKycLogRepository _userManagementKycLogRepository;
//    private readonly IConfiguration _configuration;
//    private readonly IPersonRepository _personRepository;

//    public ForgetPasswordHandler(IUserManagementRepository userManagementRepository, IUserManagementKycLogRepository userManagementKycLogRepository, IConfiguration configuration, IPersonRepository personRepository)
//    {
//        _userManagementRepository = userManagementRepository;
//        _configuration = configuration;
//        _userManagementKycLogRepository = userManagementKycLogRepository;
//        _personRepository = personRepository;
//    }

//    public async Task<BaseMessageResponse<ForgetPasswordUMResponse>> Handle(ForgetPasswordUMRequest request, CancellationToken cancellationToken)
//    {
//        var response = MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.DoneSuccessfully, new());

//        if (!CommonHelper.IsValidPassword(request.NewPassword))
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.NotValidPassword);

//        var userManagement = await _userManagementRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);
//        if (userManagement is null)
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.ItemIsNotFound);

//        var kycLog = await _userManagementKycLogRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);
//        if (kycLog != null)
//        {
//            if (kycLog.KycStep == KycStep.GenerateLink && kycLog.KycType == KycType.ForgetPasswordUM)
//            {
//                if (kycLog.TokenExpierTime > DateTime.Now)
//                {
//                    response.Data.RedirectToUrl = kycLog.FarashenasaLink;
//                    return response;
//                }
//            }
//        }

//        var person = await _personRepository.GetByNationalCodeAsync(request.CurrentUser.NationalCode, cancellationToken);
//        OmidTechAuthenticationService omidTechService = new OmidTechAuthenticationService(_configuration["BaseOmidTechUrl"], _configuration["ApiKeyOmidTech"]);
//        var omidResponse = await omidTechService.SubmitAuthentication(new AuthenticationSubmitInformation()
//        {
//            NationalCode = request.CurrentUser.NationalCode,
//            Phone = person.MobileNumber,
//            BirthDate = (DateTime)person.BirthDate,
//            IdCardSerialNumber = person.NationalCardSeries,
//            UniqueKey = null,
//            AuthorizationType = "OFFLINE"
//        }, cancellationToken, true);

//        if (omidResponse == null)
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.CallErrorOmidTech);
//        if (omidResponse.ErrorCode != null)
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.CallErrorOmidTech);
//        if (omidResponse.RedirectUrl.IsNullOrEmpty())
//            return MessageResponse.GenerateMessageResponse<ForgetPasswordUMResponse>(EResponseStatusCode.CallErrorOmidTech);

//        var linkInfo = GetLinkInfoDetail(omidResponse.RedirectUrl, omidResponse.UniqueKey);
//        await HandleForgetPassword(userManagement, person, request.NewPassword, linkInfo, request.RequestInfo.AppId, cancellationToken);

//        response.Data.RedirectToUrl = linkInfo.FarashenasaLink;

//        return response;
//    }

//    private async Task HandleForgetPassword(UserManagmentEntity um, PersonEntity person, string password, LinkInfo linkInfo, AppEnum appId, CancellationToken cancellationToken)
//    {
//        var kycLog = await _userManagementKycLogRepository.GetByUserIdAsync(um.UserId, cancellationToken);
//        if (kycLog is null)
//        {
//            await _userManagementKycLogRepository.AddAsync(new UserManagementKycLogEntity()
//            {
//                UserName = um.UserName,
//                Password = ***;
//        }
//        else
//        {
//            kycLog.Token = linkInfo.Token;
//            kycLog.TokenExpierTime = DateTime.Now.AddMinutes(20);
//            kycLog.FarashenasaLink = linkInfo.FarashenasaLink;
//            kycLog.AppId = appId;

//            await _userManagementKycLogRepository.UpdateAsync(kycLog, cancellationToken);
//        }

//        await _userManagementKycLogRepository.SaveChangeAsync(cancellationToken);
//    }

//    private LinkInfo GetLinkInfoDetail(string url, string? token)
//    {
//        if (token is null)
//        {
//            int keyIndex = url.LastIndexOf("key=");
//            var keyToken = url.Substring(keyIndex);
//            token = keyToken.Replace("key=", "");
//        }

//        string[] urls = url.Split("&");

//        var callBackToServerUrl = $"{_configuration["ApplicationUrl"]}/v2/Authentication/UpdateUM?Key={token}";
//        return new LinkInfo($"{urls.FirstOrDefault()}&returnUrl={callBackToServerUrl}", token);
//    }
//}

//public record ForgetPasswordUMRequest([Required] string NewPassword) : BaseCommand<BaseMessageResponse<ForgetPasswordUMResponse>>;

//public class ForgetPasswordUMResponse
//{
//    public string RedirectToUrl { get; set; }
//}