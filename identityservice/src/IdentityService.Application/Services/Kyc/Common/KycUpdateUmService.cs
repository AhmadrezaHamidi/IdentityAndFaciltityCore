using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Factories;
using IdentityService.Domain.Entity.UserManagementKycLog;

namespace IdentityService.Application.Services.Kyc.Common;

public class KycUpdateUmService(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    KycSmsHelper kycSmsHelper,
    IKycServiceFactory kycServiceFactory
    )
{
    public async Task<BaseMessageResponse<UpdateUMResponse>> Handle(UpdateUMRequest request, CancellationToken cancellationToken)
    {
        var kycLog = await userManagementKycLogRepository.GetByKycTokenAsync(request.Key, cancellationToken);
        if (kycLog == null)
            return MessageResponse.GenerateMessageResponse<UpdateUMResponse>(EResponseStatusCode.ItemIsNotFound)!;

        var inquiryService = kycServiceFactory.Create((EKycProvider)kycLog.KycProvider);
        var result = await inquiryService.InquiryAsync(request.Key, null, cancellationToken);
        var response = MessageResponse.GenerateMessageResponse(result.StatusCode, new UpdateUMResponse()
        {
            UMStatus = result.Data!.UmStatus,
            AppId = kycLog.AppId,
            KycType = kycLog.KycType
        });

        kycLog.KycStep = result.Data.UmStatus;
        kycLog.KycType = result.Data.KycType;
        await kycSmsHelper.SendInquiryResultBySmsAsync(kycLog, cancellationToken);

        return response!;
    }
}