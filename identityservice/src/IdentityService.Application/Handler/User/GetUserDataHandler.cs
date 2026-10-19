using IdentityService.Domain.Entity.Basic.BankDataCity;
using IdentityService.Domain.Entity.Profile.UserAddress;
using IdentityService.Domain.Entity.Profile.UserPorfile;
using IdentityService.Domain.IRepositories;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.InternalThirdParty.BankService.Request;
using OmidBank.Common.InternalThreadParty.BankService;

namespace IdentityService.Application.Handler.User;

public class GetUserDataHandler : IBaseHandler<GetUserInfoRequest,
    BaseMessageResponse<GetUserInfoResponse>>
{
    private readonly IUserAddressRepository _userAddressRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBankDataCityRepository _bankDataCityRepository;
    private BankServiceProvider _bankServiceProvider;
    private readonly IConfiguration _configuration;



    public GetUserDataHandler(IUserAddressRepository userAddressRepository,
        IUserProfileRepository userProfileRepository,
        IUserRepository userRepository, IBankDataCityRepository bankDataCityRepository, IConfiguration configuration)
    {

        _userAddressRepository = userAddressRepository;
        _userProfileRepository = userProfileRepository;
        _userRepository = userRepository;
        _bankDataCityRepository = bankDataCityRepository;
        _bankServiceProvider = new BankServiceProvider(configuration["BankServiceUrl"], configuration["ApiKeyInternal"]);
    }

    public async Task<BaseMessageResponse<GetUserInfoResponse>> Handle(GetUserInfoRequest request,
        CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<GetUserInfoResponse>(EResponseStatusCode.DoneSuccessfully, new());
        response.Data = new GetUserInfoResponse();

        var userProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(request.CurrentUser.Id, cancellationToken);
        var userAddress = await _userAddressRepository.GetUserAddressByUserIdAsync(request.CurrentUser.Id, cancellationToken);

        var getBankCustomerInfoRequest = new BankCustomerInfoRequest()
        {
            UserId = request.CurrentUser.Id,
        };
        var bankCustomerInfo = await _bankServiceProvider.BankCustomerInfo(getBankCustomerInfoRequest, cancellationToken);

        var findUserProfile = await _userProfileRepository.FindByUserId(request.CurrentUser.Id, cancellationToken);

        if (findUserProfile is not null && findUserProfile.InquiryLastDate is not null && (DateTime.Now - findUserProfile.InquiryLastDate.Value).TotalMinutes > 360)
            return MessageResponse.GenerateMessageResponse<GetUserInfoResponse>(EResponseStatusCode.ValidateInquiryLastDateSabtAhval);


        var user = await _userRepository.FindById(request.CurrentUser.Id, cancellationToken);
        if (userAddress != null)
        {
            response.Data.UserAddress = new UserAddress()
            {
                PostalCode = userAddress.PostalCode,
                ProvinceName = userAddress.ProvinceName,
                CityName = userAddress.CityName,
                TownShipName = userAddress.TownShipName,
                ZoneName = userAddress.ZoneName,
                VillageName = userAddress.VillageName,
                SubLocalityName = userAddress.SubLocalityName,
                Street = userAddress.Street,
                HouseNumber = userAddress.HouseNumber,
                Floor = userAddress.Floor,
                SideFloor = userAddress.SideFloor,
                BuildingName = userAddress.BuildingName,
                Description = userAddress.Description,
                PhoneNumber = userAddress.PhoneNumber,
                Unit = userAddress.Unit,
            };
        }

        if (userProfile != null)
        {
            var birthProvienceId = await _bankDataCityRepository.GetProvienceIdAsync(userProfile.BirthLocationId, cancellationToken);
            var registerProvienceId = await _bankDataCityRepository.GetProvienceIdAsync(userProfile.RegisterLocationId, cancellationToken);

            response.Data.UserProfile = new UserProfile()
            {
                BirthProvienceId = birthProvienceId,
                RegisterProvienceId = registerProvienceId,
                RealFirstName = userProfile.RealFirstName,
                RealLastName = userProfile.RealLastName,
                FatherName = userProfile.FatherName,
                MotherName = userProfile.MotherName,
                CustomerGroupId = userProfile.CustomerGroupId,
                EducationId = userProfile.EducationId,
                ResidentialStatusId = userProfile.ResidentialStatusId,
                SurnameId = userProfile.SurnameId,
                SexId = userProfile.SexId,
                RegisterLocationId = userProfile.RegisterLocationId,
                BirthLocationId = userProfile.BirthLocationId,
                CountryId = userProfile.CountryId,
                BirthCountryId = userProfile.BirthCountryId,
                MarriageId = userProfile.MarriageId != null ? userProfile.MarriageId == "MARRIED" ? "2" : "1" : null,
                BirthDate = user.Person.BirthDate,
                BirthCertificateNo = userProfile.BirthCertificateNo,
                BirthCertificateSerialNo = userProfile.BirthCertificateSerialNo,
                IdentityCardNumber = userProfile.IdentityCardNumber,
                IdentityCardSeries = userProfile.IdentityCardSeries,
                IdentityCardSerial = userProfile.IdentityCardSerial,
                JobTypeId = userProfile.JobTypeId,
                ReligionId = userProfile.ReligionId,
                DeathStatus = userProfile.DeathStatus,
                CityCode = userProfile.CityCode,
                StateCode = userProfile.StateCode,
                BirthCityCode = userProfile.BirthCityCode,
                BirthStateCode = userProfile.BirthStateCode,
                RegisterCityCode = userProfile.RegisterCityCode,
                RegisterStateCode = userProfile.RegisterStateCode,
                FatherNameEn = userProfile.FatherNameEn,
                RealFirstNameEn = userProfile.RealFirstNameEn,
                RealLastNameEn = userProfile.RealLastNameEn,
                BranchCityCode = bankCustomerInfo?.Data?.BranchCityCode,
                BranchStateCode = bankCustomerInfo?.Data?.BranchStateCode,
                BranchCode = bankCustomerInfo?.Data?.BranchCode,
            };
        }

        if (bankCustomerInfo != null && bankCustomerInfo.Data != null)
        {
            response.Data.BranchCode = bankCustomerInfo.Data.BranchCode;
            response.Data.BranchCityCode = bankCustomerInfo.Data.BranchCityCode;
            response.Data.BranchStateCode = bankCustomerInfo.Data.BranchStateCode;
            response.Data.PaidWage = bankCustomerInfo.Data.PaidWage != null ? bankCustomerInfo.Data.PaidWage.Value : false;
        }

        if (user != null)
        {
            response.Data.BankCustomerNumber = user.BankCustomerNumber;
        }

        return response;
    }
}

public record GetUserInfoRequest() : BaseCommand<BaseMessageResponse<GetUserInfoResponse>>;

public class GetUserInfoResponse
{
    public UserAddress UserAddress { get; set; }
    public UserProfile UserProfile { get; set; }
    public string BranchCityCode { get; set; }
    public string BranchStateCode { get; set; }
    public string BranchCode { get; set; }
    public bool? PaidWage { get; set; }
    public long? BankCustomerNumber { get; set; }
}