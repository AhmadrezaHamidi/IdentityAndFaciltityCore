using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.SendUsernameChangeOtp;

public record SendUsernameChangeOtpCommand : BaseCommand<BaseMessageResponse<SendUsernameChangeOtpDto>>;