using IdentityService.Application.Services.Kyc.Vinara;
using IdentityService.Domain.IRepositories;

namespace IdentityService.Application.Handler.UserManagement;

public class RetrieveKycUmTransactionLogHandler(
    IUserManagementKycTransactionRepository userManagementKycTransactionRepository,
    VinaraWageService vinaraWageService) : IBaseHandler<RetrieveKycUmTransactionLogRequest, BaseMessageResponse<RetrieveKycUmTransactionLogResponse>>
{
    public async Task<BaseMessageResponse<RetrieveKycUmTransactionLogResponse>> Handle(RetrieveKycUmTransactionLogRequest request, CancellationToken cancellationToken)
    {
        var result = await userManagementKycTransactionRepository.GetByKycUserIdAsync(request.UserId, cancellationToken);
        if (result is null)
            return MessageResponse.GenerateMessageResponse<RetrieveKycUmTransactionLogResponse>(EResponseStatusCode.ItemIsNotFound)!;
        var data = new RetrieveKycUmTransactionLogResponse(vinaraWageService.RetrieveWage(), result!.IsWagePaid);
        return MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, data)!;
    }
}

public record RetrieveKycUmTransactionLogRequest(int UserId) : BaseCommand<BaseMessageResponse<RetrieveKycUmTransactionLogResponse>>;

public record RetrieveKycUmTransactionLogResponse(decimal Wage, bool IsWagePaid);