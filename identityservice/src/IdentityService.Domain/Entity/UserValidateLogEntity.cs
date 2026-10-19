


namespace IdentityService.Domain.Entity;

public partial class UserValidateLogEntity
{
    public long Id { get; set; }

    public int UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? Status { get; set; }

    public string? UserImage { get; set; }

    public string? InputData { get; set; }

    public virtual UserEntity User { get; set; } = null!;
}
