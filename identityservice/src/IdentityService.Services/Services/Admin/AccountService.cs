using AutoMapper;
using IdentityService.Domain.IRepositories;
using IdentityService.Services.IServices.Admin;
using IdentityService.Services.ViewModels.Admin;

namespace IdentityService.Services.Services.Admin
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IActiveTokenRepository _repository;
        private readonly IMapper _mapper;

        public AccountService(ILogger<AccountService> logger,
            IActiveTokenRepository repository, IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<DataList<IEnumerable<ResponseForgotPasswordOtpViewModel>>> ReadForgotPasswordOtpService(ForgetPasswordViewModel viewModel, CancellationToken cancellationToken)
        {
            try
            {
                var models = await _repository.ReadForgotPasswordOtpAsync(viewModel.Offset, viewModel.Count, viewModel.StartDate, viewModel.EndDate, cancellationToken);
                if (models != null)
                {
                    var viewModels = _mapper.Map<IEnumerable<ResponseForgotPasswordOtpViewModel>>(models.Data);

                    var result = new DataList<IEnumerable<ResponseForgotPasswordOtpViewModel>>();
                    result.Data = viewModels;
                    result.RecordsFiltered = models.RecordsFiltered;
                    result.RecordsTotal = models.RecordsTotal;

                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }
    }
}