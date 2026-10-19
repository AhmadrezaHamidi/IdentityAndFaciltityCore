using IdentityService.Application.Handler.Identities.Services.ActivationCode;

namespace IdentityService.Application.Handler.Identities.Commands.InternalActivationCode.VerifyOtp;

public class VerifyActivationCodeHandler(IActivationCodeService activationCodeService) : IBaseHandler<VerifyActivationCodeCommand, BaseMessageResponse<VerifyActivationCodeResponse>>
{
    public async Task<BaseMessageResponse<VerifyActivationCodeResponse>> Handle(VerifyActivationCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await activationCodeService.Verify(request, cancellationToken);
            if (response)
            {
                return MessageResponse.GenerateMessageResponse<VerifyActivationCodeResponse>(EResponseStatusCode.DoneSuccessfully)!;
            }
            return MessageResponse.GenerateMessageResponse<VerifyActivationCodeResponse>(EResponseStatusCode.BadRequest)!;
        }
        catch (Exception)
        {
            return MessageResponse.GenerateMessageResponse<VerifyActivationCodeResponse>(EResponseStatusCode.InternalServerError)!;
        }
    }
}
public record VerifyActivationCodeCommand(Enums.ActivationCode ActivationCodeType, string MobileNumber, string Otp) : BaseCommand<BaseMessageResponse<VerifyActivationCodeResponse>>;
public record VerifyActivationCodeResponse();
