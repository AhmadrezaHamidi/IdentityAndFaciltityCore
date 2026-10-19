using IdentityService.Services.ViewModels.Admin;

namespace IdentityService.Services.IServices.Admin
{
    public interface IAccountService : IBaseService
    {
        Task<DataList<IEnumerable<ResponseForgotPasswordOtpViewModel>>> ReadForgotPasswordOtpService(ForgetPasswordViewModel viewModel, CancellationToken cancellationToken);
    }
}