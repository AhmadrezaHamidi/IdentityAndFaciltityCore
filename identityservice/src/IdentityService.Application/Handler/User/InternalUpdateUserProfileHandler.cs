using IdentityService.Domain.Entity.Profile.UserPorfile;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Handler.User;

public class InternalUpdateUserProfileHandler : IBaseHandler<InternalUpdateUserProfileRequest, BaseMessageResponse<object>>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IConfiguration _configuration;

    public InternalUpdateUserProfileHandler(IConfiguration configuration, IUserProfileRepository userProfileRepository)
    {
        _configuration = configuration;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<BaseMessageResponse<object>> Handle(InternalUpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<object>(EResponseStatusCode.DoneSuccessfully);

        await _userProfileRepository.UpdateAsync(request.UserId, request.RealFirstName, request.RealLastName, cancellationToken);
        await _userProfileRepository.SaveChangeAsync(cancellationToken);
        return response;
    }
}


public record InternalUpdateUserProfileRequest([Required] int UserId, string? RealFirstName, string? RealLastName) : BaseCommand<BaseMessageResponse<object>>;