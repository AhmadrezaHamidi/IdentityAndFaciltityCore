using IdentityService.Domain.Entity;

namespace IdentityService.Application.Services.Kyc.Common;

public class InquiryResponse
{
    public KycStep UmStatus { get; set; }
    public KycType? KycType { get; set; }
    public bool IsWagePaid { get; set; }
}