namespace IdentityService.Domain.Entity.UserManagement.UserManagementTokenLog;

public partial class UserManagementTokenLogEntity
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public DateTime? ExpierDate { get; set; }

    public bool? IsActive { get; set; }

    public int? Type { get; set; }

    public string Ip { get; set; }

    public string DeviceInfo { get; set; }

    public int? UserManagmentBiometricId { get; set; }

    public virtual UserEntity User { get; set; } = null!;
}
