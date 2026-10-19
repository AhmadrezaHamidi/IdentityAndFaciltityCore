using IdentityService.Application.Handler.Admin.Services;
using IdentityService.Contracts.Admin.CommandResponses;

namespace IdentityService.Application.Handler.Admin.Commands.AdminLoginWithPhoneNumber;

public class AdminLoginWithPhoneNumberCommandHandler(IAdminIdentityService identityService) : IBaseHandler<AdminLoginWithPhoneNumberCommand, BaseMessageResponse<AdminLoginWithPhoneNumberDto>>
{
    public async Task<BaseMessageResponse<AdminLoginWithPhoneNumberDto>> Handle(AdminLoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var response = await identityService.AdminLoginWithPhoneNumber(request, cancellationToken);
        return response;
    }
}