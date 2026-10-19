using IdentityService.Domain.Entity.Profile.UserPorfile;
using IdentityService.Domain.Entity.Profile.UserPorfile.Dto;
using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories.User;

public class UserProfileRepository : BaseGenericRepository<ApplicationDBContext, UserProfileEntity, int>, IUserProfileRepository
{
    public UserProfileRepository(ApplicationDBContext applicationDbContext) : base(applicationDbContext)
    {
    }


    public async Task<UserProfileDto>? GetUserProfileByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await _entityContext.Where(a => a.UserId == userId).Select(a => new UserProfileDto()
        {
            RealFirstName = a.RealFirstName,
            RealLastName = a.RealLastName,
            FatherName = a.FatherName,
            MotherName = a.MotherName,
            CustomerGroupId = a.CustomerGroupId,
            EducationId = a.EducationId,
            ResidentialStatusId = a.ResidentialStatusId,
            SurnameId = a.SurnameId,
            SexId = a.SexId,
            RegisterLocationId = a.RegisterLocationId,
            BirthLocationId = a.BirthLocationId,
            CountryId = a.CountryId,
            BirthCountryId = a.BirthCountryId,
            MarriageId = a.MarriageId,
            BirthCertificateNo = a.BirthCertificateNo,
            BirthCertificateSerialNo = a.BirthCertificateSerialNo,
            IdentityCardNumber = a.IdentityCardNumber,
            IdentityCardSeries = a.IdentityCardSeries,
            IdentityCardSerial = a.IdentityCardSerial,
            DeathStatus = a.DeathStatus,
            JobTypeId = a.JobTypeId,
            ReligionId = a.ReligionId,
            SejamStatus = a.SejamStatus,
            CityCode = a.CityCode,
            StateCode = a.StateCode,
            BirthStateCode = a.BirthStateCode,
            BirthCityCode = a.BirthCityCode,
            RegisterCityCode = a.RegisterCityCode,
            RegisterStateCode = a.RegisterStateCode,
            RealFirstNameEn = a.RealFirstNameEn,
            RealLastNameEn = a.RealLastNameEn,
            FatherNameEn = a.FatherNameEn,
        }).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserProfileEntity>? FindByUserId(int userId, CancellationToken cancellationToken)
        => await _entityContext.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

    public async Task UpdateSejamStatus(int userId, bool? sejamStatus, CancellationToken cancellationToken)
        => await _entityContext.Where(a => a.UserId == userId)
            .ExecuteUpdateAsync(a => a.SetProperty(c => c.SejamStatus, sejamStatus), cancellationToken);


    public async Task UpdateAsync(int userId, string firstName, string lastName, CancellationToken cancellationToken)
    {
        var entity = await _entityContext
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

        if (entity is null)
        {
            entity = new UserProfileEntity
            {
                UserId = userId,
                RealFirstName = firstName,
                RealLastName = lastName,
            };

            _entityContext.Add(entity);
        }
        else
        {
            entity.RealFirstName = firstName;
            entity.RealLastName = lastName;
        }

      
    }
}