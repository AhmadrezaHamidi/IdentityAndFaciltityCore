using IdentityService.Application.Handler.User.Shared;
using IdentityService.Domain.Entity.Profile.UserPorfile;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.InternalThirdParty.BankService.Request;
using OmidBank.Common.InternalThreadParty.BankService;

namespace IdentityService.Application.Handler.User;

public class ModifyUserProfileHandler : IBaseHandler<ModifyUserProfileRequest, BaseMessageResponse<ModifyUserProfileResponse>>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly BankServiceProvider _bankServiceProvider;


    public ModifyUserProfileHandler(IUserProfileRepository userProfileRepository, IConfiguration configuration)
    {
        _userProfileRepository = userProfileRepository;
        _bankServiceProvider = new BankServiceProvider(configuration["BankServiceUrl"], configuration["ApiKeyInternal"]);
    }

    public async Task<BaseMessageResponse<ModifyUserProfileResponse>> Handle(ModifyUserProfileRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<ModifyUserProfileResponse>(EResponseStatusCode.DoneSuccessfully, new());

        var findUserProfile = await _userProfileRepository.FindByUserId(request.CurrentUser.Id, cancellationToken);

        if (findUserProfile is null)
            return MessageResponse.GenerateMessageResponse<ModifyUserProfileResponse>(EResponseStatusCode.InquiryShouldFirst);


        findUserProfile.EducationId = request.EducationId;
        findUserProfile.JobTypeId = request.JobTypeId;
        findUserProfile.RegisterLocationId = request.RegisterLocationId;
        findUserProfile.BirthLocationId = request.BirthLocationId;
        findUserProfile.CountryId = request.CountryId ?? "0";
        findUserProfile.BirthCountryId = request.BirthCountryId;
        findUserProfile.MarriageId = request.MarriageId == Enums.MaritalStatus.Single ? "SINGLE" :
        request.MarriageId == Enums.MaritalStatus.Married ? "MARRIED" : "";
        findUserProfile.ReligionId = request.ReligionId;
        findUserProfile.CityCode = request.CityCode;
        findUserProfile.StateCode = request.StateCode;
        findUserProfile.BirthCityCode = request.BirthCityCode;
        findUserProfile.BirthStateCode = request.BirthStateCode;
        findUserProfile.RegisterCityCode = request.RegisterCityCode;
        findUserProfile.RegisterStateCode = request.RegisterStateCode;
        //findUserProfile.BranchCityCode = request.BranchCityCode;
        //findUserProfile.BranchStateCode = request.BranchStateCode;

        //call internal insert userAccount
        //var createAccountRequest = new InsertUserAccountViewModel()
        //{
        //    UserId = request.CurrentUser.Id,
        //    BranchId = int.Parse(request.BranchCode)
        //};

        //var resposneInserUserAccount=await _bankServiceProvider.AddUserAccount(createAccountRequest,cancellationToken);
        //if(resposneInserUserAccount==null || !resposneInserUserAccount.IsSuccess)
        //    return MessageResponse.GenerateMessageResponse<ModifyUserProfileResponse>(EResponseStatusCode.InternalServerError);

        await _userProfileRepository.UpdateAsync(findUserProfile, cancellationToken);
        await _userProfileRepository.SaveChangeAsync(cancellationToken);

        return response;
    }
}

public record ModifyUserProfileRequest(
    Enums.MaritalStatus MarriageId,
    [MaxLength(20)] string JobTypeId,
    [MaxLength(20)] string ReligionId,
    [MaxLength(20)] string? EducationId,
    [MaxLength(20)] string? RegisterLocationId,
    [MaxLength(20)] string? BirthLocationId,
    string? CountryId,
    [MaxLength(20)] string? BirthCountryId,
    [MaxLength(20)] string? CityCode,
    [MaxLength(20)] string? StateCode,
    [MaxLength(20)] string? BirthCityCode,
    [MaxLength(20)] string? BirthStateCode,
    [MaxLength(20)] string? RegisterCityCode,
    [MaxLength(20)] string? RegisterStateCode
    //[MaxLength(20)] string? BranchCityCode,
    //[MaxLength(20)] string? BranchStateCode
    //[MaxLength(20)] string? BranchCode


) : BaseCommand<BaseMessageResponse<ModifyUserProfileResponse>>;

public class ModifyUserProfileResponse : UserProfileResponseDto
{
}