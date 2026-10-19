using IdentityService.Application.Handler.User.Shared;
using IdentityService.Domain.Entity.Profile.UserPorfile;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Handler.User
{
    public class InternalUpdateSejamStatusHandler : IBaseHandler<InternalUpdateSejamStatusRequest, BaseMessageResponse<InternalUpdateSejamStatusResponse>>
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IConfiguration _configuration;

        public InternalUpdateSejamStatusHandler(IConfiguration configuration, IUserProfileRepository userProfileRepository)
        {
            _configuration = configuration;
            _userProfileRepository = userProfileRepository;
        }
        public async Task<BaseMessageResponse<InternalUpdateSejamStatusResponse>> Handle(InternalUpdateSejamStatusRequest request, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<InternalUpdateSejamStatusResponse>(EResponseStatusCode.DoneSuccessfully, new());

            var findUser = await _userProfileRepository.FindByUserId(request.UserId, cancellationToken);

            if (findUser is null)
                return MessageResponse.GenerateMessageResponse<InternalUpdateSejamStatusResponse>(EResponseStatusCode.ItemIsNotFound);

            await _userProfileRepository.UpdateSejamStatus(findUser.UserId, request.SejamStatus, cancellationToken);
            await _userProfileRepository.SaveChangeAsync(cancellationToken);

            return response;
        }
    }
}

public record InternalUpdateSejamStatusRequest([Required] int UserId, bool? SejamStatus) : BaseCommand<BaseMessageResponse<InternalUpdateSejamStatusResponse>>;

public class InternalUpdateSejamStatusResponse
{
}