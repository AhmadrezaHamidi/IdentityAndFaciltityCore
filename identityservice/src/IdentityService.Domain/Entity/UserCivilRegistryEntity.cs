


namespace IdentityService.Domain.Entity;

public partial class UserCivilRegistryEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? CardSerial { get; set; }

    public string? NationalId { get; set; }

    public string? UserImage { get; set; }

    public string? BirthDate { get; set; }

    public int? BookNo { get; set; }

    public int? BookRow { get; set; }

    public int? DeathStatus { get; set; }

    public string? Family { get; set; }

    public string? FatherName { get; set; }

    public bool? Gender { get; set; }

    public string? Name { get; set; }

    public int? OfficeCode { get; set; }

    public string? OfficeName { get; set; }

    public int? ShenasnameNo { get; set; }

    public string? ShenasnameSeri { get; set; }

    public string? ShenasnameSerial { get; set; }

    public int? DeathDate { get; set; }

    public string? Zipcode { get; set; }

    public string? ZipcodeDesc { get; set; }

    public virtual UserEntity User { get; set; } = null!;
}
