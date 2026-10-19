namespace IdentityService.Domain.Entity;

public partial class UserManagmentEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? PasswordUpdateDate { get; set; }
    public DateTime? TakeCertificateDate { get; set; }
    public bool? TosanRegister { get; set; }

    public bool? AutoUpdateCif { get; set; }
    public int? TryCountAutoUpdateCif { get; set; }
    public string? AdditionalDescription { get; set; }
    public virtual UserEntity User { get; set; } = null!;
}