using IdentityService.Application.Handler.Identities.Services.ChangeUserActiveState;

namespace IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.DeactiveUserState;

public class DeactiveUserStateCommandHandler(IChangeUserActiveStateService changeUserActiveStateService) : IBaseHandler<DeactiveUserStateCommand, BaseMessageResponse<object>>
{
    public async Task<BaseMessageResponse<object>> Handle(DeactiveUserStateCommand request, CancellationToken cancellationToken)
    {
        var response = await changeUserActiveStateService.DeactiveUser(request, cancellationToken);
        return response;
    }
}
