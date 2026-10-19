using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Domain.Entity.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Factories;


namespace IdentityService.Application.Handler.UserManagementKycLog;

public class ExternalUpdateUMHandler(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    KycSmsHelper kycSmsHelper,
    IKycServiceFactory kycServiceFactory) : IBaseHandler<ExternalUpdateUMRequest, BaseMessageResponse<ExternalUpdateUMResponse>>
{
    public async Task<BaseMessageResponse<ExternalUpdateUMResponse>> Handle(ExternalUpdateUMRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, new ExternalUpdateUMResponse());

        var kycLog = await userManagementKycLogRepository.GetByKycTokenAsync(request.UniqueKey, cancellationToken);
        if (kycLog is null)
            return MessageResponse.GenerateMessageResponse<ExternalUpdateUMResponse>(EResponseStatusCode.ItemIsNotFound)!;

        var inquiryService = kycServiceFactory.Create(EKycProvider.Farashenasa);
        var result = await inquiryService.InquiryAsync(request.UniqueKey, (KycResult)Enum.Parse(typeof(KycResult), request.Status), cancellationToken);
        response.StatusCode = result.StatusCode;
        response.Message = result.Message;


        kycLog.KycStep = result.Data!.UmStatus;
        await kycSmsHelper.SendInquiryResultBySmsAsync(kycLog, cancellationToken);
        response.Data!.Key = request.UniqueKey;
        response.Data.Status = result.StatusCode == EResponseStatusCode.DoneSuccessfully ? "DONE" : "FAILED";
        return response!;
    }
}

public record ExternalUpdateUMRequest([Required] string UniqueKey, [Required] string Status)
    : BaseCommand<BaseMessageResponse<ExternalUpdateUMResponse>>;

public class ExternalUpdateUMResponse
{
    public string Status { get; set; }
    public string Key { get; set; }
}

public enum KycResult : short
{
    APPROVED = 1,
    REJECT = 2
}