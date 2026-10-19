using IdentityService.Domain.Entity.Basic.BankData;
using IdentityService.Domain.Entity.Basic.BankDataCity;
using IdentityService.Domain.Entity.Basic.BankDataProvince;
using IdentityService.Domain.Entity.ChangeMobileNumber;
using IdentityService.Domain.Entity.ContentManagement;
using IdentityService.Domain.Entity.Profile.UserAddress;
using IdentityService.Domain.Entity.Profile.UserPorfile;
using IdentityService.Domain.Entity.UserManagement.LoginLogOutHistory;
using IdentityService.Domain.Entity.UserManagement.UserManagementKycTransaction;
using IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;
using IdentityService.Domain.Entity.UserManagement.UserManagmentBiometric;
using IdentityService.Infrastructure.Entities.Identities;
using IdentityService.Infrastructure.Entities.RegexValidations;

namespace IdentityService.Infrastructure.Contexts;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    {
    }


    public virtual DbSet<ChangePasswordUserManagmentLogEntity> ChangePasswordUserManagmentLog { get; set; }

    public virtual DbSet<FileEntity> File { get; set; }

    public virtual DbSet<UserManagementKycLogEntity> UserManagementKycLog { get; set; }

    public virtual DbSet<MerchantOmidBankEntity> MerchantOmidBank { get; set; }

    public virtual DbSet<PanelMenuEntity> PanelMenu { get; set; }

    public virtual DbSet<PanelUserEntity> PanelUser { get; set; }

    public virtual DbSet<PanelUserRoleEntity> PanelUserRole { get; set; }

    public virtual DbSet<PanelSecurityAuditLogEntity> PanelSecurityAuditLog { get; set; }

    public virtual DbSet<PersonEntity> People { get; set; }

    public virtual DbSet<PersonDetailEntity> PersonDetail { get; set; }

    public virtual DbSet<PersonOpenAccountEntity> PersonOpenAccount { get; set; }

    public virtual DbSet<PersonTempEntity> PersonTemp { get; set; }

    public virtual DbSet<PolicyEntity> Policie { get; set; }

    public virtual DbSet<RoleEntity> Role { get; set; }

    public virtual DbSet<RolePanelMenuEntity> RolePanelMenu { get; set; }

    public virtual DbSet<SepahMerchantsListEntity> SepahMerchantsList { get; set; }

    public virtual DbSet<UserEntity> User { get; set; }

    public virtual DbSet<UserBranchEntity> UserBranch { get; set; }

    public virtual DbSet<UserCivilRegistryEntity> UserCivilRegistry { get; set; }

    public virtual DbSet<UserManagmentEntity> UserManagment { get; set; }

    public virtual DbSet<UserManagmentBiometricEntity> UserManagmentBiometric { get; set; }

    public virtual DbSet<UserManagementTokenLogEntity> UserManagementTokenLog { get; set; }

    public virtual DbSet<UserMessageEntity> UserMessage { get; set; }

    public virtual DbSet<UserOrganizationEntity> UserOrganization { get; set; }

    public virtual DbSet<UserValidateLogEntity> UserValidateLog { get; set; }

    public virtual DbSet<BankDataEntity> BankData { get; set; }
    public virtual DbSet<UserAddressEntity> UserAddress { get; set; }
    public virtual DbSet<UserProfileEntity> UserProfile { get; set; }
    
    public virtual DbSet<BankDataProvinceEntity> BankDataProvince { get; set; }
    public virtual DbSet<BankDataCityEntity> BankDataCity { get; set; }
    public virtual DbSet<LoginLogoutHistoryEntity> LoginLogoutHistory { get; set; }
    
    public virtual DbSet<ChangeMobileNumberEntity> ChangeMobileNumber { get; set; }
    
    // V3 Databases
    public virtual DbSet<ActiveCodeEntity> ActiveCodes { get; set; }
    public virtual DbSet<ActiveTokenEntity> ActiveTokens { get; set; }

    public virtual DbSet<ChangeUsernameActiveCodes> ChangeUsernameActiveCodes { get; set; }
    public virtual DbSet<ChangePasswordActiveCodes> ChangePasswordActiveCodes { get; set; }
    public virtual DbSet<UserManagementSettingChangeHistoryEntity> UserManagementSettingChangeHistories { get; set; }
    public virtual DbSet<PropertyRegexEntity> PropertyRegexs { get; set; }
    public virtual DbSet<RegexPatternEntity> RegexPatterns { get; set; }
    public virtual DbSet<UserManagementKycTransactionEntity> UserManagementKycTransaction { get; set; }
    public virtual DbSet<UserServiceAccessEntity> UserServiceAccess { get; set; }

    public virtual DbSet<ContentItemEntity> ContentItem { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        builder.Entity<PropertyRegexEntity>()
            .HasKey(x => new { x.RegexId, x.PropertyName });

        builder.Entity<PropertyRegexEntity>()
            .HasOne(x => x.RegexPattern)
            .WithMany(r => r.PropertyRegexes)
            .HasForeignKey(x => x.RegexId);

        builder.Entity<PropertyRegexEntity>()
            .Property(x => x.PropertyName)
            .IsRequired();
        
        builder.Entity<UserManagmentEntity>(entity =>
        {
            entity.HasIndex(e => e.UserId)
                .IsUnique();
        });
    }
}