using System.Threading;
using IdentityService.Domain.Entity.Profile.UserPorfile.Dto;
using OmidBank.Common.Repository;

namespace IdentityService.Domain.Entity.Profile.UserPorfile;

public interface IUserProfileRepository : IBaseGenericRepository<UserProfileEntity, int>
{
    Task<UserProfileDto>? GetUserProfileByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<UserProfileEntity>? FindByUserId(int userId, CancellationToken cancellationToken);
    Task UpdateSejamStatus(int userId, bool? status, CancellationToken cancellationToken);
    Task UpdateAsync(int userId, string firstName, string lastName, CancellationToken cancellationToken);
}