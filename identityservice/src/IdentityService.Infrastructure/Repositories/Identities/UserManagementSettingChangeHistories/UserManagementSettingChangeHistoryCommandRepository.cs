#nullable enable
using AutoMapper;
using IdentityService.Contracts.Identities.Repositories.UserManagementSettingChangeHistories;
using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Entities;
using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Enums;
using IdentityService.Infrastructure.Contexts;
using IdentityService.Infrastructure.Entities.Identities;

namespace IdentityService.Infrastructure.Repositories.Identities.UserManagementSettingChangeHistories;

public class UserManagementSettingChangeHistoryCommandRepository(ApplicationDBContext dbContext, IMapper mapper) : IUserManagementSettingChangeHistoryCommandRepository
{
    public async Task<UserManagementSettingChangeHistory?> RetrieveAsync(long id, EUserManagementSettingChangeStatus status, CancellationToken cancellationToken)
    {
        var entity = await dbContext.UserManagementSettingChangeHistories.Where(x => 
            x.Id == id && x.Status == EUserManagementSettingChangeStatus.Unknown).FirstOrDefaultAsync(cancellationToken);
        var result = mapper.Map<UserManagementSettingChangeHistory>(entity);
        return result;
    }

    public async Task<UserManagementSettingChangeHistory> InsertAsync(UserManagementSettingChangeHistory changeUserManagementSettingHistory, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<UserManagementSettingChangeHistoryEntity>(changeUserManagementSettingHistory);
        var resultEntity = await dbContext.UserManagementSettingChangeHistories.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        var result = mapper.Map<UserManagementSettingChangeHistory>(resultEntity.Entity);
        return result;
    }

    public async Task<bool> UpdateStatusAsync(UserManagementSettingChangeHistory history, CancellationToken cancellationToken)
    {
        var entity = await dbContext.UserManagementSettingChangeHistories.Where(x => x.Id == history.Id).FirstOrDefaultAsync(cancellationToken);
        if (entity is null)
            return false;
        entity.Status = history.Status;
        entity.Description = history.Description;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}