using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Identities.Repositories.UsernameChangeActiveToken;
using IdentityService.Services.IServices;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.JwtHelper;

namespace IdentityService.Application.Handler.Identities.Services.OtpService;

public class ChangeUsernameOtpService(
    IUsernameChangeActiveTokenCommandRepository activeTokenCommandRepository,
    IUsernameChangeActiveTokenQueryRepository activeTokenQueryRepository,
    ISmsService smsService,
    IConfiguration configuration) : OtpService<SendUsernameChangeOtpDto, VerifyUsernameChangeOtpDto>(
    activeTokenCommandRepository, 
    activeTokenQueryRepository,
    smsService, configuration);