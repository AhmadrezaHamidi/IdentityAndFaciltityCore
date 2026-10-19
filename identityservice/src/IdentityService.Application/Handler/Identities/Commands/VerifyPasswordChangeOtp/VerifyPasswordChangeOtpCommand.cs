using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.VerifyPasswordChangeOtp;

public record VerifyPasswordChangeOtpCommand(string OtpCode) : BaseCommand<BaseMessageResponse<VerifyPasswordChangeOtpDto>>;