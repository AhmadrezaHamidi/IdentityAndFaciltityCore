using System.Text.Json;
using Amazon.Runtime.Internal.Util;
using IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmidBank.Common.Cryptography;
using OmidBank.Common.Extentions;
using OmidBank.Common.JwtHelper;

namespace IdentityService.Application.Handler.UserManagement;

public class InternalCheckUMTokenHandler : IBaseHandler<InternalCheckUMTokenRequest, BaseMessageResponse<InternalCheckUMTokenResponse>>
{
    private readonly IJwtService _jwtService;
    private readonly IUserManagementTokenLogRepository _userManagementTokenLogRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InternalCheckUMTokenHandler> _logger;

    public InternalCheckUMTokenHandler(IJwtService jwtService, IUserManagementTokenLogRepository userManagementTokenLogRepository, IConfiguration configuration, ILogger<InternalCheckUMTokenHandler> logger)
    {
        _jwtService = jwtService;
        _userManagementTokenLogRepository = userManagementTokenLogRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<BaseMessageResponse<InternalCheckUMTokenResponse>> Handle(InternalCheckUMTokenRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<InternalCheckUMTokenResponse>(EResponseStatusCode.DoneSuccessfully, new());

        var findToken = await _userManagementTokenLogRepository.FindByUserIdAndTokenCheckIsActiveAsync(request.UserId,request.UMToken, cancellationToken);

        if (findToken is null || findToken.IsNullOrDefault() || findToken?.User is null ||
            findToken?.User?.BankCustomerNumber is null)
            return MessageResponse.GenerateMessageResponse<InternalCheckUMTokenResponse>(EResponseStatusCode.UMUnAuthorized);

        if (!bool.Parse(_configuration["IsDevelopment"]))
        {
            (bool valid, string userid, BaseCurrentUser models, int c) = _jwtService.ValidateTokenUserManagment(request.UMToken);

            if (!valid)
                return MessageResponse.GenerateMessageResponse<InternalCheckUMTokenResponse>(EResponseStatusCode.UMUnAuthorized);


            if (findToken?.UserId != Convert.ToInt32(models?.UserId))
                return MessageResponse.GenerateMessageResponse<InternalCheckUMTokenResponse>(EResponseStatusCode.UMUnAuthorized);
        }


        return response;
    }
}

public record InternalCheckUMTokenRequest([Required] int UserId, [Required] string UMToken) : BaseCommand<BaseMessageResponse<InternalCheckUMTokenResponse>>;

public class InternalCheckUMTokenResponse
{
}