using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Factories;

namespace IdentityService.Application.Handler.UserManagement;

public class CheckStatusForgetPasswordHandler(
    IUserManagementRepository userManagementRepository,
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IKycServiceFactory kycServiceFactory) : IBaseHandler<CheckStatusForgetPasswordRequest, BaseMessageResponse<CheckStatusForgetPasswordResponse>>
{
    public async Task<BaseMessageResponse<CheckStatusForgetPasswordResponse>> Handle(CheckStatusForgetPasswordRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, new CheckStatusForgetPasswordResponse());

        var userManagement = await userManagementRepository.GetByUserIdAsync(request.CurrentUser!.Id, cancellationToken);
        if (userManagement is null)
            return MessageResponse.GenerateMessageResponse<CheckStatusForgetPasswordResponse>(EResponseStatusCode.ItemIsNotFound)!;

        var kycLog = await userManagementKycLogRepository.GetByUserIdAsync(request.CurrentUser.Id, cancellationToken);
        if (kycLog is null) 
            return response!;
        if (kycLog.KycType != KycType.ForgetPasswordUM) 
            return response!;
        
        switch (kycLog.KycStep)
        {
            case KycStep.GenerateLink when kycLog.TokenExpierTime > DateTime.Now:
                response.Data!.RedirectToUrl = kycLog.FarashenasaLink;
                response.Data.UMStatus = KycStep.GenerateLink;
                return response!;
            case KycStep.Pending:
            {
                var inquiryService = kycServiceFactory.Create(request.KycProvider);
                var result = await inquiryService.InquiryAsync(kycLog.Token, null, cancellationToken);
                response.Data!.UMStatus = result.Data!.UmStatus;
                break;
            }
        }
        return response!;
    }
}

public record CheckStatusForgetPasswordRequest : BaseCommand<BaseMessageResponse<CheckStatusForgetPasswordResponse>>
{
    public EKycProvider KycProvider;
}

public class CheckStatusForgetPasswordResponse
{
    public KycStep UMStatus { get; set; }
    public string RedirectToUrl { get; set; }

}