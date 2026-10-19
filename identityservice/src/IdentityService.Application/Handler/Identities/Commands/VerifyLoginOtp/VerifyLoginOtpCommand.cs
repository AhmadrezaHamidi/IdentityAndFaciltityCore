using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.VerifyLoginOtp;

public record VerifyLoginOtpCommand(string OtpCode, string? InitToken) : BaseCommand<BaseMessageResponse<VerifyLoginOtpDto>>;