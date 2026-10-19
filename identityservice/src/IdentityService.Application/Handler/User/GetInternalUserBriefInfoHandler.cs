using IdentityService.Domain.Entity.Profile.UserAddress;
using IdentityService.Domain.Entity.Profile.UserPorfile;
using IdentityService.Domain.IRepositories;

namespace IdentityService.Application.Handler.User;

public class GetInternalUserBriefInfoHandler : IBaseHandler<GetInternalUserBriefInfoRequest, BaseMessageResponse<GetInternalUserBriefInfoResponse>>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserRepository _userRepository;

    public GetInternalUserBriefInfoHandler(IUserProfileRepository userProfileRepository, IUserRepository userRepository)
    {
        _userProfileRepository = userProfileRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseMessageResponse<GetInternalUserBriefInfoResponse?>> Handle(GetInternalUserBriefInfoRequest request,
        CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<GetInternalUserBriefInfoResponse>(EResponseStatusCode.DoneSuccessfully, new());

        var userProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(request.UserId, cancellationToken);
        var user = await _userRepository.FindById(request.UserId, cancellationToken);


        if (user is null)
            return MessageResponse.GenerateMessageResponse<GetInternalUserBriefInfoResponse>(EResponseStatusCode.ItemIsNotFound);

        response.Data = new GetInternalUserBriefInfoResponse() { };

        if (userProfile is not null)
        {
            response.Data.RealFirstName = userProfile?.RealFirstName;
            response.Data.RealLastName = userProfile?.RealLastName;
            response.Data.FatherName = userProfile?.FatherName;
            response.Data.SexId = userProfile?.SexId;
            response.Data.DeathStatus = userProfile?.DeathStatus;
        }

        response.Data.BirthDate = (user.Person != null) ? user.Person.BirthDate : null;
        response.Data.NationalCode = (user.Person != null) ? user.Person.NationalCode : null;
        response.Data.BankCustomerNumber = user?.BankCustomerNumber;
        response.Data.MobileNumber = user?.Person?.MobileNumber;
        response.Data.NationalCardSeries = user?.Person?.NationalCardSeries;


        return response;
    }
}

public record GetInternalUserBriefInfoRequest([Required] int UserId)
    : BaseCommand<BaseMessageResponse<GetInternalUserBriefInfoResponse>>;

public class GetInternalUserBriefInfoResponse
{
    public long? BankCustomerNumber { get; set; }
    public string? NationalCode { get; set; }
    public string? MobileNumber { get; set; }
    public string? RealFirstName { get; set; }
    public string? RealLastName { get; set; }
    public string? FatherName { get; set; }
    public string? SexId { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool? DeathStatus { get; set; }
    public string NationalCardSeries { get; set; }
}