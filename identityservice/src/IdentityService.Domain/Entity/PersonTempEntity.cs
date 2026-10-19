


namespace IdentityService.Domain.Entity;

public partial class PersonTempEntity
{
    public int Id { get; set; }

    public string MobileNumber { get; set; } = null!;

    public string? NationalCode { get; set; }

    public int? TryValidateByShahkarCount { get; set; }

    public DateTime? LastValidateByShahkarDateTime { get; set; }

    public bool? IsVerifiedByShahkar { get; set; }

    public DateTime? VerifiedByShahkarDateTime { get; set; }

    public int? ActivationCode { get; set; }

    public int? SentActivateionCodesCount { get; set; }

    public DateTime? LastSentActivationCodeDateTime { get; set; }

    public bool? IsVerifiedActivationCode { get; set; }

    public DateTime? VerifiedActivationCodeDateTime { get; set; }

    public int? PersonId { get; set; }

    public string? LastIpAddress { get; set; }

    public string? LastDeviceInfo { get; set; }

    public DateTime? RegisterDate { get; set; }

    public bool IsDeleted { get; set; }

    public int? TryValidateActivationCodeCount { get; set; }

    public string? ApplicationVersion { get; set; }

    public byte? AppId { get; set; }

    public string? InvitedCode { get; set; }

    public virtual PersonEntity? Person { get; set; }
}
