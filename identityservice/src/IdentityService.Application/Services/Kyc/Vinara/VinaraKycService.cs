using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Common.Interfaces;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagementKycLog;
using IdentityService.Domain.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.InternalThreadParty.OmidTechAuthentication.Models;
using System.Diagnostics.Contracts;
using System.Text.Json;

namespace IdentityService.Application.Services.Kyc.Vinara;

public class VinaraKycService(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IUserManagementKycTransactionRepository userManagementKycTransactionRepository,
    VinaraKycTransactionService kycTransactionService,
    RequestHelperV2 requestHelper,
    IConfiguration configuration,
    ILogger<VinaraKycService> logger) : IKycServiceProvider
{
    private readonly string _baseUrl = configuration["Vinara:BaseUrl"]!;
    public EKycProvider Provider { get; set; } = EKycProvider.Vinara;

    public async Task<BaseMessageResponse<InquiryResponse>> InquiryAsync(string token, KycResult? umStatus, CancellationToken cancellationToken)
    {
        InquiryResponse data = null!;
        var kycTransaction = await userManagementKycTransactionRepository.GetByKycTokenAsync(token, cancellationToken);
        logger.LogInformation("kycTransaction:" + JsonSerializer.Serialize(kycTransaction));
        if (kycTransaction is not null)
        {
            data = new InquiryResponse { UmStatus = KycStep.Approved, KycType = kycTransaction.KycType, IsWagePaid = kycTransaction.IsWagePaid };
            return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, data)!;
        }

        var umKycLog = await userManagementKycLogRepository.GetByKycTokenAsync(token, cancellationToken);
        if (umKycLog is null)
            return MessageResponse.GenerateMessageResponse<InquiryResponse>(EResponseStatusCode.ItemIsNotFound)!;

        var status = await InquiryVinaraStatus(umKycLog.Token, umKycLog.KycStep);
        logger.LogInformation("vinara status: " + status);
        if (status is not null)
        {
            data = new InquiryResponse { UmStatus = status.Value, KycType = umKycLog.KycType, IsWagePaid = false };
            await kycTransactionService.Handle(umKycLog, status.Value, cancellationToken);
        }

        logger.LogInformation("data: " + JsonSerializer.Serialize(data));
        return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, data)!;
    }

    public Task<LinkInfo?> SubmitAuthentication(RegisterUMRequest request, CancellationToken cancellationToken)
    {
        if (request?.RequestInfo == null)
            throw new ArgumentNullException(nameof(request));

        var token = Guid.NewGuid().ToString();
        var requestInfo = request.RequestInfo;
        var url = GenerateUrlSubmitAuthentication(requestInfo, token);
        return  Task.FromResult(new LinkInfo(url, token))!;
        //request.RequestInfo.
        //var token = Guid.NewGuid().ToString();
        //var url = $"{_baseUrl}?token={token}";
        //return Task.FromResult(new LinkInfo(url, token))!;
    }



    private string UrlEncode(string url)
    {
        return string.IsNullOrEmpty(url) ? string.Empty : Uri.EscapeDataString(url);
    }

    private string GenerateUrlSubmitAuthentication(RequestInfo request, string token)
    {

        var contractPublicId = configuration["Authentication:ContractPublicId"]
                              ?? throw new InvalidOperationException("ContractPublicId not configured");

        string callBackUrl = GetCallbackUrl(request.AppId, token);

        var finalUrl = $"{_baseUrl}?token={token}&contract={contractPublicId}&callback_url={UrlEncode(callBackUrl)}";

        return finalUrl;
    }



    private string GetCallbackUrl(Enums.AppEnum appId, string token)
    {
        return appId switch
        {
            Enums.AppEnum.PWA => $"{configuration["UMPWACallBackUrl"]}?4" ?? string.Empty,
            Enums.AppEnum.OWP => $"{configuration["UMOWPCallBackUrl"]}?4" ?? string.Empty,
            Enums.AppEnum.MobileApp => $"{configuration["UMMobileCallBackUrl"]}?4" ?? string.Empty,
            Enums.AppEnum.IOS => $"{configuration["UMMobileCallBackUrl"]}?4" ?? string.Empty,
            _ => $"{configuration["UMMobileCallBackUrl"]}?4" ?? string.Empty
        };
    }


    public BaseMessageResponse<TData> GetProviderErrorMessage<TData>()
    {
        var response = MessageResponse.GenerateMessageResponse<TData>(EResponseStatusCode.CallErrorOmidTech)!;
        response.Message = "خطا در ارتباط با سرور وینارا";
        return response!;
    }

    public LinkInfo GenerateRedirectUrl(string url, string? token)
    {
        return new LinkInfo(url, token!);
    }

    public KycStep GetKycStep(InquiryResponse inquiryResponse)
    {
        return inquiryResponse is { UmStatus: KycStep.Approved, IsWagePaid: false } ? KycStep.WagePayment : inquiryResponse.UmStatus;
    }

    private async Task<KycStep?> InquiryVinaraStatus(string token, KycStep currentStatus)
    {
        var url = $"{_baseUrl}/api/v2/request/status?token={token}";
        var headers = new Dictionary<string, string>
        {
            ["Authorization"] = $"Basic {configuration["Vinara:ApiKey"]}"
        };
        logger.LogInformation("url: " + url + "\r\nHeaders: " + JsonSerializer.Serialize(headers));
        var responseStr = await requestHelper.CallServiceAsync(url, null, headers, HttpMethod.Get);
        logger.LogInformation("vinara response: " + responseStr);
        if (responseStr is null)
            return null;
        var response = JsonSerializer.Deserialize<VinaraInquiryStatusResponse>(responseStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (response is null)
            return null;
        if (response.Error?.Code == 1106)
            return currentStatus;
        logger.LogInformation("vinara response 2: " + JsonSerializer.Serialize(response));
        return kycTransactionService.ProcessUmStatus(response.Code);
    }
}