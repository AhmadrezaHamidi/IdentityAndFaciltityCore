using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Common.Interfaces;
using IdentityService.Application.Services.Kyc.Vinara;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.Person;
using IdentityService.Domain.Entity.UserManagementKycLog;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.Extentions;
using OmidBank.Common.InternalThreadParty.OmidTechAuthentication;
using OmidBank.Common.InternalThreadParty.OmidTechAuthentication.Models;
using OmidBank.Common.InternalThreadParty.OmidTechAuthentication.Request;
using OmidBank.Common.InternalThreadParty.OmidTechAuthentication.Response;

namespace IdentityService.Application.Services.Kyc.Farashenasa;

public class FarashenasaKycService(
    IConfiguration configuration,
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IPersonRepository personRepository,
    OmidTechAuthenticationService omidTechService,
    KycResultManagement kycResultManagement)
    : IKycServiceProvider
{
    public EKycProvider Provider { get; set; } = EKycProvider.Farashenasa;
    private readonly string _errorCodeNotFoundAuthInquiry = configuration["ErrorCodeNotFoundAuthInquiry"]!;

    public async Task<BaseMessageResponse<InquiryResponse>> InquiryAsync(string token, KycResult? umStatus, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, new InquiryResponse())!;
        var result = new InquiryResponse();

        var umKycLog = await userManagementKycLogRepository.GetByKycTokenAsync(token, cancellationToken);
        if (umKycLog is null)
            return MessageResponse.GenerateMessageResponse<InquiryResponse>(EResponseStatusCode.ItemIsNotFound)!;

        if (umStatus is null)
        {
            var omidResponse = await omidTechService.AuthenticationInquiry(new AuthenticationInquiryRequest { UniqueKey = token }, cancellationToken);
            if (omidResponse is null)
                return MessageResponse.GenerateMessageResponse<InquiryResponse>(EResponseStatusCode.CallErrorOmidTech)!;

            if (omidResponse.ErrorCode == _errorCodeNotFoundAuthInquiry)
            {

                await HandleNotFoundStatus(umKycLog, cancellationToken);
                response.Data!.UmStatus = KycStep.Rejected;
                response.Data.KycType = umKycLog.KycType;
                return response!;
            }

            response = SetUmStatus(omidResponse, result)!;
            if (!response.IsSuccess)
                return response!;
        }
        else
            result.UmStatus = umStatus == KycResult.APPROVED ? KycStep.Approved : KycStep.Rejected;

        await kycResultManagement.Handle(umKycLog, result.UmStatus, cancellationToken);
        response.Data = result;
        response.Data.KycType = umKycLog.KycType;
        return response!;
    }

    private static BaseMessageResponse<InquiryResponse> SetUmStatus(AuthenticationInquiryResponse omidResponse, InquiryResponse result)
    {
        if (omidResponse.QcStatus == null)
            return MessageResponse.GenerateMessageResponse<InquiryResponse>(EResponseStatusCode.CallErrorOmidTech)!;
        try
        {
            var kycStatus = omidResponse.QcStatus.ConverToEnum<KycStepOmidTech>();
            var aiStatus = omidResponse.QcStatus.ConverToEnum<KycStepOmidTech>();

            if (kycStatus == KycStepOmidTech.QC_PENDING || kycStatus == KycStepOmidTech.QC_IN_PROGRESS)
            {
                if (aiStatus == KycStepOmidTech.ACCEPT)
                    result.UmStatus = KycStep.Pending;
                else
                    result.UmStatus = KycStep.GenerateLink;
            }

            result.UmStatus = kycStatus switch
            {
                KycStepOmidTech.ACCEPT or KycStepOmidTech.QC_APPROVED => KycStep.Approved,
                KycStepOmidTech.QC_REJECT or KycStepOmidTech.PERSONAL_IMAGE_NOT_FOUND
                    or KycStepOmidTech.PERSONAL_INFO_NOT_FOUND or KycStepOmidTech.REJECT
                    or KycStepOmidTech.TIMEOUT => KycStep.Rejected,
                KycStepOmidTech.SUBMITTED => KycStep.GenerateLink,
                KycStepOmidTech.QC_PENDING or KycStepOmidTech.QC_IN_PROGRESS => KycStep.Pending,


                _ => result.UmStatus
            };
            return MessageResponse.GenerateMessageResponse<InquiryResponse>(EResponseStatusCode.DoneSuccessfully)!;
        }
        catch
        {
            return MessageResponse.GenerateMessageResponse<InquiryResponse>(EResponseStatusCode.CallErrorOmidTech)!;
        }
    }

    public async Task<LinkInfo?> SubmitAuthentication(RegisterUMRequest request, CancellationToken cancellationToken)
    {
        var authenticationRequest = await MakeSubmitInformation(request, cancellationToken);
        var omidResponse = await omidTechService.SubmitAuthentication(authenticationRequest, cancellationToken);
        if (omidResponse is not { ErrorCode: null } || omidResponse.RedirectUrl.IsNullOrEmpty())
            return null;
        var linkInfo = GetLinkInfoDetail(omidResponse.RedirectUrl, omidResponse.UniqueKey);
        return linkInfo;
    }

    private async Task<AuthenticationSubmitInformation> MakeSubmitInformation(RegisterUMRequest request, CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByNationalCodeAsync(request.CurrentUser!.NationalCode, cancellationToken);
        return new AuthenticationSubmitInformation
        {
            NationalCode = request.CurrentUser.NationalCode,
            Phone = person.MobileNumber,
            BirthDate = request.BirthDate.ToDateTime(), //requestVersion >= 2.1 ? request.BirthDate.ToDateTime() : request.BirthDate.ToDateTimeMilliseconds(),
            IdCardSerialNumber = request.NationalCardSeries.ToUpper(),
            UniqueKey = null,
            AuthorizationType = "OFFLINE"
        };
    }

    public BaseMessageResponse<TData> GetProviderErrorMessage<TData>()
    {
        return MessageResponse.GenerateMessageResponse<TData>(EResponseStatusCode.CallErrorOmidTech)!;
    }

    public LinkInfo GenerateRedirectUrl(string url, string? token)
    {
        if (token is null)
        {
            var keyIndex = url.LastIndexOf("key=", StringComparison.Ordinal);
            var keyToken = url.Substring(keyIndex);
            token = keyToken.Replace("key=", "");
        }

        var urls = url.Split("&");

        var callBackToServerUrl = $"{configuration["ApplicationUrl"]}/v2/Authentication/UpdateUM?Key={token}";
        return new LinkInfo($"{urls.FirstOrDefault()}&returnUrl={callBackToServerUrl}", token);
    }

    public KycStep GetKycStep(InquiryResponse inquiryResponse)
    {
        return inquiryResponse.UmStatus;
    }

    private LinkInfo GetLinkInfoDetail(string url, string? token)
    {
        if (token is null)
        {
            var keyIndex = url.LastIndexOf("key=", StringComparison.Ordinal) + 4;
            token = url[keyIndex..];
        }

        var urls = url.Split("&");
        var callBackToServerUrl = $"{configuration["ApplicationUrl"]}/v2/Authentication/UpdateUM?Key={token}";
        return new LinkInfo($"{urls.FirstOrDefault()}&returnUrl={callBackToServerUrl}", token);
    }

    private async Task HandleNotFoundStatus(UserManagementKycLogEntity umKycLog, CancellationToken cancellationToken)
    {
        await userManagementKycLogRepository.DeleteAsync(umKycLog, cancellationToken);
        await userManagementKycLogRepository.SaveChangeAsync(cancellationToken);
    }

}