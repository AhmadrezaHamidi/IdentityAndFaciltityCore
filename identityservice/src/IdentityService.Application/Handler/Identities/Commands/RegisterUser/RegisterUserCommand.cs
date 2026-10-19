using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.RegisterUser;

public record RegisterUserCommand(string NationalCode, string MobileNumber, string? InvitedCode,
    string RegisterToken) : BaseCommand<BaseMessageResponse<RegisterUserDto>>;