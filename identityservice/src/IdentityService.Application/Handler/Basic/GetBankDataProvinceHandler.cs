using IdentityService.Domain.Entity.Basic.BankDataProvince;

namespace IdentityService.Application.Handler.Basic;

public class GetBankDataProvinceHandler : IBaseHandler<GetBankDataProvinceRequest, BaseMessageResponse<GetBankDataProvinceResponse>>
{
    private readonly IBankDataProvinceRepository _bankDataProvinceRepository;

    public GetBankDataProvinceHandler(IBankDataProvinceRepository bankDataProvinceRepository)
    {
        _bankDataProvinceRepository = bankDataProvinceRepository;
    }

    public async Task<BaseMessageResponse<GetBankDataProvinceResponse>> Handle(GetBankDataProvinceRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<GetBankDataProvinceResponse>(EResponseStatusCode.DoneSuccessfully, new());

        response.Data.Items = (await _bankDataProvinceRepository.GetAllAsync(cancellationToken)).Select(a => new GetBankDataProvinceItem()
        {
            BankId = a.BankId,
            Title = a.Title,
        }).ToList();
        return response;
    }
}

public record GetBankDataProvinceRequest() : BaseCommand<BaseMessageResponse<GetBankDataProvinceResponse>>;

public class GetBankDataProvinceResponse
{
    public List<GetBankDataProvinceItem> Items { get; set; } = new();
}

public class GetBankDataProvinceItem
{
    public string Title { get; set; }
    public string BankId { get; set; }
}