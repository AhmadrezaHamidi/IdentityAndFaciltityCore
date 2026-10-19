using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityService.Domain.Entity.Basic.BankDataCity;
using IdentityService.Domain.Entity.UserManagementKycLog.Dto;
using IdentityService.Domain.Models;

namespace IdentityService.Domain.Entity.UserManagementKycLog;

public interface IUserManagementKycLogRepository : IBaseGenericRepository<UserManagementKycLogEntity, int>
{
    Task<UserManagementKycLogEntity> GetByKycTokenAsync(string token, CancellationToken cancellationToken);
  //  Task<UserManagementKycLogEntity> GetByUserNameAsync(string userName, CancellationToken cancellationToken);
    Task<UserManagementKycLogEntity> GetByUserIdAsync(int userId,CancellationToken cancellationToken);
    Task<List<ReportUmDto>> ReportUM(ReportUmRequestModel request, CancellationToken cancellationToken);
    Task<UserManagementKycLogEntity> GetByUserIdUmRegisterAsync(int userId, CancellationToken cancellationToken);
}
  