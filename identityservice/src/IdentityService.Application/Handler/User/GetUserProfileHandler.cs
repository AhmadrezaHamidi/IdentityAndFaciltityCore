using IdentityService.Application.Handler.UserManagement;
using IdentityService.Domain.Entity.Profile.UserPorfile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Handler.User
{
    public class GetUserProfileHandler : IBaseHandler<InternalUserProfileInfoRequest, BaseMessageResponse<GetUserProfileResponse>>
    {
        private readonly IConfiguration _configuration;
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<GetSejamStatusHandler> _logger;
        private readonly IUserProfileRepository _userProfileRepository;
        public GetUserProfileHandler(IConfiguration configuration, IRequestHelper requestHelper,
            ILogger<GetSejamStatusHandler> logger, IUserProfileRepository userProfileRepository)
        {
            _configuration = configuration;
            _requestHelper = requestHelper;
            _logger = logger;
            _userProfileRepository = userProfileRepository;

        }
        public async Task<BaseMessageResponse<GetUserProfileResponse>> Handle(InternalUserProfileInfoRequest request, CancellationToken cancellationToken)
        {

            var response = MessageResponse.GenerateMessageResponse<GetUserProfileResponse>(EResponseStatusCode.DoneSuccessfully, new());
            var userProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(request.UserId, cancellationToken);
            if (userProfile is not null)
            {
                response.Data.UserId = request.UserId;
                return response;
            }
            else
                return MessageResponse.GenerateMessageResponse<GetUserProfileResponse>(EResponseStatusCode.InternalServerError);
        }
    }
}
public record InternalUserProfileInfoRequest(
        [Required] int UserId
    ) : BaseCommand<BaseMessageResponse<GetUserProfileResponse>>;

public class GetUserProfileResponse
{
    public int UserId { get; set; }
}
