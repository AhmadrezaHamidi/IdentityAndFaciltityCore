using IdentityService.Domain.Entity.Profile.UserPorfile;
using IdentityService.Domain.IRepositories;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Handler.User;
public class GetUserByMobileNumberHandler : IBaseHandler<InternalGetUserByMobileNumberRequest, BaseMessageResponse<InternalGetUserByMobileNumberResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public GetUserByMobileNumberHandler(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }
    public async Task<BaseMessageResponse<InternalGetUserByMobileNumberResponse>> Handle(InternalGetUserByMobileNumberRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<InternalGetUserByMobileNumberResponse>(EResponseStatusCode.DoneSuccessfully, new());

        var findUser = await _userRepository.GetByMobileNumberAsync(request.MobileNumber, cancellationToken);

        if (findUser is null)
            return MessageResponse.GenerateMessageResponse<InternalGetUserByMobileNumberResponse>(EResponseStatusCode.ItemIsNotFound);
        var userData = new InternalGetUserByMobileNumberResponse()
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

public record InternalGetUserByMobileNumberRequest(
    string MobileNumber
) : BaseCommand<BaseMessageResponse<InternalGetUserByMobileNumberResponse>>;

public class InternalGetUserByMobileNumberResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MobileNumber { get; set; }
    public string UserName { get; set; }
    public string NationalCode { get; set; }
}