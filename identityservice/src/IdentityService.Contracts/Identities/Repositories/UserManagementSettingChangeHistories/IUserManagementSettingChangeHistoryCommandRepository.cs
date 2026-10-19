using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Entities;
using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Enums;

namespace IdentityService.Contracts.Identities.Repositories.UserManagementSettingChangeHistories;

public interface IUserManagementSettingChangeHistoryCommandRepository
{
    Task<UserManagementSettingChangeHistory?> RetrieveAsync(long id, EUserManagementSettingChangeStatus status, CancellationToken cancellationToken);
    Task<UserManagementSettingChangeHistory> InsertAsync(UserManagementSettingChangeHistory changeUserManagementSettingHistory, CancellationToken cancellationToken);
    Task<bool> UpdateStatusAsync(UserManagementSettingChangeHistory history, CancellationToken cancellationToken);
}