using IdentityService.Domain.Entity.Basic.BankData;
namespace IdentityService.Application.Handler.Basic;

public class GetBankDataHandler : IBaseHandler<GetBankDataRequest, BaseMessageResponse<GetBankDataResponse>>
{
    private readonly IBankDataRepository _bankDataRepository;

    public GetBankDataHandler(IBankDataRepository bankDataRepository)
    {
        _bankDataRepository = bankDataRepository;
    }

    public async Task<BaseMessageResponse<GetBankDataResponse>> Handle(GetBankDataRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<GetBankDataResponse>(EResponseStatusCode.DoneSuccessfully, new());

        response.Data.Items = (await _bankDataRepository.GetBankDataAsync(request.CategoryType, cancellationToken)).Select(a => new GetBankDataItem()
        {
            BankId = a.BankId,
            Title = a.Title,
        }).ToList();
        return response;
    }
}

public record GetBankDataRequest([Required] Enums.OmidTechBasicDataCategoryType CategoryType) : BaseCommand<BaseMessageResponse<GetBankDataResponse>>;

public class GetBankDataResponse
{
    public List<GetBankDataItem> Items { get; set; } = new();
}

public class GetBankDataItem
{
    public string Title { get; set; }
    public string BankId { get; set; }
}