using IdentityService.Application.Handler.Identities.Services;
using IdentityService.Contracts.Identities.ReadModels;

namespace IdentityService.Application.Handler.Identities.Queries.RetrieveSettingConfigurations;

public class RetrieveSettingConfigurationsQueryHandler(IIdentityService identityService) : IBaseHandler<RetrieveSettingConfigurationsQuery, BaseMessageResponse<RetrieveSettingConfigurationsDto>>
{
    public async Task<BaseMessageResponse<RetrieveSettingConfigurationsDto>> Handle(RetrieveSettingConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var result = await identityService.RetrieveSettingConfigurations(request, cancellationToken);
        return result;
    }
}