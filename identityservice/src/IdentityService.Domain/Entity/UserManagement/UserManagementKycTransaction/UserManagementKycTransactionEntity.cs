using System.ComponentModel.DataAnnotations;
using static OmidBank.Common.Enums;

namespace IdentityService.Domain.Entity.UserManagement.UserManagementKycTransaction;

public partial class UserManagementKycTransactionEntity
{
    public int Id { get; set; }
    [MaxLength(64)]
    public string Token { get; set; } = string.Empty;
    [MaxLength(20)]
    public string NationalCardSeries { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int UserId { get; set; }
    public int ProviderType { get; set; }
    public AppEnum AppId { get; set; }
    public KycType KycType { get; set; }
    public bool IsWagePaid { get; set; }
    public DateTime PaidWageDateTime { get; set; }
    public DateTime CreateDateTime { get; set; }
    public virtual UserEntity User { get; set; } = null!;

}