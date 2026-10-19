using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Contracts.Identities.Repositories.PasswordChangeActiveToken;
using IdentityService.Services.IServices;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.JwtHelper;

namespace IdentityService.Application.Handler.Identities.Services.OtpService;

public class ChangePasswordOtpService(
    IPasswordChangeActiveTokenCommandRepository activeTokenCommandRepository,
    IPasswordChangeActiveTokenQueryRepository activeTokenQueryRepository,
    ISmsService smsService,
    IConfiguration configuration) : OtpService<SendPasswordChangeOtpDto, VerifyPasswordChangeOtpDto>(
    activeTokenCommandRepository, 
    activeTokenQueryRepository,
    smsService, configuration);