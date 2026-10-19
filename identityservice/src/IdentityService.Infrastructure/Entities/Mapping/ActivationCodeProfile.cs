using AutoMapper;
using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Entities;
using IdentityService.Infrastructure.Entities.Identities;

namespace IdentityService.Infrastructure.Entities.Mapping;

public class ActivationCodeProfile : Profile
{
    public ActivationCodeProfile()
    {
        CreateMap<ActivationCodeModel, ActiveCodeEntity>();
        CreateMap<ActiveCodeEntity, ActivationCodeModel>();
        CreateMap<UserManagementSettingChangeHistory, UserManagementSettingChangeHistoryEntity>();
        CreateMap<UserManagementSettingChangeHistoryEntity, UserManagementSettingChangeHistory>();

        CreateMap<ChangePasswordActiveCodes, ActivationCodeModel>();
        CreateMap<ActivationCodeModel, ChangePasswordActiveCodes>();
        CreateMap<ChangeUsernameActiveCodes, ActivationCodeModel>();
        CreateMap<ActivationCodeModel, ChangeUsernameActiveCodes>();
    }
}