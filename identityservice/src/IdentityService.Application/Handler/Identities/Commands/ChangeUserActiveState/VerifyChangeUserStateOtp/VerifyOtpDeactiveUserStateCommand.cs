namespace IdentityService.Application.Handler.Identities.Commands.ChangeUserActiveState.VerifyChangeUserStateOtp;

public record VerifyOtpDeactiveUserStateCommand(string OtpCode) : BaseCommand<BaseMessageResponse<object>>
{
}
