using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagementKycLog;

namespace IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement;

public class VinaraPushVerificationService(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    VinaraKycTransactionService kycTransactionService)
{
    public async Task<BaseMessageResponse<object>> Handle(VinaraPushApiVerificationRequest response, CancellationToken cancellationToken)
    {
        var umKycLog = await userManagementKycLogRepository.GetByKycTokenAsync(response.Token, cancellationToken);
        if (umKycLog is null)
            return MessageResponse.GenerateMessageResponse<object>(EResponseStatusCode.ItemIsNotFound)!;
        var status = ProcessUmStatus(response);
        await kycTransactionService.Handle(umKycLog, status, cancellationToken);
        return MessageResponse.GenerateMessageResponse<object>(EResponseStatusCode.DoneSuccessfully)!;
    }

    private KycStep ProcessUmStatus(VinaraPushApiVerificationRequest response)
    {
        return response.Error.Code != 1000 ? KycStep.Rejected : kycTransactionService.ProcessUmStatus(response.Status);
    }
}