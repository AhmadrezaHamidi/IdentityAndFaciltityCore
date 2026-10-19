using IdentityService.Domain.Entity.Basic.BankDataCity;

namespace IdentityService.Application.Handler.Basic;

public class GetBankDataCityHandler: IBaseHandler<GetBankDataCityRequest, BaseMessageResponse<GetBankDataCityResponse>>
{
    private readonly IBankDataCityRepository _bankDataCityRepository;

    public GetBankDataCityHandler(IBankDataCityRepository bankDataCityRepository)
    {
        _bankDataCityRepository = bankDataCityRepository;
    }

    public async Task<BaseMessageResponse<GetBankDataCityResponse>> Handle(GetBankDataCityRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<GetBankDataCityResponse>(EResponseStatusCode.DoneSuccessfully, new());

        response.Data.Items = (await _bankDataCityRepository.GetByBankStateIdAsync(request.BankStateId,cancellationToken)).Select(a => new GetBankDataCityItem()
        {
            BankId = a.BankId,
            Title = a.Title,
            BankStateId = a.BankStateId,
        }).ToList();
        return response;
    }
}

public record GetBankDataCityRequest([Required] string BankStateId) : BaseCommand<BaseMessageResponse<GetBankDataCityResponse>>;

public class GetBankDataCityResponse
{
    public List<GetBankDataCityItem> Items { get; set; } = new();
}

public class GetBankDataCityItem
{
    public string Title { get; set; }
    public string BankId { get; set; }
    public string BankStateId { get; set; }
}