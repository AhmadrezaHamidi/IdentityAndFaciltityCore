using IdentityService.Domain.Entity.UserManagementKycLog;
using IdentityService.Domain.Entity.UserManagementKycLog.Dto;
using IdentityService.Infrastructure.Contexts;
using XAct;
using XAct.Users;

namespace IdentityService.Infrastructure.Repositories.UserManagementKycLog;

public class UserManagementKycLogRepository : BaseGenericRepository<ApplicationDBContext, UserManagementKycLogEntity, int>, IUserManagementKycLogRepository
{
    private readonly ApplicationDBContext _applicationDbContext;

    public UserManagementKycLogRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    //public async Task<UserManagementKycLogEntity> GetByUserNameAsync(string userName, CancellationToken cancellationToken)
    //    => await _entityContext.FirstOrDefaultAsync(a => a.UserName == userName, cancellationToken);

    public async Task<UserManagementKycLogEntity> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        => await _entityContext.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

    public async Task<UserManagementKycLogEntity> GetByUserIdUmRegisterAsync(int userId, CancellationToken cancellationToken)
       => await _entityContext.FirstOrDefaultAsync(a => a.KycType == KycType.RegisterUM && a.UserId == userId, cancellationToken);

    public async Task<List<ReportUmDto>> ReportUM(ReportUmRequestModel request, CancellationToken cancellationToken)
    {
        var result = _entityContext
            .Include(a => a.User)
            .ThenInclude(a => a.Person)
            .Select(m => new ReportUmDto
            {
                NationalCode = m.User!.Person.NationalCode,
                Mobile = m.User!.Person.MobileNumber,
                BirthDate = m.User!.Person.BirthDate == null ? "" : DateConverterHelper.ConvertToShamsi(m.User!.Person.BirthDate, Enums.DateTimeFormatEnum.Date),
                // Username = m.UserName,
                RequestType = m.KycType == KycType.RegisterUM ? "ایجاد کاربری" : "فراموشی رمز",
                Status = m.KycStep == KycStep.NotExistUser ? "کاربر وجود ندارد" :
                    m.KycStep == KycStep.GenerateLink ? "ایجاد لینک" :
                    m.KycStep == KycStep.Pending ? "در انتظار تایید احراز هویت" :
                    m.KycStep == KycStep.Approved ? "تأیید اولیه" :
                    m.KycStep == KycStep.Rejected ? "رد شده نهایی" : "",

                NationalCardSeries = m.User!.Person.NationalCardSeries,
            }).AsEnumerable()
            .Union(
                _applicationDbContext.UserManagment.Include(b => b.User)
                    .ThenInclude(b => b.Person).Select(m => new ReportUmDto
                    {
                        NationalCode = m.User!.Person.NationalCode,
                        Mobile = m.User!.Person.MobileNumber,
                        BirthDate = m.User!.Person.BirthDate == null ? "" : DateConverterHelper.ConvertToShamsi(m.User!.Person.BirthDate, Enums.DateTimeFormatEnum.Date),
                        // Username = m.UserName,
                        RequestType = "ایجاد کاربری",
                        Status = "تأیید شده نهایی",
                        NationalCardSeries = m.User!.Person.NationalCardSeries,
                    }).AsEnumerable()
            )
            .Where(l =>
                (string.IsNullOrEmpty(request.NationalCode) || l.NationalCode == request.NationalCode) &&
                (string.IsNullOrEmpty(request.Mobile) || l.Mobile == request.Mobile) &&
                (string.IsNullOrEmpty(request.Username)) &&
                (string.IsNullOrEmpty(request.RequestType) || l.RequestType == request.RequestType.ToString()) &&
                (string.IsNullOrEmpty(request.Status) || l.Status == request.Status.ToString()) &&
                (string.IsNullOrEmpty(request.BirthDateTo) || (!string.IsNullOrEmpty(l.BirthDate) && DateConverterHelper.ConvertToMiladiDate(l.BirthDate) <= DateConverterHelper.ConvertToMiladiDate(request.BirthDateTo))) &&
                (string.IsNullOrEmpty(request.BirthDateFrom) || (!string.IsNullOrEmpty(l.BirthDate) && DateConverterHelper.ConvertToMiladiDate(l.BirthDate) >= DateConverterHelper.ConvertToMiladiDate(request.BirthDateFrom)))).Distinct()
            .Skip(request.Offset)
            .Take(request.Count)
            .ToList();
        return result;
    }

    public async Task<UserManagementKycLogEntity> GetByKycTokenAsync(string token, CancellationToken cancellationToken)
        => await _entityContext
            .Include(a => a.User)
            .ThenInclude(a => a.Person)
            .FirstOrDefaultAsync(a => a.Token == token, cancellationToken);
}