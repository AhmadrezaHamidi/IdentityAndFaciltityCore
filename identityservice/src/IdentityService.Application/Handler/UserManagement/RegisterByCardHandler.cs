using Microsoft.Extensions.Configuration;
using OmidBank.Common.Cache;
using OmidBank.Common.ExternalThirdParty.Tosan.Signup;
using OmidBank.Common.ExternalThirdParty.Tosan.Signup.Request;
using OmidBank.Common.ExternalThirdParty.Tosan.User;

namespace IdentityService.Application.Handler.UserManagement;

public class RegisterByCardHandler : IBaseHandler<RegisterByCardRequest, BaseMessageResponse<RegisterByCardResponse>>
{
    private readonly bool _isDevelopTosan;
    private readonly string _baseUrl;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly TosanSignupProvider _tosanSignUpProvider;


    public RegisterByCardHandler(IConfiguration configuration, TosanSignupProvider tosanSignUpProvider)
    {
        _isDevelopTosan = bool.Parse(configuration["IsDevelopTosan"] ?? "false");
        _baseUrl = configuration["BaseUrlTosan"];
        _tosanSignUpProvider = tosanSignUpProvider;
    }

    public async Task<BaseMessageResponse<RegisterByCardResponse>> Handle(RegisterByCardRequest request,
        CancellationToken cancellationToken)
    {
        var response =
            MessageResponse.GenerateMessageResponse<RegisterByCardResponse>(EResponseStatusCode.DoneSuccessfully,
                null);
        if (_isDevelopTosan) return response;

        var sessionId = _redisCacheManager.Read(string.Format(CacheKey.TosanLoginTemporaryUserSessionId, request.RequestInfo.Token))!.ToString();

        if (string.IsNullOrEmpty(sessionId))
        {
            response = MessageResponse.GenerateMessageResponse<RegisterByCardResponse>(EResponseStatusCode.UnAuthorized);
            return response;
        }

        var result = await _tosanSignUpProvider.SignUpWithoutAuthorizationAsync(new(sessionId)
        {
         
            Username = request.Username,
            Password = ***;

        if (result.Exception is not null)
        {
            response = MessageResponse.GenerateMessageResponse<RegisterByCardResponse>(EResponseStatusCode.CallErrorTosan);
            response.Message = result.Exception.TranslateMessage;
            return response;
        }

        return response;
    }
}

public record RegisterByCardRequest(string Username, string Password, string Cif)
    : BaseCommand<BaseMessageResponse<RegisterByCardResponse>>;

public class RegisterByCardResponse
{
}