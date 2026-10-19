namespace IdentityService.Application.Handler.User;

public class AutoModifyCustomerNumberHandler : IBaseHandler<AutoModifyCustomerNumberRequest, BaseMessageResponse<AutoModifyCustomerNumberResponse>>
{
    public async Task<BaseMessageResponse<AutoModifyCustomerNumberResponse>> Handle(AutoModifyCustomerNumberRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<AutoModifyCustomerNumberResponse>(EResponseStatusCode.DoneSuccessfully, new());

        return response;
    }
}

public record AutoModifyCustomerNumberRequest() : BaseCommand<BaseMessageResponse<AutoModifyCustomerNumberResponse>>;

public class AutoModifyCustomerNumberResponse
{
    public long? BankCustomerNumber { get; set; }
}