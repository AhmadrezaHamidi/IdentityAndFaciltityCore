using IdentityService.Application.Services.Kyc.Common;

namespace IdentityService.Application.Handler.UserManagement;

public class DeliverKycUmServiceHandler(KycResultManagement kycResultManagement) : IBaseHandler<DeliverKycUmServiceRequest, BaseMessageResponse<object>>
{
    public async Task<BaseMessageResponse<object>> Handle(DeliverKycUmServiceRequest request, CancellationToken cancellationToken)
    {
        var result = await kycResultManagement.DeliverService(request.UserId, cancellationToken);
        var response = MessageResponse.GenerateMessageResponse(result
            ? EResponseStatusCode.DoneSuccessfully
            : EResponseStatusCode.Failed);
        return response;
    }
}

public record DeliverKycUmServiceRequest(int UserId) : BaseCommand<BaseMessageResponse<object>>;