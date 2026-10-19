using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Common.Enums;
using IdentityService.Application.Services.Kyc.Vinara;
using IdentityService.Domain.Entity;
using OmidBank.Common.InternalThreadParty.OmidTechAuthentication.Models;

namespace IdentityService.Application.Services.Kyc.Common.Interfaces;

public interface IKycServiceProvider
{
    EKycProvider Provider { get; set; }
    Task<BaseMessageResponse<InquiryResponse>> InquiryAsync(string token, KycResult? umStatus, CancellationToken cancellationToken);
    Task<LinkInfo?> SubmitAuthentication(RegisterUMRequest request, CancellationToken cancellationToken);
    BaseMessageResponse<TData> GetProviderErrorMessage<TData>();
    LinkInfo GenerateRedirectUrl(string url, string? token);
    KycStep GetKycStep(InquiryResponse inquiryResponse);
}