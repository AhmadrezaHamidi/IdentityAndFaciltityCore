using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.VerifyUsernameChangeOtp;

public record VerifyUsernameChangeOtpCommand(string OtpCode) : BaseCommand<BaseMessageResponse<VerifyUsernameChangeOtpDto>>;