using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.ChangeUsername;

public record ChangeUsernameCommand(string CurrentUsername, string NewUsername, string Token) : BaseCommand<BaseMessageResponse<ChangeUsernameDto>>;