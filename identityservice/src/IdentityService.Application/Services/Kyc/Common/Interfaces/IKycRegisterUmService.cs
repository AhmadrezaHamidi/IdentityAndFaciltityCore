using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Common.Enums;

namespace IdentityService.Application.Services.Kyc.Common.Interfaces;

public interface IKycRegisterUmService
{
    EKycProvider Provider { get; set; }
    Task<BaseMessageResponse<RegisterUMResponse>> Register(RegisterUMRequest request, CancellationToken cancellationToken);
}