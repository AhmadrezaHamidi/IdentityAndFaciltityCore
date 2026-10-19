using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Common.Interfaces;

namespace IdentityService.Application.Services.Kyc.Factories;

public class KycRegisterUmServiceFactory(IEnumerable<IKycRegisterUmService> kycUmRegisterServices) : IKycRegisterUmServiceFactory
{
    private readonly Dictionary<EKycProvider, IKycRegisterUmService> _kycServices = kycUmRegisterServices.ToDictionary(x => x.Provider);
    public IKycRegisterUmService Create(EKycProvider kycProvider)
    {
        if (_kycServices.TryGetValue(kycProvider, out var kycService))
            return kycService;

        throw new InvalidOperationException($"No kyc um register service registered with type {kycProvider}.");
    }
}