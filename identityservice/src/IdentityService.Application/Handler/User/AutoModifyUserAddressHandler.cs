using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using IdentityService.Application.Handler.User.Shared;
using IdentityService.Domain.Entity.Profile.UserAddress;
using Microsoft.Extensions.Configuration;
using OmidBank.Common.InternalThreadParty.OmidTech.Request;
using OmidBank.Common.InternalThreadParty.OmidTech;
using OmidBank.Common.InternalThreadParty.OmidTech.Response;
using Amazon.Runtime.Internal.Util;
using OmidBank.Common.InternalThirdParty.PostalCodeService.Request;
using OmidBank.Common.InternalThirdParty.PostalCodeService;
using Microsoft.Extensions.Options;
using static OmidBank.Common.InternalThirdParty.PostalCodeService.PostalCodeServiceProvider;
using OmidBank.Common.Cache;
using System.Net.Http;

namespace IdentityService.Application.Handler.User;

public class AutoModifyUserAddressHandler : IBaseHandler<AutoModifyUserAddressRequest, BaseMessageResponse<AutoModifyUserAddressResponse>>
{
    private readonly IUserAddressRepository _userAddressRepository;
    private readonly IConfiguration _configuration;
    private readonly bool _isDevelopment;
    private readonly IRequestScopedLogger<AutoModifyUserAddressHandler> _logger;
    private readonly IOptions<PostalProviderOptions> _postalOptions;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly HttpClient _httpClient;

    public AutoModifyUserAddressHandler(IUserAddressRepository userAddressRepository, IConfiguration configuration,
        IRequestScopedLogger<AutoModifyUserAddressHandler> logger,IOptions<PostalProviderOptions> postalOptions, IRedisCacheManager redisCacheManager,
        HttpClient httpClient)
    {
        _configuration = configuration;
        _userAddressRepository = userAddressRepository;
        _isDevelopment = bool.Parse(configuration["IsDevelopment"]);
        _logger = logger;
        _postalOptions = postalOptions;
        _redisCacheManager = redisCacheManager;
        _httpClient = httpClient;
    }


    public async Task<BaseMessageResponse<AutoModifyUserAddressResponse>> Handle(AutoModifyUserAddressRequest request, CancellationToken cancellationToken)
    {
        var response = MessageResponse.GenerateMessageResponse<AutoModifyUserAddressResponse>(EResponseStatusCode.DoneSuccessfully, new());
        var omidTechProvider = new OmidTechProvider(_configuration["BaseOmidTechUrl"], _configuration["ApiKeyOmidTech"], _httpClient);

        var findUserAddress = await _userAddressRepository.FindByUserId(request.CurrentUser.Id, cancellationToken);

        if (findUserAddress is not null && findUserAddress.PostalCode == request.PostalCode && findUserAddress.InquiryLastDate is not null && (DateTime.Now - findUserAddress.InquiryLastDate.Value).TotalMinutes <= 360)
        {
            response.Data.ProvinceName = findUserAddress.ProvinceName;
            response.Data.CityName = findUserAddress.CityName;
            response.Data.Description = findUserAddress.Description;
            response.Data.HouseNumber = findUserAddress.HouseNumber;
            response.Data.Unit = findUserAddress.Unit;
            response.Data.PhoneNumber = findUserAddress.PhoneNumber;
            return response;
        }

        //if (_isDevelopment)
        //{
        //    var PostCodeInfos = new PostalCodeInquiryRequest
        //    {
        //        ClientBatchId = 31,
        //        PostCodes = new List<PostCodeInquiryRequestItem>()
        //        {
        //            new PostCodeInquiryRequestItem
        //            {
        //                PostCode = "1234567891", ClientRowId = 1
        //            },
        //        }
        //    };

        //    var des = JsonSerializer.Serialize<GetAddressByPostCodeRequest>(PostCodeInfos);
        //    getAddressResponse = await omidTechProvider.PostalCodeInquiry(PostCodeInfos, cancellationToken);
        //}
        //else
        //{

        var provider = new PostalCodeServiceProvider(_configuration,_postalOptions, _redisCacheManager, _httpClient);

        var resultPostalCode = await provider.PostalInquiry(new PostalCodeInquiryViewModel
        {
            PostalCode = request.PostalCode
        }, cancellationToken);

        if (resultPostalCode is null)
            return MessageResponse.GenerateMessageResponse<AutoModifyUserAddressResponse>(EResponseStatusCode.CallErroProvider);

        if (!resultPostalCode.Success)
            return MessageResponse.GenerateMessageResponse<AutoModifyUserAddressResponse>(EResponseStatusCode.InvalidPostalCode);

        var addressInfo = resultPostalCode.Data;
        if (findUserAddress is null)
        {
            findUserAddress = new UserAddressEntity()
            {
                UserId = request.CurrentUser.Id,
                PostalCode = request.PostalCode,
                ProvinceName = addressInfo.Province,
                CityName = addressInfo.TownShip,
                ZoneName = addressInfo.Zone,
                VillageName = addressInfo.Village,
                SubLocalityName = addressInfo.Locality,
                Street = addressInfo.Street2,
                HouseNumber = int.Parse(addressInfo.HouseNumber),
                Floor = addressInfo.Floor,
                SideFloor = addressInfo.SideFloor,
                BuildingName = addressInfo.BuildingName,
                Description = "",
                InquiryLastDate = DateTime.Now
            };
            await _userAddressRepository.AddAsync(findUserAddress, cancellationToken);
        }
        else
        {
            findUserAddress.PostalCode = request.PostalCode;
            findUserAddress.ProvinceName = addressInfo.Province;
            findUserAddress.CityName = addressInfo.TownShip;
            findUserAddress.ZoneName = addressInfo.Zone;
            findUserAddress.VillageName = addressInfo.Village;
            findUserAddress.SubLocalityName = addressInfo.Locality;
            findUserAddress.Street = addressInfo.Street2;
            findUserAddress.HouseNumber = int.Parse(addressInfo.HouseNumber);
            findUserAddress.Floor = addressInfo.Floor;
            findUserAddress.SideFloor = addressInfo.SideFloor;
            findUserAddress.BuildingName = addressInfo.BuildingName;
            findUserAddress.Description ="";
            findUserAddress.InquiryLastDate = DateTime.Now;
            await _userAddressRepository.UpdateAsync(findUserAddress, cancellationToken);
        }

        await _userAddressRepository.SaveChangeAsync(cancellationToken);

        response.Data.ProvinceName = addressInfo.Province;
        response.Data.CityName = addressInfo.TownShip;
        response.Data.Description ="";
        response.Data.HouseNumber = int.Parse(addressInfo.HouseNumber);
        response.Data.Unit = addressInfo.HouseNumber.ToString();
        response.Data.PhoneNumber = findUserAddress.PhoneNumber;

        return response;
    }
}

public record AutoModifyUserAddressRequest([Required] string PostalCode) : BaseCommand<BaseMessageResponse<AutoModifyUserAddressResponse>>;

public class AutoModifyUserAddressResponse : UserAddressResponseDto
{
    public string Description { get; set; }
    public double? HouseNumber { get; set; }
    public string Unit { get; set; }
    public string PhoneNumber { get; set; }
}