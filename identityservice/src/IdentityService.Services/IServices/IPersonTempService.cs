namespace IdentityService.Services.IServices;

public interface IPersonTempService : IBaseService
{
    bool CheckLastValidateByShahkarDate(string mobileNumber, int month, CancellationToken cancellation);
    public bool UpdateLastValidateByShahkarDateTime(PersonTempModel personTemp, CancellationToken cancellationToken);
}
