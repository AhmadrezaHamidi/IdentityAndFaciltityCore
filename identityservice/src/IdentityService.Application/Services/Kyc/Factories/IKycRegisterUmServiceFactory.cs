using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Common.Interfaces;

namespace IdentityService.Application.Services.Kyc.Factories;

public interface IKycRegisterUmServiceFactory
{
    IKycRegisterUmService Create(EKycProvider kycProvider);
}