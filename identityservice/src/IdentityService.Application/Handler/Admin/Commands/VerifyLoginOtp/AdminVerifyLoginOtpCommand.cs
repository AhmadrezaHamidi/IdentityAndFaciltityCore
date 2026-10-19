using IdentityService.Contracts.Admin.CommandResponses;

namespace IdentityService.Application.Handler.Admin.Commands.AdminVerifyLoginOtp;

public record AdminVerifyLoginOtpCommand(string OtpCode) : BaseCommand<BaseMessageResponse<AdminVerifyLoginOtpDto>>;