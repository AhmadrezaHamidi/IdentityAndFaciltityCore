using Elasticsearch.Net;
using IdentityService.Domain.Entity;
using IdentityService.Domain.IRepositories;
using IdentityService.Services.IServices;
using IdentityService.Services.Services;
using IdentityService.Services.ViewModels;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.Cache;
using OmidBank.Common.ExternalThirdParty.Tosan.Global;
using OmidBank.Common.ExternalThirdParty.Tosan.Signup;
using System.Collections.Generic;
using OmidBank.Common.ExternalThirdParty.Tosan.Global.Request;
using static OmidBank.Common.Enums;

namespace IdentityService.Application.Handler.UserManagement
{
    public class SendOtpUMHandler : IBaseHandler<SendOtpUMRequest, BaseMessageResponse<SendOtpUMResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly ISmsService _smsService;
        private readonly string _messageServiceUrl, _apiKeyInternal;
        private readonly IConfiguration _configuration;
        private readonly TosanGlobalProvider _tosanGlobalProvider;

        public SendOtpUMHandler(IRedisCacheManager redisCacheManager, IUserRepository userRepository, IConfiguration configuration, ISmsService smsService, TosanGlobalProvider tosanGlobalProvider)
        {
            _configuration = configuration;
            _redisCacheManager = redisCacheManager;
            _userRepository = userRepository;
            _smsService = smsService;
            _tosanGlobalProvider = tosanGlobalProvider;
            _apiKeyInternal = _configuration["ApiKeyInternal"];
            _messageServiceUrl = _configuration["MessageServiceUrl"];
        }

        public async Task<BaseMessageResponse<SendOtpUMResponse>> Handle(SendOtpUMRequest request, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<SendOtpUMResponse>(EResponseStatusCode.DoneSuccessfully, null);
            var code = CommonHelper.GenerateOtpCode();

            var findUser = await _userRepository.FindById(request.CurrentUser.Id, cancellationToken);
            if (findUser is null)
                return MessageResponse.GenerateMessageResponse<SendOtpUMResponse>(EResponseStatusCode.ItemIsNotFound);

            //if (findUser.BankCustomerNumber is null)
            //{
                var checkUpdate = await UpdateCifAsync(findUser.Id, findUser.Person.NationalCode, cancellationToken);
                if (!checkUpdate)
                    return MessageResponse.GenerateMessageResponse<SendOtpUMResponse>(EResponseStatusCode.UpdateCifPending);
            //}

            var sendSmsRequest = new SendActivitionCodeOmidTechViewModel()
            {
                MobileNumber = findUser.Person.MobileNumber,
                Scheduler = null,
                Code = code,
                Url = _messageServiceUrl,
                ApiKeyInternal = _apiKeyInternal
            };
            _redisCacheManager.Write(string.Format(CacheKey.OtpRegisterUM, request.RequestInfo.Token), code, Enums.ECacheTime.TwoMinutes);
            await _smsService.SendActivationCodeFromThirdPartyOmidTech(sendSmsRequest, cancellationToken);
            return response;
        }

        private async Task<bool> UpdateCifAsync(int userId, string nationalCode, CancellationToken cancellationToken)
        {
            var sessionIdThirdparty = _redisCacheManager.Read<object>(string.Format(CacheKey.TosanSystemSessionId));
            if (sessionIdThirdparty is null)
                return false;

            var response = await _tosanGlobalProvider.GetCustomerBriefDetailAsync(new GetCustomerBriefDetailRequest(sessionIdThirdparty.ToString())
            {
                Ssn = nationalCode
            }, cancellationToken);

            if (response.Exception is not null)
                return false;

            if (response.Result.CustomerBriefDetailInfoBeans != null && response.Result.CustomerBriefDetailInfoBeans.Any())
            {
                var cif = response.Result.CustomerBriefDetailInfoBeans.FirstOrDefault().Cif;
                if (!string.IsNullOrWhiteSpace(cif))
                {
                    await _userRepository.UpdateUserCustomerNumber(userId, Convert.ToInt64(cif), cancellationToken);
                    return true;
                }
            }

            return false;
        }
    }
}

public record SendOtpUMRequest([Required] string MobileNumber) : BaseCommand<BaseMessageResponse<SendOtpUMResponse>>;

public class SendOtpUMResponse
{
}