using IdentityService.Services.ViewModels;

namespace IdentityService.Services.IServices
{
    public interface ISmsService : IBaseService
    {
        Task<SmsThirdPartyResponseModel> SendActivationCodeFromThirdParty(string mobileNumber, string code, string url,string userName, CancellationToken cancellationToken);
        Task<bool> SendActivationCodeFromThirdPartyOmidTech(SendActivitionCodeOmidTechViewModel viewModel /*string mobileNumber, string scheduler, string code, string url*/, CancellationToken cancellationToken);
    }
}
