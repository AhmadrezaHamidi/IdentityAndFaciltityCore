using IdentityService.Domain.IRepositories;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Handler.User;

public class GetPersonByNationalCodeHandler : IBaseHandler<InternalGetUserByNationalCodeRequest, BaseMessageResponse<InternalGetUserByNationalCodeResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public GetPersonByNationalCodeHandler(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }
    public async Task<BaseMessageResponse<InternalGetUserByNationalCodeResponse>> Handle(InternalGetUserByNationalCodeRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<InternalGetUserByNationalCodeResponse>(EResponseStatusCode.DoneSuccessfully, new());

        var findUser = await _userRepository.GetPersonByNationalCodeAsync(request.NationalCode, cancellationToken);

        if (findUser is null)
            return MessageResponse.GenerateMessageResponse<InternalGetUserByNationalCodeResponse>(EResponseStatusCode.ItemIsNotFound);
        var userData = new InternalGetUserByNationalCodeResponse()
        {
            Id = findUser.Id,
            FirstName = findUser.FirstName,
            LastName = findUser.LastName,
            MobileNumber = findUser.MobileNumber,
            NationalCode = findUser.NationalCode,
        };

        response.Data = userData;
        return response;
    }
}

public record InternalGetUserByNationalCodeRequest(
    string NationalCode
) : BaseCommand<BaseMessageResponse<InternalGetUserByNationalCodeResponse>>;

public class InternalGetUserByNationalCodeResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MobileNumber { get; set; }
    public string UserName { get; set; }
    public string NationalCode { get; set; }
}
