using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.Person;
using IdentityService.Domain.Entity.UserManagement.UserManagementKycTransaction;
using IdentityService.Domain.Entity.UserManagementKycLog;
using IdentityService.Domain.IRepositories;

namespace IdentityService.Application.Services.Kyc.Common;

public class KycResultManagement(
    IUserManagementKycLogRepository userManagementKycLogRepository,
    IUserManagementRepository userManagementRepository,
    IUserManagementKycTransactionRepository userManagementKycTransactionRepository,
    IUserRepository userRepository,
    IPersonRepository personRepository)
{
    public async Task Handle(string token, KycStep umStatus, CancellationToken cancellationToken)
    {
        var umKycLog = await userManagementKycLogRepository.GetByKycTokenAsync(token, cancellationToken);
        await Handle(umKycLog, umStatus, cancellationToken);
    }

    public async Task Handle(UserManagementKycLogEntity umKycLog, KycStep umStatus, CancellationToken cancellationToken)
    {
        switch (umStatus)
        {
            case KycStep.GenerateLink:
                await HandleSubmitStatus(umKycLog, cancellationToken);
                break;
            case KycStep.Pending:
                await HandlePendingStatus(umKycLog, cancellationToken);
                break;
            case KycStep.Approved:
                await HandleApprovedStatus(umKycLog, cancellationToken);
                break;
            case KycStep.Rejected:
                await HandleRejectedStatus(umKycLog, cancellationToken);
                break;
            case KycStep.NotExistUser:
            case KycStep.IsNotBankCustomer:
            case KycStep.IsNotBankCustomerEnableCreateCustomer:
            case KycStep.CustomerPending:
            default:
                break;
        }
    }

    private async Task HandlePendingStatus(UserManagementKycLogEntity umKycLog, CancellationToken cancellationToken)
    {
        if (umKycLog.KycStep == KycStep.Pending)
            return;
        umKycLog.KycStep = KycStep.Pending;
        await userManagementKycLogRepository.UpdateAsync(umKycLog, cancellationToken);
        await userManagementKycLogRepository.SaveChangeAsync(cancellationToken);
    }

    private async Task HandleSubmitStatus(UserManagementKycLogEntity umKycLog, CancellationToken cancellationToken)
    {
        if (umKycLog.KycStep == KycStep.GenerateLink)
            return;
        umKycLog.KycStep = KycStep.GenerateLink;
        await userManagementKycLogRepository.UpdateAsync(umKycLog, cancellationToken);
        await userManagementKycLogRepository.SaveChangeAsync(cancellationToken);
    }

    private async Task HandleApprovedStatus(UserManagementKycLogEntity umKycLog, CancellationToken cancellationToken)
    {
        if (umKycLog.KycProvider == (int)EKycProvider.Farashenasa)
            await DeliverService(umKycLog, cancellationToken);
        await userManagementKycLogRepository.DeleteAsync(umKycLog, cancellationToken);
        await userManagementRepository.SaveChangeAsync(cancellationToken);
    }

    public async Task DeliverService(UserManagementKycLogEntity umKycLog, CancellationToken cancellationToken)
    {
        switch (umKycLog.KycType)
        {
            case KycType.RegisterUM:
                {
                    await userManagementRepository.AddAsync(new UserManagmentEntity
                    {
                        AutoUpdateCif = true,
                        UserId = umKycLog.UserId,
                        CreateDate = DateTime.Now,
                    }, cancellationToken);

                    var findUser = await userRepository.FindById(umKycLog.UserId, cancellationToken);
                    if (findUser is not null)
                    {
                        findUser.Person.BirthDate = umKycLog.BirthDate;
                        findUser.Person.NationalCardSeries = umKycLog.NationalCardSeries;
                        await personRepository.UpdateAsync(findUser.Person, cancellationToken);
                    }

                    break;
                }
            case KycType.ForgetPasswordUM:
                {
                    var findUserManagement = await userManagementRepository.GetByUserIdAsync(umKycLog.UserId, cancellationToken);
                    findUserManagement!.PasswordUpdateDate = DateTime.Now;
                    await userManagementRepository.UpdateAsync(findUserManagement, cancellationToken);
                    await userManagementRepository.LogoutUM(findUserManagement.User.Person.MobileNumber, cancellationToken);
                    break;
                }
            case KycType.Certificate:
                {
                    var findUserManagement = await userManagementRepository.GetByUserIdAsync(umKycLog.UserId, cancellationToken);
                    findUserManagement!.TakeCertificateDate = DateTime.Now;

                    var findUser = await userRepository.FindById(umKycLog.UserId, cancellationToken);
                    if (findUser is not null)
                    {
                        findUser.Person.NationalCardSeries = umKycLog.NationalCardSeries;
                        await personRepository.UpdateAsync(findUser.Person, cancellationToken);
                    }
                    break;
                }
            default:
                break;
        }
    }

    public async Task<bool> DeliverService(int userId, CancellationToken cancellationToken)
    {
        var kycTransactionLog = await userManagementKycTransactionRepository.GetByKycUserIdAsync(userId, cancellationToken);
        if (kycTransactionLog is null)
            return false;
        await DeliverService(kycTransactionLog, cancellationToken);
        return true;
    }

    public async Task DeliverService(UserManagementKycTransactionEntity umKycLog, CancellationToken cancellationToken)
    {
        switch (umKycLog.KycType)
        {
            case KycType.RegisterUM:
                {
                    await userManagementRepository.AddAsync(new UserManagmentEntity
                    {
                        AutoUpdateCif = true,
                        UserId = umKycLog.UserId,
                        CreateDate = DateTime.Now,
                    }, cancellationToken);
                    umKycLog.IsWagePaid = true;
                    umKycLog.PaidWageDateTime = DateTime.Now;
                    await userManagementKycTransactionRepository.UpdateAsync(umKycLog, cancellationToken);
                    var findUser = await userRepository.FindById(umKycLog.UserId, cancellationToken);
                    if (findUser is not null)
                    {
                        findUser.Person.BirthDate = umKycLog.BirthDate;
                        findUser.Person.NationalCardSeries = umKycLog.NationalCardSeries;
                        await personRepository.UpdateAsync(findUser.Person, cancellationToken);
                    }
                    break;
                }
            case KycType.ForgetPasswordUM:
                {
                    var findUserManagement = await userManagementRepository.GetByUserIdAsync(umKycLog.UserId, cancellationToken);
                    findUserManagement!.PasswordUpdateDate = DateTime.Now;
                    await userManagementRepository.UpdateAsync(findUserManagement, cancellationToken);
                    await userManagementRepository.LogoutUM(findUserManagement.User.Person.MobileNumber, cancellationToken);
                    break;
                }
            case KycType.Certificate:
                {
                    var findUserManagement = await userManagementRepository.GetByUserIdAsync(umKycLog.UserId, cancellationToken);
                    findUserManagement!.TakeCertificateDate = DateTime.Now;
                    break;
                }
            default:
                break;
        }

        await userManagementRepository.SaveChangeAsync(cancellationToken);
    }

    private async Task HandleRejectedStatus(UserManagementKycLogEntity umKycLog, CancellationToken cancellationToken)
    {
        await userManagementKycLogRepository.DeleteAsync(umKycLog, cancellationToken);
        await userManagementKycLogRepository.SaveChangeAsync(cancellationToken);
    }

    private async Task HandleNotFoundStatus(UserManagementKycLogEntity umKycLog, CancellationToken cancellationToken)
    {
        await userManagementKycLogRepository.DeleteAsync(umKycLog, cancellationToken);
        await userManagementKycLogRepository.SaveChangeAsync(cancellationToken);
    }
}