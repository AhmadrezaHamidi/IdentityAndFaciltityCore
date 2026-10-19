using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Common.Interfaces;
using IdentityService.Application.Services.Kyc.Factories;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagementKycLog;
using OmidBank.Common.Helper.Minio;
using OmidBank.Common.InternalThirdParty.OmidTech.Request;
using OmidBank.Common.InternalThreadParty.OmidTech;
using OmidBank.Common.InternalThreadParty.OmidTech.Request;

namespace IdentityService.Application.Services.Kyc.Vinara;

public class VinaraKycRegisterUmService(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IUserManagementRepository userManagementRepository,
    ImageStorageService imageStorageService,
    IKycServiceFactory kycServiceFactory,
    OmidTechProvider omidTechProvider) : KycRegisterUmBaseService(userManagementKycLogRepository, 
    userManagementRepository, kycServiceFactory, EKycProvider.Vinara), IKycRegisterUmService
{
    protected override async Task<BaseMessageResponse<RegisterUMResponse>> InquiryRequiredInformation(RegisterUMRequest request, CancellationToken cancellationToken)
    {
        var response = await InquiryPerson(request, cancellationToken);
        if(!response.IsSuccess)
            return response;
        return await RetrievePersonImage(request, cancellationToken);
    }

    private async Task<BaseMessageResponse<RegisterUMResponse>> InquiryPerson(RegisterUMRequest registerUmRequest, CancellationToken cancellationToken)
    {
        var request = new GetPersonByNationalCodeAndBirthdateRequest
        {
            BirthDate = DateTimeOffset.FromUnixTimeSeconds(registerUmRequest.BirthDate).UtcDateTime,
            NationalId = registerUmRequest.CurrentUser!.NationalCode,
        };
        var sabteAhvalResponse = await omidTechProvider.GetPersonByNationalCodeAndBirthDate(request, cancellationToken);
        if (sabteAhvalResponse is null)
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.CallErrorOmidTech)!;
        if (!sabteAhvalResponse.Success)
        {
            var response = MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.InvalidOperation)!;
            response.Message = "تاریخ تولد صحیح نیست";
            return response!;
        }

        KycLog!.Gender = Convert.ToInt32(sabteAhvalResponse.Data.Gender);
        return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.DoneSuccessfully)!;
    }

    private async Task<BaseMessageResponse<RegisterUMResponse>> RetrievePersonImage(RegisterUMRequest registerUmRequest, CancellationToken cancellationToken)
    {
        var request = new GetPersonImgRequest
        {
            NationalId = registerUmRequest.CurrentUser!.NationalCode,
            CardSerial = registerUmRequest.NationalCardSeries,
        };
        var personImg = await omidTechProvider.GetPersonImg(request, cancellationToken);
        if (personImg is null)
            return MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.CallErrorOmidTech)!;
        if (!personImg.IsSuccess)
        {
            var response = MessageResponse.GenerateMessageResponse<RegisterUMResponse>(EResponseStatusCode.InvalidOperation)!;
            response.Message = "سریال پشت کارت ملی صحیح نمی باشد";
            return response!;
        }

        var bytes = Convert.FromBase64String(personImg.ResultData.Image);
        KycLog!.ImageName = Guid.NewGuid() + ".png";
        await imageStorageService.UploadAsync(bytes, KycLog.ImageName, (int)Enums.UploladFileType.Image, cancellationToken);
        return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, new RegisterUMResponse())!;
    }
}
