using System.Threading;

namespace IdentityService.Domain.IRepositories
{
    public interface IPersonTempRepository : IBaseRepository
    {
        PersonTempModel GetPersonTempById(int id, CancellationToken cancellationToken);
        PersonTempModel GetPersonTempByMobileNumber(string mobileNumber, CancellationToken cancellationToken);
        List<PersonTempModel> GetAllPersonTemp(CancellationToken cancellationToken);
        int InsertPersonTemp(PersonTempModel personTemp, CancellationToken cancellationToken);
        bool GetPersonTempByIpAddress(string ip, CancellationToken cancellationToken);
        bool GetPersonTempRegisterCountByIpAddress(string ip, CancellationToken cancellationToken);
        bool GetTestUser(string mobilNumber, CancellationToken cancellationToken);
        int UpdatePersonTemp(PersonTempModel personTemp, CancellationToken cancellationToken);
        int DeletePersonTemp(int id, CancellationToken cancellationToken);
        Task<IEnumerable<PersonTempModel>> GetUserLoginReportAsync(string nationalCode, string mobileNumber, CancellationToken cancellationToken);
        PersonTempModel GetLastPersonTempByMobileNumber(string mobileNumber, CancellationToken cancellationToken);
        Task<PersonTempModel> GetPersonTempInfo(string mobileNumber, CancellationToken cancellationToken);
        Task UpdateApplicationVersion(int id, string applicationVersion, CancellationToken cancellationToken);
    }
}
