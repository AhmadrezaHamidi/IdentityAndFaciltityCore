using IdentityService.Domain.Entity.Profile.UserAddress.Dto;

namespace IdentityService.Domain.Entity.Profile.UserAddress;

public interface IUserAddressRepository : IBaseGenericRepository<UserAddressEntity, int>
{
    Task<UserAddressDto>? GetUserAddressByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<UserAddressEntity>? FindByUserId(int userId, CancellationToken cancellationToken);
}