using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;

namespace IdentityService.Application.Handler.UserManagement;

public class VinaraPushApiVerificationRequestHandler(VinaraPushVerificationService pushVerificationService) : IBaseHandler<VinaraPushApiVerificationRequest, VinaraBaseResponse<object>>
{
    public async Task<VinaraBaseResponse<object>> Handle(VinaraPushApiVerificationRequest request, CancellationToken cancellationToken)
    {
        var result = await pushVerificationService.Handle(request, cancellationToken);
        var response = new VinaraBaseResponse<object>
        {
            Timestamp = DateTime.UtcNow.ToString("O"),
            Success = result.IsSuccess,
        };
        return response;
    }
}