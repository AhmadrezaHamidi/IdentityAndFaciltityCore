using IdentityService.Domain.Models;
using System.Threading;

namespace IdentityService.Domain.IRepositories
{
    public interface IActiveTokenRepository : IBaseRepository
    {
        Task AddActivationCodeAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken);
        Task<bool> VerifyActivationCodeAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken);
        Task UpdateActivationCodeAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken);
        Task GenerateTokenAsync(ActiveTokenModel activeToken, CancellationToken cancellationToken);
        Task<object> CheckTokenAsync(ActiveTokenModel activeToken, CancellationToken cancellationToken);
        Task<ActiveTokenModelBeforeLogin> RetrieveTokenBeforeLoginAsync(ActiveTokenModelBeforeLogin activeToken, CancellationToken cancellationToken);
        Task DeActiveTokenAsync(string mobileNumber, string token, CancellationToken cancellationToken);
        Task DeActiveTokenBeforeLoginAsync(string mobileNumber, string token, CancellationToken cancellationToken);
        Task DeActiveTokenAsync(string token, CancellationToken cancellationToken);
        Task DeActiveTokenBeforeLoginAsync(string token, CancellationToken cancellationToken);
        Task InsertTokenAsync(ActiveTokenModel activeToken, CancellationToken cancellationToken);
        Task InsertTokenBeforeLoginAsync(ActiveTokenModelBeforeLogin activeToken, CancellationToken cancellationToken);
        Task ForgotPasswordOtpAsync(ForgotPasswordOtpModel forgotPasswordOtp, CancellationToken cancellationToken);
        Task<bool> CheckRefrenceIdResetPasswordAsync(ForgotPasswordOtpModel forgotPasswordOtp, CancellationToken cancellationToken);
        Task<bool> VerifyForgotPasswordOtpAsync(ForgotPasswordOtpModel forgotPasswordOtp, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<ActivationCodeModel>>> ReadActivationCodeAsync(int offset, int count, DateTime? DateFrom, DateTime? DateTo, string mobileNumber, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<ForgotPasswordOtpModel>>> ReadForgotPasswordOtpAsync(int offset, int count, DateTime? DateFrom, DateTime? DateTo, CancellationToken cancellationToken);
        Task<bool> LimitationSendActivationCode(string mobileNumber, CancellationToken cancellationToken);
        Task<bool> LimitationVerifyActivationCode(string mobileNumber, CancellationToken cancellationToken);
        Task<bool> LimitationLoginWithIpAddress(string IpAddress, CancellationToken cancellationToken);
        //Task<bool> GetTryCount(string mobileNumber, string url, int serviceStatus, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<ActiveTokenModel?>>> GetActiveTokenAsync(string mobileNumber, long? timStamp, CancellationToken cancellationToken);
        Task<string> GetActiveTokenByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken);
        Task UpdateActivationCodeByMobileNumber(string mobileNumber, CancellationToken cancellationToken);
        Task<bool> IsTokenActiveAsync(string token);
    }
}
