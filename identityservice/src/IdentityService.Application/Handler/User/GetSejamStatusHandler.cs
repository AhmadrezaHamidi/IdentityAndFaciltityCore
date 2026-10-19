using IdentityService.Domain.Entity.Profile.UserPorfile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.InternalThreadParty.IdentityService.Response;
using OmidBank.Common.InternalThreadParty.InvestmentService;
using System.Text.Json;


namespace IdentityService.Application.Handler.User
{
    public class GetSejamStatusHandler : IBaseHandler<GetSejamStatusRequest, BaseMessageResponse<GetSejamStatusResponse>>
    {
        private readonly IConfiguration _configuration;
        private readonly IRequestHelper _requestHelper;
        private readonly ILogger<GetSejamStatusHandler> _logger;
        private readonly IUserProfileRepository _userProfileRepository;


        public GetSejamStatusHandler(IConfiguration configuration, IRequestHelper requestHelper,
            ILogger<GetSejamStatusHandler> logger, IUserProfileRepository userProfileRepository)
        {
            _configuration = configuration;
            _requestHelper = requestHelper;
            _logger = logger;
            _userProfileRepository = userProfileRepository;

        }

        public async Task<BaseMessageResponse<GetSejamStatusResponse>> Handle(GetSejamStatusRequest request, CancellationToken cancellationToken)
        {

            var response = MessageResponse.GenerateMessageResponse<GetSejamStatusResponse>(EResponseStatusCode.DoneSuccessfully, null);
            var userProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(request.CurrentUser.Id, cancellationToken);
            var result = new GetSejamStatusResponse();
            if (userProfile == null || userProfile.SejamStatus == false || userProfile.SejamStatus == null)
            {
                result.SejamStatus = false;
                response.Data = result;
            }
            else
            {
                result.SejamStatus = true;
                response.Data = result;
            }
            return response;
        }
    }
}


public record GetSejamStatusRequest() : BaseCommand<BaseMessageResponse<GetSejamStatusResponse>>;

public class GetSejamStatusResponse
{
    public bool SejamStatus { get; set; }
}