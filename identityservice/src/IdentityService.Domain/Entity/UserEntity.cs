using IdentityService.Domain.Entity.Profile.UserAddress;
using IdentityService.Domain.Entity.Profile.UserPorfile;
using IdentityService.Domain.Entity.UserManagement.LoginLogOutHistory;
using IdentityService.Domain.Entity.UserManagement.UserManagementKycTransaction;
using IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;

namespace IdentityService.Domain.Entity;

public partial class UserEntity
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public bool BlockByAdmin { get; set; }

    public string? InvitedCode { get; set; }
    public bool? HasLevel3Authorization { get; set; }

    public string? FirebaseKey { get; set; }

    public string? MyInviteCode { get; set; }
    public long? BankCustomerNumber { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<ChangePasswordUserManagmentLogEntity> ChangePasswordUserManagmentLogs { get; set; } = new List<ChangePasswordUserManagmentLogEntity>();

    public virtual ICollection<UserManagementKycLogEntity> UserManagementKycLogs { get; set; } = new List<UserManagementKycLogEntity>();

    public virtual PersonEntity Person { get; set; } = null!;

    public virtual ICollection<UserCivilRegistryEntity> UserCivilRegistries { get; set; } = new List<UserCivilRegistryEntity>();

    public virtual ICollection<UserManagementTokenLogEntity> UserManagementTokenLogs { get; set; } = new List<UserManagementTokenLogEntity>();

    public virtual ICollection<UserManagmentEntity> UserManagments { get; set; } = new List<UserManagmentEntity>();

    public virtual ICollection<UserMessageEntity> UserMessages { get; set; } = new List<UserMessageEntity>();

    public virtual ICollection<UserOrganizationEntity> UserOrganizations { get; set; } = new List<UserOrganizationEntity>();

    public virtual ICollection<UserValidateLogEntity> UserValidateLogs { get; set; } = new List<UserValidateLogEntity>();
    public virtual ICollection<UserAddressEntity> UserAddress { get; set; } = new List<UserAddressEntity>();
    public virtual ICollection<UserProfileEntity> UserProfiles { get; set; } = new List<UserProfileEntity>();
    public virtual ICollection<LoginLogoutHistoryEntity>? LoginLogoutHistories { get; set; } = new List<LoginLogoutHistoryEntity>();

    public virtual ICollection<UserManagementKycTransactionEntity> UserManagementKycTransaction { get; set; } = new List<UserManagementKycTransactionEntity>();


    public void Active()
    {
        if (!IsActive)
        {
            IsActive = true;
        }
    }
    public void Deactive()
    {
        if (IsActive)
        {
            IsActive = false;
        }
    }
}
