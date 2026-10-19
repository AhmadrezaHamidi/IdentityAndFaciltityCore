using IdentityService.Domain.Entity.Profile.UserAddress;
using IdentityService.Domain.Entity.Profile.UserAddress.Dto;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.User;

public class UserAddressRepository : BaseGenericRepository<ApplicationDBContext, UserAddressEntity, int>, IUserAddressRepository
{
    public UserAddressRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
    }

    public async Task<UserAddressDto> GetUserAddressByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await _entityContext.Where(a => a.UserId == userId).Select(a => new UserAddressDto()
        {
           
            PostalCode = a.PostalCode,
            ProvinceName = a.ProvinceName,
            CityName = a.CityName,
            TownShipName = a.TownShipName,
            ZoneName = a.ZoneName,
            VillageName = a.VillageName,
            SubLocalityName = a.SubLocalityName,
            Street = a.Street,
            HouseNumber = a.HouseNumber,
            Floor = a.Floor,
            SideFloor = a.SideFloor,
            BuildingName = a.BuildingName,
            Description = a.Description,
            PhoneNumber = a.PhoneNumber,
            Unit = a.Unit,
        }).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserAddressEntity> FindByUserId(int userId, CancellationToken cancellationToken)
        => await _entityContext.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

}