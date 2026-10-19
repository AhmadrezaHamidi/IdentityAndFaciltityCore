namespace IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.DeactiveUserState;

public record DeactiveUserStateCommand(string MobileNumber) : BaseCommand<BaseMessageResponse<object>>
{
}
