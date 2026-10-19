using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Common.Interfaces;

namespace IdentityService.Application.Services.Kyc.Factories;

public interface IKycServiceFactory
{
    IKycServiceProvider Create(EKycProvider kycProvider);
}