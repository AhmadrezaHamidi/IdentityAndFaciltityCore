using System.Text.RegularExpressions;
using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Common.Interfaces;
using IdentityService.Application.Services.Kyc.Factories;
using IdentityService.Application.Services.Kyc.Vinara;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.Person;
using IdentityService.Domain.Entity.UserManagementKycLog;
using OmidBank.Common.Extentions;
using OmidBank.Common.InternalThreadParty.OmidTechAuthentication.Models;

namespace IdentityService.Application.Services.Kyc.Farashenasa;

public partial class FarashenasaKycRegisterUmServiceOld(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IUserManagementRepository userManagementRepository,
    IKycServiceFactory kycServiceFactory) : IKycRegisterUmService
{
    public EKycProvider Provider { get; set; } = EKycProvider.Farashenasa;

    public async Task<BaseMessageResponse<RegisterUMResponse>> Register(RegisterUMRequest request, CancellationToken cancellationToken)
    {
        if (userManagementRepository.CheckExistByUserId(request.CurrentUser!.Id))
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.YouHaYouAlreadyHaveTwoFactorAuthentication)!;

        if(!NationalCardRegex().IsMatch(request.NationalCardSeries))
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.NationalCardSeries)!;

        if (((DateTime.Now - request.BirthDate.ToDateTime()).TotalDays / 365) < 18)
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.AgeNotValid)!;

        var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, new RegisterUMResponse());
        var kycLog = await userManagementKycLogRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);
        if (kycLog != null)
        {
            response.Data!.RedirectToUrl = kycLog.FarashenasaLink;
            return response!;
        }

        var serviceProvider = kycServiceFactory.Create(request.KycProvider);
        var linkInfo = await serviceProvider.SubmitAuthentication(request, cancellationToken);
        if (linkInfo is null)
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.CallErrorOmidTech)!;
        
        await HandleRegisterUm(request, kycLog, linkInfo, request.RequestInfo.AppId, cancellationToken);
        response.Data!.RedirectToUrl = linkInfo.FarashenasaLink;
        return response!;
    }

    private async Task HandleRegisterUm(RegisterUMRequest request, UserManagementKycLogEntity? entity, LinkInfo linkInfo, Enums.AppEnum appId, CancellationToken cancellationToken)
    {
        //double requestVersion = 0;
        //if (!string.IsNullOrEmpty(request.RequestInfo.Version))
        //    requestVersion = double.Parse(request.RequestInfo.Version);
        
        if (entity is null)
        {
            await userManagementKycLogRepository.AddAsync(new UserManagementKycLogEntity()
            {

                NationalCardSeries = request.NationalCardSeries.ToUpper(),
                BirthDate = request.BirthDate.ToDateTime(),// requestVersion >= 2.1 ? request.BirthDate.ToDateTime() : request.BirthDate.ToDateTimeMilliseconds(),
                Token = linkInfo.Token,
                TokenExpierTime = DateTime.Now.AddMinutes(20),
                FarashenasaLink = linkInfo.FarashenasaLink,
                KycStep = KycStep.GenerateLink,
                KycType = KycType.RegisterUM,
                UserId = request.CurrentUser!.Id,
                AppId = appId,
                KycProvider = (int)request.KycProvider,
            }, cancellationToken);
        }
        else
        {
            entity.NationalCardSeries = request.NationalCardSeries.ToUpper();
            entity.BirthDate = request.BirthDate.ToDateTime();  //requestVersion >= 2.1 ?request.BirthDate.ToDateTime() : request.BirthDate.ToDateTimeMilliseconds();
            entity.Token = linkInfo.Token;
            entity.TokenExpierTime = DateTime.Now.AddMinutes(20);
            entity.FarashenasaLink = linkInfo.FarashenasaLink;
            entity.AppId = appId;
            entity.KycProvider = (int)request.KycProvider;
            await userManagementKycLogRepository.UpdateAsync(entity, cancellationToken);
        }

        await userManagementKycLogRepository.SaveChangeAsync(cancellationToken);
    }
    //
    // private static bool ValidateUserName(string userName)
    // {
    //     return !userName.IsNullOrEmpty() && CommonHelper.IsValidUserNameUM(userName);
    // }
    //
    // private static bool ValidatePassword(string password)
    // {
    //     return !password.IsNullOrEmpty() && CommonHelper.IsValidPasswordUM(password);
    // }

    [GeneratedRegex(@"^(?:[0-9]{10}|[0-9][A-Z][0-9]{8})$")]
    private static partial Regex NationalCardRegex();
}