using IdentityService.Application.Handler.Identities.Commands.InternalActivationCode.Create;
using IdentityService.Application.Handler.Identities.Commands.InternalActivationCode.VerifyOtp;
using IdentityService.Contracts.Identities.Repositories.ActiveTokens;
using IdentityService.Domain.Models;

namespace IdentityService.Application.Handler.Identities.Services.ActivationCode;

public class ActivationCodeService(IActiveTokenCommandRepository activeTokenCommandRepository, IActiveTokenQueryRepository activeTokenQueryRepository) : IActivationCodeService
{
    public async Task<CreateActivationCodeResponse> Create(CreateActivationCodeCommand command, CancellationToken cancellationToken)
    {
        var activationCode = await GenerateAndStoreActivationCodeAsync(command.ActivationCodeType, command.MobileNumber, cancellationToken);
        if (activationCode is not null)
        {
            return new CreateActivationCodeResponse(activationCode.Code);
        }
        return null;
    }

    public async Task<bool> Verify(VerifyActivationCodeCommand request, CancellationToken cancellationToken)
    {
        return await CheckOtpCodeVerificationFailureAsync(request, cancellationToken);
    }

    #region [ - Private Method - ]
    private async Task<ActivationCodeModel> GenerateAndStoreActivationCodeAsync(Enums.ActivationCode activationCodeType, string mobileNumber, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(Enums.ActivationCode), activationCodeType))
        {
            return null;
        }

        var activationCode = new ActivationCodeModel(mobileNumber)
        {
            Type = activationCodeType
        };

        activationCode.GenerateCode(false);
        activationCode.SetTime(DateTime.Now);

        await activeTokenCommandRepository.DeactivateActivationCodeByMobileNumber(mobileNumber, cancellationToken);
        await activeTokenCommandRepository.InsertActivationCodeAsync(activationCode, cancellationToken);

        return activationCode;
    }
    private async Task<bool> CheckOtpCodeVerificationFailureAsync(VerifyActivationCodeCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(Enums.ActivationCode), request.ActivationCodeType))
        {
            return false;
        }

        var activationCode = new ActivationCodeModel(request.RequestInfo.MobileNumber, request.Otp);

        activationCode.SetTime(DateTime.Now);
        activationCode.Type = request.ActivationCodeType;

        var status = await activeTokenQueryRepository.VerifyActivationCodeAsync(activationCode, cancellationToken);
        await activeTokenCommandRepository.DeactivateActivationCodeAsync(activationCode, cancellationToken);

        return status;
    }

    #endregion
}
