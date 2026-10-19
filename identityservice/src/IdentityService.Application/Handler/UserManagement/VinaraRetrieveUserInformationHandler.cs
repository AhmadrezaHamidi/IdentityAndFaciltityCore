using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;

namespace IdentityService.Application.Handler.UserManagement;

public class VinaraRetrieveUserInformationHandler(RetrieveUserInformationService userInformationService) : IBaseHandler<VinaraRetrieveUserInformationRequest, VinaraBaseResponse<VinaraRetrieveUserInformationResponse>>
{
    public async Task<VinaraBaseResponse<VinaraRetrieveUserInformationResponse>> Handle(VinaraRetrieveUserInformationRequest request, CancellationToken cancellationToken)
    {
        var result = await userInformationService.Handle(request, cancellationToken);
        return result;
    }
}