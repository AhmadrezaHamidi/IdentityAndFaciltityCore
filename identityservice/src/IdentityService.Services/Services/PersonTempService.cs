using IdentityService.Domain.IRepositories;

namespace IdentityService.Services.Services;

public class PersonTempService : BaseService, IPersonTempService
{
    private readonly ILogger<PersonTempService> _logger;
    private readonly IPersonTempRepository _personTempRepository;

    public PersonTempService(ILogger<PersonTempService> logger, IPersonTempRepository personTempRepository)
    {
        _logger = logger;
        _personTempRepository = personTempRepository;
    }

    public bool CheckLastValidateByShahkarDate(string mobileNumber, int month, CancellationToken cancellationToken)
    {
        try
        {
            var personTemp = _personTempRepository.GetPersonTempByMobileNumber(mobileNumber, cancellationToken);

            if (personTemp == null) return false;

            if (personTemp.LastValidateByShahkarDateTime != null &&
                personTemp.LastValidateByShahkarDateTime.Value <= DateTime.Now.AddMonths(-month))
                return false;
            else
                return true;
        }
        catch (Exception exp)
        {
            _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            return false;
        }
    }

    public bool UpdateLastValidateByShahkarDateTime(PersonTempModel personTemp, CancellationToken cancellationToken)
    {
        try
        {
            personTemp.LastValidateByShahkarDateTime = DateTime.Now;
            _personTempRepository.UpdatePersonTemp(personTemp, cancellationToken);
            return true;
        }
        catch (Exception exp)
        {
            _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            return false;
        }
    }
}
