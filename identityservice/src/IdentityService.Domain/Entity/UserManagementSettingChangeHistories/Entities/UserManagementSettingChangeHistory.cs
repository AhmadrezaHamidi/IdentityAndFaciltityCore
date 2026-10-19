using IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Enums;

namespace IdentityService.Domain.Entity.UserManagementSettingChangeHistories.Entities;

public class UserManagementSettingChangeHistory
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public EUserManagementSettingChangeStatus Status { get; private set; } = EUserManagementSettingChangeStatus.Unknown;
    public EUserManagementSettingOperation Operation { get; private set; }
    public DateTime CreateAt { get; private set; } = DateTime.Now;
    public string Description { get; private set; } = string.Empty;

    public static UserManagementSettingChangeHistory Create(long userId, EUserManagementSettingOperation operation)
    {
        return new UserManagementSettingChangeHistory
        {
            UserId = userId,
            Operation = operation,
        };
    }
    
    public void SetAsSuccessful()
    {
        Status = EUserManagementSettingChangeStatus.Success;
    }
    
    public void SetAsFailed(string errorMessage)
    {
        Status = EUserManagementSettingChangeStatus.Failed;
        Description = errorMessage;
    }
}