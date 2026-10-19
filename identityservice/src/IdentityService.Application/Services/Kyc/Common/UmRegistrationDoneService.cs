using IdentityService.Domain.IRepositories;

namespace IdentityService.Application.Services.Kyc.Common;

public class UmRegistrationDoneService(
    IUserManagementKycTransactionRepository userManagementKycTransactionRepository,
    KycResultManagement kycResultManagement)
{
    public async Task Handle(int userId, CancellationToken cancellationToken)
    {
        var kycLog = await userManagementKycTransactionRepository.GetByKycUserIdAsync(userId, cancellationToken);
        await kycResultManagement.DeliverService(kycLog!, cancellationToken);
    }
}