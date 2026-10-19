using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.SendPasswordChangeOtp;

public record SendPasswordChangeOtpCommand : BaseCommand<BaseMessageResponse<SendPasswordChangeOtpDto>>;