using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Factories;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagementKycLog;
using IdentityService.Domain.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.Cache;
using OmidBank.Common.ExternalThirdParty.Tosan.Global.Request;
using OmidBank.Common.ExternalThirdParty.Tosan.Global;
using OmidBank.Common.ExternalThirdParty.Tosan;
using OmidBank.Common.InternalThreadParty.BankService;
using OmidBank.Common.ExternalThirdParty.Tosan.Deposit.Request;
using OmidBank.Common.ExternalThirdParty.Tosan.Deposit;
using OmidBank.Common.InternalThirdParty.BankService.Request;

namespace IdentityService.Application.Handler.UserManagement;

public class CheckStatusUmHandler(
    IUserManagementRepository userManagementRepository,
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IUserManagementKycTransactionRepository userManagementKycTransactionRepository,
    IConfiguration configuration,
    TosanGlobalProvider tosanGlobalProvider,
    TosanDepositProvider tosanDepositProvider,
    IRedisCacheManager redisCacheManager,
    IKycServiceFactory kycServiceFactory,
    ILogger<CheckStatusUmHandler> logger) : IBaseHandler<CheckStatusUmRequest, BaseMessageResponse<CheckStatusUMResponse>>
{
    private readonly bool _enableCreateCustomer = bool.Parse(configuration["EnableCreateCustomer"]!);
    public async Task<BaseMessageResponse<CheckStatusUMResponse>> Handle(CheckStatusUmRequest request, CancellationToken cancellationToken)
    {
        request.KycProvider = (EKycProvider)Convert.ToInt32(configuration["KycProvider"]!);
        var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, new CheckStatusUMResponse());
        var bankProvider = new BankServiceProvider(configuration["BankServiceUrl"], configuration["ApiKeyInternal"]);
        var umUser = await userManagementRepository.GetByUserIdAsync(request.CurrentUser!.Id, cancellationToken);
        var checkBankCustomerRequest = false;
        
        if (umUser is not null && umUser.TosanRegister == true)
        {
            response.Data!.IsUmUser = 3;
            response.Data.RedirectToUrl = null!;
            return response!;
        }

        var sessionIdThirdParty = redisCacheManager.Read<object>(string.Format(CacheKey.TosanSystemSessionId));
        if (sessionIdThirdParty is null)
            return MessageResponse.GenerateMessageResponse<CheckStatusUMResponse>(EResponseStatusCode.SystemTokenNotFound)!;

        var getCifResponse = await tosanGlobalProvider.GetCustomerDetailInfoAsync(new GetCustomerDetailInfoRequest(sessionIdThirdParty.ToString())
        {
            UseBaseUrlSystem = true,
            Code = request.CurrentUser.NationalCode
        }, cancellationToken);

        if (getCifResponse.Exception is not null && getCifResponse.Exception.Code != TosanMessage.InvalidCustomerWSException)
            return MessageResponse.GenerateMessageResponse<CheckStatusUMResponse>(EResponseStatusCode.CallErrorTosan)!;

        if (umUser is not null)
        {
            response.Data!.IsUmUser = 2;
            response.Data.RedirectToUrl = null!;

            var cifExceptionStatus = getCifResponse.Exception is not null && getCifResponse.Exception.Code == TosanMessage.InvalidCustomerWSException;
            
            var createBankCustomerRequest = await bankProvider.BankCustomerInfo(new BankCustomerInfoRequest
            {
                UserId = request.CurrentUser.Id
            }, cancellationToken);

            if (createBankCustomerRequest.Data?.SuccessFulDateTime is not null)
                checkBankCustomerRequest = true;

            switch (cifExceptionStatus)
            {
                case true when checkBankCustomerRequest:
                    response.Data.UMStatus = _enableCreateCustomer ? KycStep.CustomerPending : KycStep.IsNotBankCustomer;
                    response.Data.IsUmUser = 1;
                    return response!;
                case true:
                    response.Data.UMStatus = _enableCreateCustomer ? KycStep.IsNotBankCustomerEnableCreateCustomer : KycStep.IsNotBankCustomer;
                    response.Data.IsUmUser = 1;
                    return response!;
                case false:
                {
                    var customerDetailInfo = await tosanGlobalProvider.GetCustomerBriefDetailAsync(
                        new GetCustomerBriefDetailRequest(sessionIdThirdParty.ToString()) { Ssn = request.CurrentUser.NationalCode }, cancellationToken);

                    var customerInfo = customerDetailInfo.Result.CustomerBriefDetailInfoBeans.FirstOrDefault();
                    if (customerInfo is null) 
                        return response!;
                    
                    var responseDepositList = await tosanDepositProvider.GetDepositsAsync(new GetDepositsRequest(sessionIdThirdParty.ToString())
                    {
                        Cif = customerInfo.Cif,
                        Length = 1
                    }, cancellationToken);
                    
                    if (responseDepositList.Result.TotalRecord > 0)
                        return response!;  // is umUser 2

                    if (checkBankCustomerRequest)
                    {
                        response.Data.UMStatus = _enableCreateCustomer ? KycStep.CustomerPending : KycStep.IsNotBankCustomer;
                        response.Data.IsUmUser = 1;
                        return response!;
                    }

                    response.Data.UMStatus = _enableCreateCustomer ? KycStep.IsNotBankCustomerEnableCreateCustomer : KycStep.IsNotBankCustomer;
                    response.Data.IsUmUser = 1;
                    return response!;
                }
            }
        }

        var kycLog = await userManagementKycLogRepository.GetByUserIdUmRegisterAsync(request.CurrentUser.Id, cancellationToken);
        if (kycLog is not null)
            request.KycProvider = (EKycProvider)kycLog.KycProvider;
        response.Data!.IsUmUser = 1;
        if (getCifResponse.Exception is not null && getCifResponse.Exception.Code == TosanMessage.InvalidCustomerWSException)
        {
            response.Data.UMStatus = _enableCreateCustomer ? kycLog?.KycStep ?? KycStep.NotExistUser : KycStep.IsNotBankCustomer;
            if (kycLog is null)
                return response!;
        }
        var kycTransactionLog = await userManagementKycTransactionRepository.GetByKycUserIdAsync(request.CurrentUser.Id, cancellationToken);
        if (kycTransactionLog is not null)
            request.KycProvider = (EKycProvider)kycTransactionLog.ProviderType;
        string token;
        var inquiryService = kycServiceFactory.Create(request.KycProvider);
        if (kycTransactionLog is null)
        {
            if (kycLog is null)
                return response!;
            token = kycLog.Token;
            if (!string.IsNullOrWhiteSpace(kycLog.FarashenasaLink) && !string.IsNullOrWhiteSpace(kycLog.Token))
                response.Data.RedirectToUrl = inquiryService.GenerateRedirectUrl(kycLog.FarashenasaLink, kycLog.Token).FarashenasaLink;
        }
        else
            token = kycTransactionLog.Token;
        var result = await inquiryService.InquiryAsync(token, null, cancellationToken);
        if (result.Data is null)
            return inquiryService.GetProviderErrorMessage<CheckStatusUMResponse>();

        response.Data.UMStatus = inquiryService.GetKycStep(result.Data);
        if (response.Data.UMStatus is KycStep.Pending or KycStep.Approved or KycStep.Rejected)
            response.Data.RedirectToUrl = null!;
        return response!;
    }
}

public record CheckStatusUmRequest : BaseCommand<BaseMessageResponse<CheckStatusUMResponse>>
{
    public EKycProvider KycProvider { get; set; }
}

public class CheckStatusUMResponse
{
    public int IsUmUser { get; set; } //1 has not UM  2 has UM & has not Tosan    3 has UM & has Tosan
    public KycStep UMStatus { get; set; }
    public string RedirectToUrl { get; set; }
}