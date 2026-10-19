using IdentityService.Domain.Entity;
using OmidBank.Common.Cache;
using OmidBank.Common.ExternalThirdParty.Tosan;
using OmidBank.Common.ExternalThirdParty.Tosan.Global;
using OmidBank.Common.ExternalThirdParty.Tosan.Other;
using OmidBank.Common.ExternalThirdParty.Tosan.Signup;
using OmidBank.Common.ExternalThirdParty.Tosan.Signup.Request;

namespace IdentityService.Application.Handler.UserManagement;

public class RegisterUMUserHandler : IBaseHandler<RegisterUMUserRequest, BaseMessageResponse<RegisterUMUserResponse>>
{
    private readonly IUserManagementRepository _userManagementRepository;
    private TosanSignupProvider _tosanSignupProvider;
    private TosanGlobalProvider _tosanGlobalProvider;
    private readonly IRedisCacheManager _redisCacheManager;

    public RegisterUMUserHandler(TosanSignupProvider tosanSignupProvider, TosanGlobalProvider tosanGlobalProvider, IRedisCacheManager redisCacheManager, IUserManagementRepository userManagementRepository)
    {
        _tosanSignupProvider = tosanSignupProvider;
        _tosanGlobalProvider = tosanGlobalProvider;
        _redisCacheManager = redisCacheManager;
        _userManagementRepository = userManagementRepository;
    }

    public async Task<BaseMessageResponse<RegisterUMUserResponse>> Handle(RegisterUMUserRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<RegisterUMUserResponse>(EResponseStatusCode.DoneSuccessfully, null);

        var findUm = await _userManagementRepository.GetByUserIdAsync(request.CurrentUser!.Id, cancellationToken);
        if (findUm is null)
            return MessageResponse.GenerateMessageResponse<RegisterUMUserResponse>(EResponseStatusCode.ItemIsNotFound)!;

        var otpCode = _redisCacheManager.Read<object>(string.Format(CacheKey.OtpRegisterUM, request.RequestInfo.Token));
        if (otpCode is null || otpCode.ToString() != request.OtpCode)
            return MessageResponse.GenerateMessageResponse<RegisterUMUserResponse>(EResponseStatusCode.InvalidOtpCode)!;


        var sessionIdThirdParty = _redisCacheManager.Read<object>(string.Format(CacheKey.TosanSystemSessionId));
        if (sessionIdThirdParty is null)
            return MessageResponse.GenerateMessageResponse<RegisterUMUserResponse>(EResponseStatusCode.SystemTokenNotFound)!;

        var registerResponse = await _tosanSignupProvider.SignUpWithoutAuthorizationAsync(new SignUpWithoutAuthorizationRequest(sessionIdThirdParty.ToString())
        {
            UseBaseUrlSystem = true,
            Cif = findUm.User.BankCustomerNumber.ToString(),
            Username = request.Username,
            Password = ***"117",
                Name = "YAGHUT-MOBILE-APPLICATION"
            }
        }, cancellationToken);


        if (registerResponse.Exception is not null)
        {
            response = MessageResponse.GenerateMessageResponse<RegisterUMUserResponse>(EResponseStatusCode.CallErrorTosan);
            response.Message = registerResponse.Exception.TranslateMessage;

            var errorCodes = new HashSet<TosanMessage>
            {
                TosanMessage.InvalidChannelWSException,
                TosanMessage.DuplicateEntityWSException,
                TosanMessage.InvalidUserWSException,
                TosanMessage.PasswordStrengthLevelWSException,
                TosanMessage.BlockUserWSException,
                TosanMessage.CustomerIsIncapableRuntimeWSException,
                TosanMessage.GroupIdIsInvalidRuntimeWSException
            };
            if (registerResponse.Exception.Code == TosanMessage.UserChannelAlreadyExistsWSException)
            {
                findUm.TosanRegister = true;
                await _userManagementRepository.SaveChangeAsync(cancellationToken);
                response = MessageResponse.GenerateMessageResponse<RegisterUMUserResponse>(EResponseStatusCode.UserAlreadyExists);
            }
            else if (errorCodes.Contains(registerResponse.Exception.Code))
            {
                response = MessageResponse.GenerateMessageResponse<RegisterUMUserResponse>(EResponseStatusCode.ErrorRegisterUm);
                response.Message = registerResponse.Exception.TranslateMessage;
            }
            else if (registerResponse.Exception.Code == TosanMessage.ValidatorWSException)
            {
                response = MessageResponse.GenerateMessageResponse<RegisterUMUserResponse>(EResponseStatusCode.BadRequest);
                response.Message = ExceptionTranslator.Translate(registerResponse.Exception.ExceptionMessage);
            }

            return response;
        }

        findUm.TosanRegister = true;
        await _userManagementRepository.SaveChangeAsync(cancellationToken);
        return response;
    }
}

public record RegisterUMUserRequest([Required] string Username, [Required] string Password, string OtpCode) : BaseCommand<BaseMessageResponse<RegisterUMUserResponse>>;

public class RegisterUMUserResponse
{
}

public class ExceptionTranslator
{
    private static readonly Dictionary<string, string> Translations = new Dictionary<string, string>
    {
        { "must not be empty", ".نام کاربری نباید خالی باشد" },
        { "The length of the parameter must be between 8 and 50", ".نام کاربری باید بین ۸ تا ۵۰ کاراکتر باشد" },
        { "AlphaNumericalUsername must be longer than or equal  : 5", "نام کاربری باید حداقل ۵ کاراکتر و ترکیبی از حروف و عدد باشد" },
        { "An alphaNumerical Username must be a combination of numbers and letters.", "نام کاربری باید ترکیبی از حروف و اعداد باشد" },
        // { "The value must match the following pattern: [0-9A-Za-z][0-9A-Za-z._-]*", "نام کاربری باید فقط شامل حروف، اعداد، نقطه، خط زیر یا خط تیره باشد" }
    };

    public static string Translate(string message)
    {
        foreach (var entry in Translations)
        {
            if (message.Contains(entry.Key))
                return entry.Value;
        }

        return "خطای ناشناخته‌ای رخ داده است.";
    }
}