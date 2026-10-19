using IdentityService.Domain.Entity.Profile.UserAddress;
using IdentityService.Domain.Entity.Profile.UserPorfile;
using IdentityService.Domain.IRepositories;

namespace IdentityService.Application.Handler.User;

public class GetInternalUserDataHandler : IBaseHandler<GetInternalUserInfoRequest,
    BaseMessageResponse<GetInternalUserInfoResponse>>
{
    private readonly IUserAddressRepository _userAddressRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserRepository _userRepository;
    private IActiveTokenRepository _activeTokenRepository;

    public GetInternalUserDataHandler(IUserAddressRepository userAddressRepository,
        IUserProfileRepository userProfileRepository,
        IUserRepository userRepository, IActiveTokenRepository activeTokenRepository)
    {
        _userAddressRepository = userAddressRepository;
        _userProfileRepository = userProfileRepository;
        _userRepository = userRepository;
        _activeTokenRepository = activeTokenRepository;
    }

    public async Task<BaseMessageResponse<GetInternalUserInfoResponse>> Handle(GetInternalUserInfoRequest request,
        CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<GetInternalUserInfoResponse>(EResponseStatusCode.DoneSuccessfully, new());

        var userProfile = await _userProfileRepository.GetUserProfileByUserIdAsync(request.UserId, cancellationToken);
        var userAddress = await _userAddressRepository.GetUserAddressByUserIdAsync(request.UserId, cancellationToken);
        var user = await _userRepository.FindById(request.UserId, cancellationToken);


        if (user is null)
            return MessageResponse.GenerateMessageResponse<GetInternalUserInfoResponse>(EResponseStatusCode.ItemIsNotFound);

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
                Unit = userAddress.Unit
            };
        }

        if (userProfile != null)
        {
            response.Data.UserProfile.RealFirstName = userProfile.RealFirstName;
            response.Data.UserProfile.RealLastName = userProfile.RealLastName;
            response.Data.UserProfile.FatherName = userProfile.FatherName;
            response.Data.UserProfile.MotherName = userProfile.MotherName;
            response.Data.UserProfile.CustomerGroupId = userProfile.CustomerGroupId;
            response.Data.UserProfile.EducationId = userProfile.EducationId;
            response.Data.UserProfile.ResidentialStatusId = userProfile.ResidentialStatusId;
            response.Data.UserProfile.SurnameId = userProfile.SurnameId;
            response.Data.UserProfile.SexId = userProfile.SexId;
            response.Data.UserProfile.RegisterLocationId = userProfile.RegisterLocationId;
            response.Data.UserProfile.BirthLocationId = userProfile.BirthLocationId;
            response.Data.UserProfile.CountryId = userProfile.CountryId;
            response.Data.UserProfile.BirthCountryId = userProfile.BirthCountryId;
            response.Data.UserProfile.MarriageId = userProfile.MarriageId;
            response.Data.UserProfile.BirthCertificateNo = userProfile.BirthCertificateNo;
            response.Data.UserProfile.BirthCertificateSerialNo = userProfile.BirthCertificateSerialNo;
            response.Data.UserProfile.IdentityCardNumber = userProfile.IdentityCardNumber;
            response.Data.UserProfile.IdentityCardSeries = userProfile.IdentityCardSeries;
            response.Data.UserProfile.IdentityCardSerial = userProfile.IdentityCardSerial;
            response.Data.UserProfile.ReligionId = userProfile.ReligionId;
            response.Data.UserProfile.JobTypeId = userProfile.JobTypeId;
            response.Data.UserProfile.DeathStatus = userProfile.DeathStatus;

            response.Data.UserProfile.RealFirstNameEn = userProfile.RealFirstNameEn;
            response.Data.UserProfile.RealLastNameEn = userProfile.RealLastNameEn;
            response.Data.UserProfile.FatherNameEn = userProfile.FatherNameEn;
            response.Data.UserProfile.CityCode = userProfile.CityCode;
            response.Data.UserProfile.StateCode = userProfile.StateCode;
            response.Data.UserProfile.BirthStateCode = userProfile.BirthStateCode;
            response.Data.UserProfile.BirthCityCode = userProfile.BirthCityCode;
            response.Data.UserProfile.RegisterCityCode = userProfile.RegisterCityCode;
            response.Data.UserProfile.RegisterStateCode = userProfile.RegisterStateCode;
        }

        response.Data.UserProfile.BirthDate = (user.Person != null) ? user.Person.BirthDate : null;
        response.Data.UserProfile.NationalCardSeries = (user.Person.NationalCardSeries != null) ? user.Person.NationalCardSeries : null;
        response.Data.BankCustomerNumber = user.BankCustomerNumber;
        response.Data.MobileNumber = user.Person.MobileNumber;
        response.Data.NationalCardSeries = user.Person.NationalCardSeries;
        response.Data.NationalCode= user.Person.NationalCode;
        response.Data.ActiveToken = await _activeTokenRepository.GetActiveTokenByMobileNumberAsync(user.Person.MobileNumber, cancellationToken);


        return response;
    }
}

