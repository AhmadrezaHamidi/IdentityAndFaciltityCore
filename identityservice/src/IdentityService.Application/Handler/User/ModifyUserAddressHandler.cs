using IdentityService.Application.Handler.User.Shared;
using IdentityService.Domain.Entity.Profile.UserAddress;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.InternalThreadParty.OmidTech;
using OmidBank.Common.InternalThreadParty.OmidTech.Request;

namespace IdentityService.Application.Handler.User;

public class ModifyUserAddressHandler : IBaseHandler<ModifyUserAddressRequest, BaseMessageResponse<ModifyUserAddressResponse>>
{
    private readonly IUserAddressRepository _userAddressRepository;
    private readonly IConfiguration _configuration;

    public ModifyUserAddressHandler(IUserAddressRepository userAddressRepository, IConfiguration configuration)
    {
        _userAddressRepository = userAddressRepository;
        _configuration = configuration;
    }


    public async Task<BaseMessageResponse<ModifyUserAddressResponse>> Handle(ModifyUserAddressRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<ModifyUserAddressResponse>(EResponseStatusCode.DoneSuccessfully, new());

        var findUserAddress = await _userAddressRepository.FindByUserId(request.CurrentUser.Id, cancellationToken);

        if (findUserAddress is null)
            return MessageResponse.GenerateMessageResponse<ModifyUserAddressResponse>(EResponseStatusCode.InquiryShouldFirst);


        findUserAddress.Description = request.Description;
        findUserAddress.HouseNumber = request.HouseNumber;
        findUserAddress.Unit = request.Unit;
        findUserAddress.PhoneNumber = request.PhoneNumber;
        await _userAddressRepository.UpdateAsync(findUserAddress, cancellationToken);
        await _userAddressRepository.SaveChangeAsync(cancellationToken);

        return response;
    }
}

public record ModifyUserAddressRequest(
    [Required] string Description,
    [Required] double HouseNumber,
    [Required] string Unit,
    [Required] string PhoneNumber
) : BaseCommand<BaseMessageResponse<ModifyUserAddressResponse>>;

public class ModifyUserAddressResponse : UserAddressResponseDto
{
}