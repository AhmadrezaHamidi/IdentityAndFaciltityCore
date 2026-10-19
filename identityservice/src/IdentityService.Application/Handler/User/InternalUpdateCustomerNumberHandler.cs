using IdentityService.Application.Handler.User.Shared;
using IdentityService.Domain.Entity.Profile.UserAddress;
using IdentityService.Domain.IRepositories;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Handler.User;

public class InternalUpdateCustomerNumberHandler : IBaseHandler<InternalUpdateCustomerNumberRequest, BaseMessageResponse<InternalUpdateCustomerNumberResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public InternalUpdateCustomerNumberHandler( IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _userRepository = userRepository;
    }


    public async Task<BaseMessageResponse<InternalUpdateCustomerNumberResponse>> Handle(InternalUpdateCustomerNumberRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<InternalUpdateCustomerNumberResponse>(EResponseStatusCode.DoneSuccessfully, new());

        var findUser = await _userRepository.FindById(request.UserId, cancellationToken);

        if (findUser is null)
            return MessageResponse.GenerateMessageResponse<InternalUpdateCustomerNumberResponse>(EResponseStatusCode.ItemIsNotFound);


        findUser.BankCustomerNumber = request.BankCustomerNumber;
        await _userRepository.UpdateAsync(findUser, cancellationToken);
        await _userRepository.SaveChangeAsync(cancellationToken);

        return response;
    }
}

public record InternalUpdateCustomerNumberRequest(
    [Required] int UserId,
    long? BankCustomerNumber
) : BaseCommand<BaseMessageResponse<InternalUpdateCustomerNumberResponse>>;

public class InternalUpdateCustomerNumberResponse : UserAddressResponseDto
{
}