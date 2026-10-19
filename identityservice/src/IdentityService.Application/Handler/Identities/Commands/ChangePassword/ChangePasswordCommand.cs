using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.ChangePassword;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword, string Token) : BaseCommand<BaseMessageResponse<ChangePasswordDto>>;