using System.Text.RegularExpressions;
using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Factories;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagementKycLog;
using OmidBank.Common.Extentions;

namespace IdentityService.Application.Services.Kyc.Common;

public abstract partial class KycRegisterUmBaseService(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IUserManagementRepository userManagementRepository,
    IKycServiceFactory kycServiceFactory,
    EKycProvider provider)
{
    public EKycProvider Provider { get; set; } = provider;
    protected UserManagementKycLogEntity? KycLog;

    public async Task<BaseMessageResponse<RegisterUMResponse>> Register(RegisterUMRequest request, CancellationToken cancellationToken)
    {
        var response = await InitialCheck(request, cancellationToken);
        if (!response.IsSuccess || response.Data is not null)
            return response;

        MakeUserManagementKycLog(request);
        response = await InquiryRequiredInformation(request, cancellationToken);
        if (!response.IsSuccess)
            return response;
        var serviceProvider = kycServiceFactory.Create(request.KycProvider);
        var linkInfo = await serviceProvider.SubmitAuthentication(request, cancellationToken);
        if (linkInfo is null)
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.CallErrorOmidTech)!;
            // return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.VinaraCallError)!;
        SetKycLogLinkInfo(linkInfo);
        await AddKycLog(cancellationToken);
        response.Data!.RedirectToUrl = linkInfo.FarashenasaLink;
        return response;
    }

    private async Task<BaseMessageResponse<RegisterUMResponse>> InitialCheck(RegisterUMRequest request, CancellationToken cancellationToken)
    {
        if (userManagementRepository.CheckExistByUserId(request.CurrentUser!.Id))
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.YouHaYouAlreadyHaveTwoFactorAuthentication)!;

        if(!NationalCardRegex().IsMatch(request.NationalCardSeries))
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.NationalCardSeries)!;

        if (((DateTime.Now - request.BirthDate.ToDateTime()).TotalDays / 365) < 18)
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.AgeNotValid)!;

        var response = MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.DoneSuccessfully)!;
        KycLog = await userManagementKycLogRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);
        if (KycLog is not null)
            response.Data = new RegisterUMResponse { RedirectToUrl = KycLog.FarashenasaLink };
        return response!;
    }
    
    protected virtual Task<BaseMessageResponse<RegisterUMResponse>> InquiryRequiredInformation(RegisterUMRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, new RegisterUMResponse()))!;
    }

    private void MakeUserManagementKycLog(RegisterUMRequest request)
    {
        KycLog = new UserManagementKycLogEntity
        {
            NationalCardSeries = request.NationalCardSeries.ToUpper(),
            BirthDate = request.BirthDate.ToDateTime(), // requestVersion >= 2.1 ? request.BirthDate.ToDateTime() : request.BirthDate.ToDateTimeMilliseconds(),
            TokenExpierTime = DateTime.Now.AddMinutes(20),
            KycStep = KycStep.GenerateLink,
            KycType = KycType.RegisterUM,
            UserId = request.CurrentUser!.Id,
            AppId = request.RequestInfo.AppId,
            KycProvider = (int)request.KycProvider,
        };
    }

    private void SetKycLogLinkInfo(LinkInfo linkInfo)
    {
        KycLog!.Token = linkInfo.Token;
        KycLog.FarashenasaLink = linkInfo.FarashenasaLink;
    }

    private async Task AddKycLog(CancellationToken cancellationToken)
    {
        await userManagementKycLogRepository.AddAsync(KycLog!, cancellationToken);
        await userManagementKycLogRepository.SaveChangeAsync(cancellationToken);
    }

    [GeneratedRegex(@"^(?:[0-9]{10}|[0-9][A-Z][0-9]{8})$")]
    private static partial Regex NationalCardRegex();
}