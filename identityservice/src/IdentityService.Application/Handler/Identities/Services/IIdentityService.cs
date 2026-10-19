using IdentityService.Application.Handler.Identities.Commands.ChangePassword;
using IdentityService.Application.Handler.Identities.Commands.ChangeUsername;
using IdentityService.Application.Handler.Identities.Commands.LoginWithPhoneNumber;
using IdentityService.Application.Handler.Identities.Commands.RegisterUser;
using IdentityService.Application.Handler.Identities.Commands.SendPasswordChangeOtp;
using IdentityService.Application.Handler.Identities.Commands.SendUsernameChangeOtp;
using IdentityService.Application.Handler.Identities.Commands.VerifyLoginOtp;
using IdentityService.Application.Handler.Identities.Commands.VerifyPasswordChangeOtp;
using IdentityService.Application.Handler.Identities.Commands.VerifyUsernameChangeOtp;
using IdentityService.Application.Handler.Identities.Queries.RetrieveSettingConfigurations;
using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Identities.ReadModels;

namespace IdentityService.Application.Handler.Identities.Services;

public interface IIdentityService
{
    Task<BaseMessageResponse<LoginWithPhoneNumberDto>> LoginWithPhoneNumber(LoginWithPhoneNumberCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<VerifyLoginOtpDto>> VerifyLoginOtp(VerifyLoginOtpCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<RegisterUserDto>> RegisterUser(RegisterUserCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<SendUsernameChangeOtpDto>> SendUsernameChangeOtp(SendUsernameChangeOtpCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<VerifyUsernameChangeOtpDto>> VerifyUsernameChangeOtp(VerifyUsernameChangeOtpCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<SendPasswordChangeOtpDto>> SendPasswordChangeOtp(SendPasswordChangeOtpCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<VerifyPasswordChangeOtpDto>> VerifyPasswordChangeOtp(VerifyPasswordChangeOtpCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<ChangePasswordDto>> ChangePassword(ChangePasswordCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<ChangeUsernameDto>> ChangeUsername(ChangeUsernameCommand request, CancellationToken cancellationToken);
    Task<BaseMessageResponse<RetrieveSettingConfigurationsDto>> RetrieveSettingConfigurations(RetrieveSettingConfigurationsQuery request, CancellationToken cancellationToken);
}