public record GetInternalUserInfoRequest([Required] int UserId) : BaseCommand<BaseMessageResponse<GetInternalUserInfoResponse>>;

public class GetInternalUserInfoResponse
{
    public UserAddress UserAddress { get; set; } = new();
    public UserProfile UserProfile { get; set; } = new();
    public long? BankCustomerNumber { get; set; }
    public string MobileNumber { get; set; }
    public string ActiveToken { get; set; }
    public string NationalCardSeries { get; set; }
    public string NationalCode { get; set; }
}

public class UserAddress
{
    public string PostalCode { get; set; }
    public string ProvinceName { get; set; }
    public string CityName { get; set; }
    public string TownShipName { get; set; }
    public string ZoneName { get; set; }
    public string VillageName { get; set; }
    public string SubLocalityName { get; set; }
    public string Street { get; set; }
    public double? HouseNumber { get; set; }
    public string Floor { get; set; }
    public string SideFloor { get; set; }
    public string BuildingName { get; set; }
    public string Description { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Unit { get; set; }
}

public class UserProfile
{
    public string RealFirstName { get; set; }
    public string RealLastName { get; set; }
    public string FatherName { get; set; }
    public string MotherName { get; set; }
    public string CustomerGroupId { get; set; }
    public string EducationId { get; set; }
    public string ResidentialStatusId { get; set; }
    public string SurnameId { get; set; }
    public string SexId { get; set; }
    public string RegisterLocationId { get; set; }
    public string BirthLocationId { get; set; }
    public string CountryId { get; set; }
    public string BirthCountryId { get; set; }
    public string MarriageId { get; set; }
    public DateTime? BirthDate { get; set; }
    public string BirthCertificateNo { get; set; }
    public string BirthCertificateSerialNo { get; set; }
    public string IdentityCardNumber { get; set; }
    public string IdentityCardSeries { get; set; }
    public string IdentityCardSerial { get; set; }
    public string JobTypeId { get; set; }
    public string ReligionId { get; set; }
    public bool? DeathStatus { get; set; }
    public string BirthProvienceId { get; set; }
    public string RegisterProvienceId { get; set; }
    public string? RealFirstNameEn { get; set; }
    public string? RealLastNameEn { get; set; }
    public string? FatherNameEn { get; set; }
    public string? CityCode { get; set; }
    public string? StateCode { get; set; }
    public string? BirthCityCode { get; set; }
    public string? BirthStateCode { get; set; }
    public string? RegisterCityCode { get; set; }
    public string? RegisterStateCode { get; set; }
    public string? NationalCardSeries { get; set; }
    public string? BranchCityCode { get; set; }
    public string? BranchStateCode { get; set; }
    public string? BranchCode { get; set; }
}
