using IdentityService.Application.Handler.Identities.Services.ActivationCode;

namespace IdentityService.Application.Handler.Identities.Commands.InternalActivationCode.Create;

public class CreateActivationCodeHandler(IActivationCodeService activationCodeService) : IBaseHandler<CreateActivationCodeCommand, BaseMessageResponse<CreateActivationCodeResponse>>
{
    public async Task<BaseMessageResponse<CreateActivationCodeResponse>> Handle(CreateActivationCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var activationCodeResponse = await activationCodeService.Create(request, cancellationToken);
            if (activationCodeResponse != null)
            {
                return MessageResponse.GenerateMessageResponse<CreateActivationCodeResponse>(EResponseStatusCode.DoneSuccessfully, activationCodeResponse)!;
            }
            return MessageResponse.GenerateMessageResponse<CreateActivationCodeResponse>(EResponseStatusCode.BadRequest)!;
        }
        catch (Exception)
        {
            return MessageResponse.GenerateMessageResponse<CreateActivationCodeResponse>(EResponseStatusCode.InternalServerError)!;
        }
    }
}
public record CreateActivationCodeCommand(Enums.ActivationCode ActivationCodeType, string MobileNumber) : BaseCommand<BaseMessageResponse<CreateActivationCodeResponse>>;
public record CreateActivationCodeResponse(string Otp);
