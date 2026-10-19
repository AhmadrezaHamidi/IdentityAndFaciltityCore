using IdentityService.Domain.Entity.Profile.UserPorfile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdentityService.Application.Handler.User
{
    public class AddUserProfileInfoHandler : IBaseHandler<AddUserProfileInfoRequest, BaseMessageResponse<AddUserProfileInfoResponse>>
    {
        private readonly IConfiguration _configuration;
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<AddUserProfileInfoHandler> _logger;
        private readonly IUserProfileRepository _userProfileRepository;


        public AddUserProfileInfoHandler(IConfiguration configuration, IRequestHelper requestHelper,
            ILogger<AddUserProfileInfoHandler> logger, IUserProfileRepository userProfileRepository)
        {
            _configuration = configuration;
            _requestHelper = requestHelper;
            _logger = logger;
            _userProfileRepository = userProfileRepository;

        }

        public async Task<BaseMessageResponse<AddUserProfileInfoResponse>> Handle(AddUserProfileInfoRequest request, CancellationToken cancellationToken)
        {

            var response = MessageResponse.GenerateMessageResponse<AddUserProfileInfoResponse>(EResponseStatusCode.DoneSuccessfully, null);


            var userProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(request.UserId, cancellationToken);
            if (userProfile is not null)
                return MessageResponse.GenerateMessageResponse<AddUserProfileInfoResponse>(EResponseStatusCode.InternalServerError);

            var userProfileInfo = new UserProfileEntity()
            {
                UserId = request.UserId,
                SejamStatus = request.SejamStatus
            };
            await _userProfileRepository.AddAsync(userProfileInfo, cancellationToken);
            await _userProfileRepository.SaveChangeAsync(cancellationToken);

            return response;
        }
    }
}
public record AddUserProfileInfoRequest() : BaseCommand<BaseMessageResponse<AddUserProfileInfoResponse>>
{
    public int UserId { get; set; }
    public bool SejamStatus { get; set; }
}

public class AddUserProfileInfoResponse
{
}