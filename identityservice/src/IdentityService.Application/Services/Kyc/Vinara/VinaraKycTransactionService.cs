using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.UserManagement.UserManagementKycTransaction;
using IdentityService.Domain.IRepositories;

namespace IdentityService.Application.Services.Kyc.Vinara;

public class VinaraKycTransactionService(
    IUserManagementKycTransactionRepository userManagementKycTransactionRepository,
    KycResultManagement kycResultManagement)
{
    public KycStep ProcessUmStatus(string status)
    {
        return status switch
        {
            "submitted" => KycStep.GenerateLink,
            "accept" => KycStep.Approved,
            "reject" or "timeout" or "personal_info_not_found" or "personal_image_not_found" => KycStep.Rejected,
            _ => KycStep.Rejected
        };
    }
    
    public KycStep ProcessUmStatus(int status)
    {
        return status switch
        {
            >= 1000 and <= 1005 => KycStep.GenerateLink,
            2004 => KycStep.Approved,
            >= 3000 and <= 3012 => KycStep.Rejected,
            >= 2000 and <= 2002 => KycStep.Pending,
            _ => KycStep.Rejected
        };
    }

    public async Task Handle(UserManagementKycLogEntity umKycLog, KycStep status, CancellationToken cancellationToken)
    {
        if (status == KycStep.Approved)
            await AddKycTransaction(umKycLog, cancellationToken);
        await kycResultManagement.Handle(umKycLog, status, cancellationToken);
    }
    
    private async Task AddKycTransaction(UserManagementKycLogEntity umKycLog, CancellationToken cancellationToken)
    {
        var kycVinaraTransaction = new UserManagementKycTransactionEntity
        {
            KycType = umKycLog.KycType!.Value,
            AppId = umKycLog.AppId,
            UserId = umKycLog.UserId,
            CreateDateTime = DateTime.UtcNow,
            BirthDate = umKycLog.BirthDate ?? DateTime.MinValue,
            ProviderType = (int)EKycProvider.Vinara,
            Token = umKycLog.Token,
            IsWagePaid = false,
            NationalCardSeries = umKycLog.NationalCardSeries,
            PaidWageDateTime = DateTime.Now,
        };
        await userManagementKycTransactionRepository.AddAsync(kycVinaraTransaction, cancellationToken);
    }
}