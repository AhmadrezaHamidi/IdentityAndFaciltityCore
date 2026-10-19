using System.Threading;

namespace IdentityService.Domain.IRepositories
{
    public interface IKycRepository : IBaseRepository
    {
        KycModel GetKycById(int id, CancellationToken cancellationToken);
        KycModel GetKycByMobileNumber(string mobileNumber, CancellationToken cancellationToken);
        int InsertKyc(KycModel kycItem, CancellationToken cancellationToken);
        int UpdateKyc(KycModel kycItem, CancellationToken cancellationToken);
    }
}
