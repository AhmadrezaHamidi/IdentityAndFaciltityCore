


namespace IdentityService.Domain.Entity;

public partial class PolicyEntity
{
    public int Id { get; set; }

    public int? MaxCountLogin { get; set; }

    public int? MaxCountCallBankService { get; set; }

    public int? MaxForgetPassword { get; set; }

    public int? MaxRefreshToken { get; set; }

    public int? ServiceId { get; set; }
}
