using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Enums;

namespace IdentityService.Infrastructure.Entities.Identities;

public class UserManagementSettingChangeHistoryEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public EUserManagementSettingChangeStatus Status { get; set; }
    public EUserManagementSettingOperation Operation { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public string Description { get; set; } = string.Empty;
}