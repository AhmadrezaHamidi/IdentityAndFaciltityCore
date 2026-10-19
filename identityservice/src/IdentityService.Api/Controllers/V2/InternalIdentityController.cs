using IdentityService.Application.Handler.User;
using IdentityService.Services.ViewModels.Request;

namespace IdentityService.Api.Controllers.V2
{
    [ApiVersion("2")]
    public class InternalIdentityController : BaseIdentityApplicationApi
    {
        private readonly IActiveTokenRepository _activeTokenRepository;
        private readonly IUserRepository _userRepository;

        public InternalIdentityController(IActiveTokenRepository activeTokenRepository, IUserRepository userRepository)
        {
            _activeTokenRepository = activeTokenRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        [InternalApi]
        public async Task<IActionResult> Authenticate([FromBody] UserAuthenticationRequest request, CancellationToken cancellationToken)
        {
            var response = MessageResponse.GenerateMessageResponse<UserModel>(EResponseStatusCode.DoneSuccessfully);
            var checkActiveToken = new ActiveTokenModel(request.ActiveToken, request.MobileNumber);

            var status = await _activeTokenRepository.CheckTokenAsync(checkActiveToken, cancellationToken);

            if (request.UserWithNewMobileNumber.HasValue && request.UserWithNewMobileNumber.Value && (status == null || !(bool)status))
            {
                var checkActiveTokenBeforeLogin = new ActiveTokenModelBeforeLogin(request.ActiveToken, request.MobileNumber);
                var token = await _activeTokenRepository.RetrieveTokenBeforeLoginAsync(checkActiveTokenBeforeLogin, cancellationToken);
                if (token is not null)
                {
                    status = true;
                    request.MobileNumber = token.OldMobileNumber;
                }
            }
            if (status == null || !(bool)status)
                return BaseResult(MessageResponse.GenerateMessageResponse<UserModel>(EResponseStatusCode.UnAuthorized));

            var user = await _userRepository.GetByMobileNumberAsync(request.MobileNumber, cancellationToken);
            if (!user.IsActive)
                return BaseResult(MessageResponse.GenerateMessageResponse<UserModel>(EResponseStatusCode.UnAuthorized));

            //var userIsActive = await _userRepository.IsActive(user.Id, cancellationToken);
            //if (userIsActive)
            //    return BaseResult(MessageResponse.GenerateMessageResponse<UserModel>(EResponseStatusCode.UnAuthorized));

            response.Data = user;
            return BaseResult(response);
        }

        [HttpPost]
        [InternalApi]
        public async Task<IActionResult> ChangeMobileNumber([FromBody] ChangeMobileNumberRequest request, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(request, cancellationToken));

        [HttpPost]
        [InternalApi]
        public async Task<IActionResult> ReverseChangeMobileNumber([FromBody] ReverseChangeMobileNumberRequest request, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(request, cancellationToken));

        [HttpPost]
        [InternalApi]
        public async Task<IActionResult> CheckChangeMobileNumberPayment([FromBody] CheckChangeMobileNumberPaymentRequest request, CancellationToken cancellationToken)
            => BaseResult(await _mediator.Send(request, cancellationToken));
    }
}