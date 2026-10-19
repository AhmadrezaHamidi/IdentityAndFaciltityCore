using IdentityService.Domain.IRepositories;
using IdentityService.Domain.Models;

namespace IdentityService.Application.Handler.User;

public class GeneratePersonOpenAccountHandler : IBaseHandler<GeneratePersonOpenAccountRequest, BaseMessageResponse<GeneratePersonOpenAccountResponse>>
{
    private readonly IUserRepository _userRepository;

    public GeneratePersonOpenAccountHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<BaseMessageResponse<GeneratePersonOpenAccountResponse>> Handle(
        GeneratePersonOpenAccountRequest request, CancellationToken cancellationToken)
    {
        var response =
            MessageResponse.GenerateMessageResponse<GeneratePersonOpenAccountResponse>(EResponseStatusCode
                .DoneSuccessfully);

        var uId = await _userRepository.GeneratePersonOpenAccountModel(
            new GeneratePersonOpenAccountModel
                { NationalCode = request.NationalCode, MobileNumber = request.MobileNumber },
            cancellationToken);
        if (uId != null)
        {
            response.Data=new();
            response.Data.UId = uId;
        }
        else
        {
            response =
                MessageResponse.GenerateMessageResponse<GeneratePersonOpenAccountResponse>(EResponseStatusCode
                    .ErrorOccurred);
        }

        return response;
    }
}

public record GeneratePersonOpenAccountRequest : BaseCommand<BaseMessageResponse<GeneratePersonOpenAccountResponse>>
{
    public string NationalCode { get; set; }
    public string MobileNumber { get; set; }
}

public class GeneratePersonOpenAccountResponse
{
    public string UId { get; set; }
}