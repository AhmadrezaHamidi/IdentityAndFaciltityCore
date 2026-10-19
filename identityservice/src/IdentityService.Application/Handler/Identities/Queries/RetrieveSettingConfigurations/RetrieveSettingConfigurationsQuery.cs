using IdentityService.Contracts.Identities.ReadModels;

namespace IdentityService.Application.Handler.Identities.Queries.RetrieveSettingConfigurations;

public record RetrieveSettingConfigurationsQuery : BaseCommand<BaseMessageResponse<RetrieveSettingConfigurationsDto>>;