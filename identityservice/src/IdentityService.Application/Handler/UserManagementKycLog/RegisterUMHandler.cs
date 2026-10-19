using IdentityService.Application.Services.Kyc.Common;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Factories;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Application.Handler.UserManagementKycLog;

public class RegisterUMHandler(IKycRegisterUmServiceFactory umServiceFactory, IConfiguration configuration) : IBaseHandler<RegisterUMRequest, BaseMessageResponse<RegisterUMResponse>>
{
    public async Task<BaseMessageResponse<RegisterUMResponse>> Handle(RegisterUMRequest request, CancellationToken cancellationToken)
    {
        request.KycProvider = (EKycProvider)Convert.ToInt32(configuration["KycProvider"]!);
        var umService = umServiceFactory.Create(request.KycProvider);
        var result = await umService.Register(request, cancellationToken);
        return result;
    }
}

public record RegisterUMRequest() : BaseCommand<BaseMessageResponse<RegisterUMResponse>>
{
    [MinLength(8)]
    [MaxLength(10)]
    [Required]
    public string NationalCardSeries { get; set; }
    [Required] public long BirthDate { get; set; }
    public EKycProvider KycProvider { get; set; }
}

public class RegisterUMResponse
{
    public string RedirectToUrl { get; set; }
}