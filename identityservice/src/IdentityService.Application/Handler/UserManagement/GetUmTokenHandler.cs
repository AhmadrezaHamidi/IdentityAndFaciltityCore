
using IdentityService.Domain.Entity;
using IdentityService.Domain.Entity.Person;
using IdentityService.Domain.IRepositories;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.JwtHelper;


namespace IdentityService.Application.Handler.UserManagement;

public class GetUmTokenHandler : IBaseHandler<UMTokenRequest, BaseMessageResponse<UMTokenResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly int _accessTokenExpierTime;
    private readonly IJwtService _jwtService;
    private readonly bool _isDevelopment;
    private readonly string _tokenDevelopment;
    private IUserRepository _userRepository;

    public GetUmTokenHandler(IJwtService jwtService, IConfiguration configuration, IPersonRepository personRepository, IUserRepository userRepository)
    {
        _accessTokenExpierTime = int.Parse(configuration["AccessTokenExpierTime"]);
        _jwtService = jwtService;
        _isDevelopment = bool.Parse(configuration["IsDevelopment"]);
        _tokenDevelopment = configuration["TokenDevelopment"];
        _personRepository = personRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseMessageResponse<UMTokenResponse>> Handle(UMTokenRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<UMTokenResponse>(EResponseStatusCode.DoneSuccessfully, new());
        var getPerson = await _personRepository.GetByNationalCodeAsync(request.NationalCode, cancellationToken);
        var user = await _userRepository.GetByMobileNumberAsync(getPerson.MobileNumber, cancellationToken);
        if (!_isDevelopment)
        {
            response.Data.AccessToken = _jwtService.GenerateAsyncUserManagment(user.Id, _accessTokenExpierTime, request.NationalCode, getPerson.MobileNumber);         
        }
        else
        {
            response.Data.AccessToken = _tokenDevelopment;
        }

        return response;
    }
}



public record UMTokenRequest([Required] string NationalCode) : BaseCommand<BaseMessageResponse<UMTokenResponse>>;

public class UMTokenResponse
{
    public string AccessToken { get; set; }
}