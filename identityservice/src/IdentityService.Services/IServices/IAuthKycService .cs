namespace IdentityService.Services.IServices
{
    public interface IAuthKycService : IBaseService
    {
        AuthKycResultViewModel AuthKycCheckByUid(AuthKycParameterViewModel model, CancellationToken cancellationToken);
    }
}
