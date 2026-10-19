using IdentityService.Application.Handler.Admin.Commands.AdminLoginWithPhoneNumber;
using IdentityService.Application.Handler.Admin.Commands.AdminVerifyLoginOtp;
using IdentityService.Contracts.Admin.CommandResponses;

namespace IdentityService.Application.Handler.Admin.Services;

public interface IAdminIdentityService
{
    Task<BaseMessageResponse<AdminLoginWithPhoneNumberDto>> AdminLoginWithPhoneNumber(AdminLoginWithPhoneNumberCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<AdminVerifyLoginOtpDto>> AdminVerifyLoginOtp(AdminVerifyLoginOtpCommand request, CancellationToken cancellationToken);
}