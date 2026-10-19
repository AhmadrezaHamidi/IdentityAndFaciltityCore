using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Common.Interfaces;

namespace IdentityService.Application.Services.Kyc.Factories;

public class KycServiceFactory(IEnumerable<IKycServiceProvider> kycServices) : IKycServiceFactory
{
    private readonly Dictionary<EKycProvider, IKycServiceProvider> _kycServices = kycServices.ToDictionary(x => x.Provider);
    public IKycServiceProvider Create(EKycProvider kycProvider)
    {
        if (_kycServices.TryGetValue(kycProvider, out var kycService))
            return kycService;

        throw new InvalidOperationException($"No kyc service registered with type {kycProvider}.");
    }
}