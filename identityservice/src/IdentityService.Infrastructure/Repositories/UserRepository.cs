using IdentityService.Domain.Models.UserBranchs;
using IdentityService.Infrastructure.Contexts;
using OmidBank.Common.Base.Shared;

namespace IdentityService.Infrastructure.Repositories
{
    public class UserRepository : BaseGenericRepository<ApplicationDBContext, UserEntity, int>, IUserRepository
    {

        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly int _checkNationalCodeTryCount;
        private readonly ApplicationDBContext _applicationDbContext;

        public UserRepository(IConfiguration configuration, ILogger<UserRepository> logger, ApplicationDBContext applicationDbContext) : base(applicationDbContext)

        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("IdentityConnection");
            _checkNationalCodeTryCount = int.Parse(configuration["CheckNationalCodeTryCount"]);
            _applicationDbContext = applicationDbContext;
        }


        public async Task<UserModel> CheckPanelUserByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            var userRow = await
               (
                   from u in _applicationDbContext.PanelUser.AsNoTracking()
                   join p in _applicationDbContext.People.AsNoTracking()
                       on u.PersonId equals p.Id
                   join pd in _applicationDbContext.PersonDetail.AsNoTracking()
                       on p.Id equals pd.PersonId into pds
                   from pd in pds.DefaultIfEmpty()
                   where u.UserName == userName && u.IsActive
                   select new
                   {
                       PanelUserId = u.Id,
                       u.PersonId,
                       p.FirstName,
                       p.LastName,
                       p.MobileNumber,
                       u.UserName,
                       u.CreationTime,
                       p.NationalCode,
                       AvatarName = pd != null ? pd.AvatarName : null,
                       u.IsActive,
                       p.IsVerified,
                       Password = ***;

            if (userRow == null)
                return null;

            var user = new UserModel(
                userRow.PanelUserId,
                userRow.PersonId,
                userRow.FirstName,
                userRow.LastName,
                userRow.MobileNumber,
                userRow.UserName,
                userRow.CreationTime,
                userRow.NationalCode,
                userRow.AvatarName,
                userRow.IsActive,
                userRow.IsVerified,
                userRow.Password,
                email: userRow.Email
            );

            var roleMenus = await
               (
                   from r in _applicationDbContext.Role.AsNoTracking()
                   join ur in _applicationDbContext.PanelUserRole.AsNoTracking()
                       on r.Id equals ur.RoleId
                   join rpm in _applicationDbContext.RolePanelMenu.AsNoTracking()
                       on r.Id equals rpm.RoleId
                   join pm in _applicationDbContext.PanelMenu.AsNoTracking()
                       on rpm.PanelMenuId equals pm.Id
                   where ur.PanelUserId == userRow.PanelUserId
                         && r.IsActive
                         && pm.IsActive
                   select new RoleMenuModel(
                       r.Id,
                       r.IsActive,
                       r.PersianTitle,
                       r.Title,
                       pm.Url,
                       pm.Title,
                       pm.ParentId,
                       pm.IconName,
                       pm.ControllerName,
                       pm.ActionName,
                       pm.IsRootMenu,
                       pm.IsInternalMenu
                   )
               ).ToListAsync(cancellationToken);

            user.SetRoleMenuModels(roleMenus);
            return user;
        }

        public int UpdatePersonWithCivilRegistery(string FirstName, string LastName, bool Gender, string NationalCode, CancellationToken cancellationToken)
        {
            SqlConnection conn = null;
            SqlConnection connection = null;
            try
            {
                int yourValue = 0;
                string sql = "Update Person set FirstName = '" + FirstName + "',LastName='" + LastName + "' OUTPUT INSERTED.Id Where NationalCode = '" + NationalCode + "'";
                using (conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    yourValue = (int)cmd.ExecuteScalar();
                }

                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute("Update PersonDetail set Gender = @Gender Where PersonId = @PersonId", new { Gender, PersonId = yourValue });
                    connection.Close();
                }

                return 1;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return default;
            }
            finally
            {
                if (connection != null)
                {
                    conn.Close();
                    conn.Dispose();
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<AddPersonStatusModel> AddPersonAsync(PersonModel person, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddPerson]";

                    connection.Open();

                    var personStatus = await connection.QueryFirstOrDefaultAsync<AddPersonStatusModel>(query,
                        new
                        {
                            person.FirstName,
                            person.LastName,
                            person.NationalCode,
                            person.MobileNumber
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return personStatus;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<PersonModel> GetPersonByIdAsync(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPersonById]";

                    connection.Open();

                    var user = (await connection.QueryAsync<PersonModel>(query,
                            new
                            {
                                Id = id
                            },
                            commandType: System.Data.CommandType.StoredProcedure))
                        .Select(row => new PersonModel(
                            row.Id,
                            row.FirstName,
                            row.LastName,
                            row.NationalCode,
                            row.MobileNumber))
                        .FirstOrDefault();

                    connection.Close();
                    return user;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<PersonModel?> GetPersonByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = @$"select * from [dbo].[Person] where NationalCode=@NationalCode";

                    connection.Open();
                    return connection.QueryFirstOrDefault<PersonModel>(query, new { NationalCode = nationalCode });
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<PersonModel> GetPersonByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPersonByMobileNumber]";

                    connection.Open();

                    var user = (await connection.QueryAsync<PersonModel>(query,
                            new
                            {
                                MobileNumber = mobileNumber
                            },
                            commandType: System.Data.CommandType.StoredProcedure))
                        .Select(row => new PersonModel(
                            row.Id,
                            row.FirstName,
                            row.LastName,
                            row.NationalCode,
                            row.MobileNumber))
                        .FirstOrDefault();

                    connection.Close();
                    return user;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<DataList<IEnumerable<PersonModel>>> ReadPersonAsync(int offset, int count, string firstName, string lastName, string nationalCode, string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                var queryResult = new DataList<IEnumerable<PersonModel>>();
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ReadPerson]";

                    connection.Open();

                    var result = await connection.QueryMultipleAsync(query,
                        new
                        {
                            MobileNumber = mobileNumber,
                            FirstName = firstName,
                            LastName = lastName,
                            NationalCode = nationalCode,
                            Skip = offset,
                            Fetch = count
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    var persons = result.Read<PersonModel>()
                        .Select(row => new PersonModel(
                            row.Id,
                            row.FirstName,
                            row.LastName,
                            row.NationalCode,
                            row.MobileNumber))
                        .ToList();


                    var counts = result.Read<int>().FirstOrDefault();

                    queryResult.Data = persons;
                    queryResult.RecordsFiltered = counts;
                    queryResult.RecordsTotal = counts;

                    connection.Close();

                    return queryResult;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<UserModel> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPanelUserById]";

                    connection.Open();

                    var user = (await connection.QueryAsync<UserModel>(query,
                            new
                            {
                                Id = id
                            },
                            commandType: System.Data.CommandType.StoredProcedure))
                        .Select(row => new UserModel(
                            row.Id,
                            row.PersonId,
                            row.FirstName,
                            row.LastName,
                            row.MobileNumber,
                            row.UserName,
                            row.CreationTime,
                            row.NationalCode,
                            row.AvatarName,
                            row.IsActive,
                            email: row.Email)).FirstOrDefault();

                    connection.Close();
                    return user;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<UserModel> GetByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetUserByMobileNumber]";

                    connection.Open();

                    var user = (await connection.QueryAsync<UserModel>(query,
                            new
                            {
                                MobileNumber = mobileNumber
                            },
                            commandType: System.Data.CommandType.StoredProcedure))
                        .Select(row => new UserModel(
                            row.Id,
                            row.PersonId,
                            row.FirstName,
                            row.LastName,
                            row.MobileNumber,
                            row.UserName,
                            row.CreationTime,
                            row.NationalCode,
                            row.AvatarName,
                            row.IsActive,
                            row.IsVerified,
                            invitedCode: row.InvitedCode,
                            myInviteCode: row.MyInviteCode,
                            cityId: row.CityId)
                        {
                            BankCustomerNumber = row.BankCustomerNumber,
                            BirthDate = row.BirthDate,

                        })
                        .FirstOrDefault();

                    connection.Close();

                    return user;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<UserModel> GetPanelUserByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                if (string.IsNullOrWhiteSpace(mobileNumber))
                    return null;

                if (!mobileNumber.All(char.IsDigit))
                    return null;

                var userRow = await
                   (
                       from u in _applicationDbContext.PanelUser.AsNoTracking()
                       join p in _applicationDbContext.People.AsNoTracking()
                           on u.PersonId equals p.Id
                       join pd in _applicationDbContext.PersonDetail.AsNoTracking()
                           on p.Id equals pd.PersonId into pds
                       from pd in pds.DefaultIfEmpty()
                       where p.MobileNumber == mobileNumber
                       select new
                       {
                           PanelUserId = u.Id,
                           u.PersonId,
                           p.FirstName,
                           p.LastName,
                           p.MobileNumber,
                           u.UserName,
                           u.CreationTime,
                           p.NationalCode,
                           AvatarName = pd != null ? pd.AvatarName : null,
                           u.IsActive,
                           p.IsVerified,
                           Password = ***;

                if (userRow == null)
                    return null;

                var user = new UserModel(
                    userRow.PanelUserId,
                    userRow.PersonId,
                    userRow.FirstName,
                    userRow.LastName,
                    userRow.MobileNumber,
                    userRow.UserName,
                    userRow.CreationTime,
                    userRow.NationalCode,
                    userRow.AvatarName,
                    userRow.IsActive,
                    userRow.IsVerified,
                    userRow.Password,
                    email: userRow.Email
                );

                var roleMenus = await
                   (
                       from r in _applicationDbContext.Role.AsNoTracking()
                       join ur in _applicationDbContext.PanelUserRole.AsNoTracking()
                           on r.Id equals ur.RoleId
                       join rpm in _applicationDbContext.RolePanelMenu.AsNoTracking()
                           on r.Id equals rpm.RoleId
                       join pm in _applicationDbContext.PanelMenu.AsNoTracking()
                           on rpm.PanelMenuId equals pm.Id
                       where ur.PanelUserId == userRow.PanelUserId
                             && r.IsActive
                             && pm.IsActive
                       select new RoleMenuModel(
                           r.Id,
                           r.IsActive,
                           r.PersianTitle,
                           r.Title,
                           pm.Url,
                           pm.Title,
                           pm.ParentId,
                           pm.IconName,
                           pm.ControllerName,
                           pm.ActionName,
                           pm.IsRootMenu,
                           pm.IsInternalMenu
                       )
                   ).ToListAsync(cancellationToken);

                user.SetRoleMenuModels(roleMenus);
                return user;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }



        public async Task<ProfileModel> GetProfileAsync(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetUserByMobileNumber]";

                    connection.Open();

                    var profile = await connection.QueryFirstOrDefaultAsync<ProfileModel>(query,
                        new
                        {
                            MobileNumber = mobileNumber
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();

                    return profile;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<MobileNumberFormatted> GetPersonMobileNumberByNationalCode(string nationalCode)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPersonMobileNumberByNationalCode]";

                    connection.Open();

                    var profile = await connection.QueryFirstOrDefaultAsync<MobileNumberFormatted>(query,
                        new
                        {
                            NationalCode = nationalCode
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();

                    return profile;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp.Message);
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> CheckNationalCodeTryCount(string nationalCode)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[CheckNationalCodeTryCount]";

                    connection.Open();

                    var count = await connection.QueryFirstOrDefaultAsync<int?>(query,
                        new
                        {
                            NationalCode = nationalCode
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    if (count != null && count > _checkNationalCodeTryCount)
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp.Message);
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<int> UpdateProfileAsync(ProfileModel profile, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[UpdateProfile]";
                    connection.Open();
                    DateTime? birthDay = null;
                    var result = await connection.QueryFirstOrDefaultAsync<int>(query,
                        new
                        {
                            profile.PersonId,
                            profile.NationalCode,
                            profile.FirstName,
                            profile.LastName,
                            profile.CityId,
                            profile.Address,
                            profile.MobileNumber,
                            profile.Gender,
                            DateOfBirth = profile.Birthday.HasValue ? profile.Birthday.Value.ToDateTime() : birthDay,
                            profile.MyInviteCode
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return result;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<int> UpdatePersonVerified(int personId, bool isVerified, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[UpdatePersonVerified]";

                    connection.Open();
                    var result = await connection.QueryFirstOrDefaultAsync<int>(query,
                        new
                        {
                            personId,
                            isVerified
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return result;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> UpdateAvatarAsync(string userName, string mobileNumber, string fileName, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[UpdateProfileAvatar]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            UserName = userName,
                            MobileNumber = mobileNumber,
                            AvatarName = fileName
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> UpdaterKvcAsync(string mobileNumber, bool? nationalCard, bool? signImage, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[UpdateKyc]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            MobileNumber = mobileNumber,
                            NationalCard = nationalCard,
                            SignImage = signImage
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<int> AddPanelUserAsync(UserModel user, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddPanelUser]";

                    connection.Open();

                    var userId = await connection.QueryFirstOrDefaultAsync<int>(query,
                        new
                        {
                            user.PersonId,
                            user.UserName,
                            PasswordHash = user.Password,
                            user.Email,
                            user.IsActive
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return userId;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> UpdatePanelUserAsync(UserModel user, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[UpdatePanelUser]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            user.Id,
                            user.PersonId,
                            user.NationalCode,
                            user.FirstName,
                            user.LastName,
                            user.CityId,
                            user.Address,
                            user.Email,
                            user.IsActive
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<UserModel> CheckPanelUserAsync(string userName, string password, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                //password = ***;

                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPanelUserByUserName]";

                    connection.Open();

                    var result = await connection.QueryMultipleAsync(query,
                        new
                        {
                            UserName = userName
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    var user = result.Read<UserModel>()
                        .Select(row => new UserModel(
                            row.Id,
                            row.PersonId,
                            row.FirstName,
                            row.LastName,
                            row.MobileNumber,
                            row.UserName,
                            row.CreationTime,
                            row.NationalCode,
                            row.AvatarName,
                            row.IsActive,
                            row.IsVerified,
                            row.Password,
                            email: row.Email))
                        .FirstOrDefault();

                    if (user != null)
                    {
                        var roleMenuUrls = result.Read<RoleMenuModel>()
                            .Select(row => new RoleMenuModel(
                                row.RoleId,
                                row.RoleIsActive,
                                row.RolePersianTitle,
                                row.RoleTitle,
                                row.MenuUrl,
                                row.MenuTitle,
                                row.MenuParentId,
                                row.MenuIconName,
                                row.MenuControllerName,
                                row.MenuActionName,
                                row.IsRootMenu,
                                row.IsInternalMenu))
                            .ToList();

                        user.SetRoleMenuModels(roleMenuUrls);
                    }

                    connection.Close();

                    if (user != null && OmidBank.Common.Helper.Hashing.Cryptography.Verify(password, user.Password))
                    {
                        return user;
                    }

                    return null;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<UserModel> GetPanelUserByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPanelUserByUserName]";

                    connection.Open();

                    var result = await connection.QueryMultipleAsync(query,
                        new
                        {
                            UserName = userName
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    var user = result.Read<UserModel>()
                        .Select(row => new UserModel(
                            row.Id,
                            row.PersonId,
                            row.FirstName,
                            row.LastName,
                            row.MobileNumber,
                            row.UserName,
                            row.CreationTime,
                            row.NationalCode,
                            row.AvatarName,
                            row.IsActive,
                            row.IsVerified,
                            row.Password,
                            email: row.Email))
                        .FirstOrDefault();

                    if (user != null)
                    {
                        var roleMenuUrls = result.Read<RoleMenuModel>()
                            .Select(row => new RoleMenuModel(
                                row.RoleId,
                                row.RoleIsActive,
                                row.RolePersianTitle,
                                row.RoleTitle,
                                row.MenuUrl,
                                row.MenuTitle,
                                row.MenuParentId,
                                row.MenuIconName,
                                row.MenuControllerName,
                                row.MenuActionName,
                                row.IsRootMenu,
                                row.IsInternalMenu))
                            .ToList();

                        user.SetRoleMenuModels(roleMenuUrls);
                    }

                    connection.Close();

                    return user;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<int> CheckInvitedCodeAsync(string mobileNumber, string invitedCode, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[CheckInvitedCode]";

                    connection.Open();

                    var result = await connection.QueryFirstOrDefaultAsync<int>(query,
                        new
                        {
                            MobileNumber = mobileNumber,
                            InvitedCode = invitedCode
                        },
                        commandType: System.Data.CommandType.StoredProcedure);
                    connection.Close();

                    return result;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return -1;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> UpdateInvitedCodeAsync(int personId, string invitedCode, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[UpdateInvitedCode]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            PersonId = personId,
                            InvitedCode = invitedCode
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> UpdateMyInviteCodeAsync(int id, string myInviteCode, CancellationToken cancellationToken)
        {
            var userEntity = await _entityContext.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
            if (userEntity == null)
                return false;
            userEntity.MyInviteCode = myInviteCode;
            return true;
        }

        public async Task<DataList<IEnumerable<UserModel>>> PanelReadAsync(int offset, int count, string firstName, string lastName,
            string nationalCode, string mobileNumber, string userName, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                var queryResult = new DataList<IEnumerable<UserModel>>();
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ReadPanelUser]";

                    connection.Open();

                    var result = await connection.QueryMultipleAsync(query,
                        new
                        {
                            UserName = userName,
                            MobileNumber = mobileNumber,
                            FirstName = firstName,
                            LastName = lastName,
                            NationalCode = nationalCode,
                            Skip = offset,
                            Fetch = count
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    var users = result.Read<UserModel>()
                        .Select(row => new UserModel(
                            row.Id,
                            row.PersonId,
                            row.FirstName,
                            row.LastName,
                            row.MobileNumber,
                            row.UserName,
                            row.CreationTime,
                            row.NationalCode,
                            row.AvatarName,
                            row.IsActive,
                            email: row.Email))
                        .ToList();


                    var counts = result.Read<int>().FirstOrDefault();

                    queryResult.Data = users;
                    queryResult.RecordsFiltered = counts;
                    queryResult.RecordsTotal = counts;

                    connection.Close();

                    return queryResult;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<DataList<IEnumerable<UserModel>>> AppReadAsync(int offset, int count, string firstName, string lastName,
            string nationalCode, string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                var queryResult = new DataList<IEnumerable<UserModel>>();
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ReadAppUser]";

                    connection.Open();

                    var result = await connection.QueryMultipleAsync(query,
                        new
                        {
                            MobileNumber = mobileNumber,
                            FirstName = firstName,
                            LastName = lastName,
                            NationalCode = nationalCode,
                            Skip = offset,
                            Fetch = count
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    var users = result.Read<UserModel>()
                        .Select(row => new UserModel(
                            row.Id,
                            row.PersonId,
                            row.FirstName,
                            row.LastName,
                            row.MobileNumber,
                            row.UserName,
                            row.CreationTime,
                            row.NationalCode,
                            row.AvatarName,
                            row.IsActive,
                            email: row.Email))
                        .ToList();


                    var counts = result.Read<int>().FirstOrDefault();

                    queryResult.Data = users;
                    queryResult.RecordsFiltered = counts;
                    queryResult.RecordsTotal = counts;

                    connection.Close();

                    return queryResult;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<string> IsValidUserAsync(string nationalCode, string mobileNumber, string email, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[IsValidPanelUser]";

                    connection.Open();

                    var userName = await connection.QueryFirstOrDefaultAsync<string>(query,
                        new
                        {
                            MobileNumber = mobileNumber,
                            NationalCode = nationalCode,
                            Email = email
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return userName;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<string> IsValidAppUserAsync(string nationalCode, string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[IsValidAppUser]";

                    connection.Open();

                    var userName = await connection.QueryFirstOrDefaultAsync<string>(query,
                        new
                        {
                            MobileNumber = mobileNumber,
                            NationalCode = nationalCode,
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return userName;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> ResetPasswordAsync(string userName, string newPassword, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                newPassword = ***;
                //newPassword = ***;
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ResetPassword]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            UserName = userName,
                            NewPassword = ***;

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> AddApplicationUserAsync(string mobileNumber, string invitedCode, string firebaseKey, string myInviteCode, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                var person = new PersonModel(mobileNumber);
                var personStatus = await AddPersonAsync(person, cancellationToken);
                if (personStatus == null)
                    throw new Exception("Add Person Exception");

                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddUser]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            personStatus.PersonId,
                            InvitedCode = invitedCode,
                            firebaseKey,
                            myInviteCode
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> AddApplicationUserAsync(int personId, string invitedCode, string firebaseKey, string myInviteCode, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddUser]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            PersonId = personId,
                            InvitedCode = invitedCode,
                            firebaseKey,
                            myInviteCode,
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> AddUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddPanelUserRole]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            UserId = userId,
                            RoleId = roleId
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> RemoveUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[DeletePanelUserRole]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            UserId = userId,
                            RoleId = roleId
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> AddOrganizationAsync(int userId, int organizationId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddUserOrganization]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            UserId = userId,
                            OrganizationId = organizationId
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> RemoveOrganizationAsync(int userId, int organizationId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[DeleteUserOrganization]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            UserId = userId,
                            OrganizationId = organizationId
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<DataList<IEnumerable<UserOrganizationModel>>> ReadUserOrganizationAsync(int offset, int count, int organizationId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                var queryResult = new DataList<IEnumerable<UserOrganizationModel>>();
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ReadUserOrganization]";

                    connection.Open();

                    var result = await connection.QueryMultipleAsync(query,
                        new
                        {
                            OrganizationId = organizationId,
                            Skip = offset,
                            Fetch = count
                        },
                        commandType: System.Data.CommandType.StoredProcedure);


                    var userOrganizations = result.Read<UserOrganizationModel>()
                        .Select(row => new UserOrganizationModel(
                            row.Id,
                            row.UserId,
                            row.OrganizationId,
                            row.FirstName,
                            row.LastName,
                            row.NationalCode,
                            row.MobileNumber
                        ))
                        .ToList();

                    var counts = result.Read<int>().FirstOrDefault();

                    queryResult.Data = userOrganizations;
                    queryResult.RecordsFiltered = counts;
                    queryResult.RecordsTotal = counts;

                    connection.Close();

                    return queryResult;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> ChangePasswordAsync(string userName, string oldPassword, string newPassword, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                oldPassword = ***;
                newPassword = ***;

                //oldPassword = ***;
                //newPassword = ***;

                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ChangePassword]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            UserName = userName,
                            OldPassword = ***;

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> SetReadStatusMessage(int messageId, int userId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[SetReadStatusMessage]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            MessageId = messageId,
                            UserId = userId
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> DeleteUserMessageAsync(int messageId, int userId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[DeleteUserMessage]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            MessageId = messageId,
                            UserId = userId
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<DataList<IEnumerable<ReadUserMessagesModel>>> ReadUserMessageAsync(int offset, int count, int userId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                var queryResult = new DataList<IEnumerable<ReadUserMessagesModel>>();
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ReadUserMessage]";

                    connection.Open();

                    var result = await connection.QueryMultipleAsync(query,
                        new
                        {
                            UserId = userId,
                            Skip = offset,
                            Fetch = count
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    var messages = result.Read<ReadUserMessagesModel>().ToList();

                    var counts = result.Read<int>().FirstOrDefault();

                    queryResult.Data = messages;
                    queryResult.RecordsFiltered = counts;
                    queryResult.RecordsTotal = counts;

                    connection.Close();

                    return queryResult;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<DataList<IEnumerable<UserMessageModel>>> PanelReadUserMessageAsync(int offset, int count, int messageId, byte? type, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                var queryResult = new DataList<IEnumerable<UserMessageModel>>();
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ReadPanelUserMessage]";

                    connection.Open();

                    var result = await connection.QueryMultipleAsync(query,
                        new
                        {
                            Type = type,
                            MessageId = messageId,
                            Skip = offset,
                            Fetch = count
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    var messages = result.Read<UserMessageModel>()
                        .Select(row => new UserMessageModel(
                            row.MessageId,
                            row.Read
                        ))
                        .ToList();

                    var counts = result.Read<int>().FirstOrDefault();

                    queryResult.Data = messages;
                    queryResult.RecordsFiltered = counts;
                    queryResult.RecordsTotal = counts;

                    connection.Close();

                    return queryResult;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> AddUserMessageAsync(int messageId, string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddUserMessage]";

                    connection.Open();

                    var dbResult = await connection.QueryFirstOrDefaultAsync<int>(query,
                        new
                        {
                            MessageId = messageId,
                            MobileNumber = mobileNumber
                        },
                        commandType: System.Data.CommandType.StoredProcedure);
                    connection.Close();
                    if (dbResult == 1)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task UpdateUserFirebaseKey(int userId, string firebaseKey, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[UpdateUserFirebaseKey]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                        new
                        {
                            userId,
                            firebaseKey
                        },
                        commandType: System.Data.CommandType.StoredProcedure);
                    connection.Close();
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<FirebaseResultModel> InquiryFireBaseToken(int userId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[InquiryFireBaseToken]";

                    connection.Open();

                    var dbResult = await connection.QueryFirstOrDefaultAsync<FirebaseResultModel>(query,
                        new
                        {
                            userId
                        },
                        commandType: System.Data.CommandType.StoredProcedure);
                    connection.Close();

                    return dbResult;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<int> ReadUserMessageCount(int userId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ReadUserMessageCount]";

                    connection.Open();

                    var count = await connection.QueryFirstOrDefaultAsync<int>(query,
                        new
                        {
                            UserId = userId
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();

                    return count;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return 0;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<string> GeneratePersonOpenAccountModel(GeneratePersonOpenAccountModel model, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GeneratePersonOpenAccount]";

                    connection.Open();

                    var uId = await connection.QueryFirstOrDefaultAsync<Guid?>(query,
                        new
                        {
                            MobileNumber = model.MobileNumber,
                            NationalCode = model.NationalCode
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    if (uId != null)
                        return uId.ToString();
                    else
                        return null;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(exp.Message);
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<object> GetPersonDetail(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                PersonDetailModel creditApplyItem = null;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    creditApplyItem = connection.Query<PersonDetailModel>("SELECT  [Id] FROM [User] where PersonId=@Id", new { Id = id }).FirstOrDefault();
                    connection.Close();
                }

                return creditApplyItem;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return default;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> UpdateFierbaseToken(string MobilNumber, string FierbaseToken)
        {
            SqlConnection connection = null;
            try
            {
                string sql = "[dbo].[UpdateFierbaseToken]";
                using (connection = new SqlConnection(_connectionString))
                {
                    try
                    {
                        connection.Open();
                        connection.ExecuteScalar(sql, new
                        {
                            MobilNumber,
                            FierbaseToken
                        }, commandType: System.Data.CommandType.StoredProcedure);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return default;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> AddUserBranch(UserBranchModel model, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddUserBranchs]";

                    connection.Open();

                    var userId = await connection.QueryFirstOrDefaultAsync<int>(query,
                        new
                        {
                            model.UserId,
                            model.BranchCode,
                            model.RegionId,
                            model.SupervisorType
                        },
                        commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();
                    return true;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<bool> ExistInvitedCodeAsync(string invitedCode, int? personId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ExistInvitedCode]";

                    connection.Open();

                    var invitedCodeStatus = (await connection.QueryFirstOrDefaultAsync<int>(query,
                            new
                            {
                                InvitedCode = invitedCode,
                                PersonId = personId
                            },
                            commandType: System.Data.CommandType.StoredProcedure));

                    connection.Close();
                    return invitedCodeStatus == 1 ? true : false;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<int?> GetPersonIdNationalCodeAsync(string nationalCode, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = @$"select Id from [dbo].[Person] where NationalCode=@NationalCode";

                    connection.Open();
                    return connection.QueryFirstOrDefault<int>(query, new { NationalCode = nationalCode });
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<long?> GetBankCustomerNumberAsync(string nationalCode, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = @$"select Id from [dbo].[Person] where NationalCode=@NationalCode";

                    connection.Open();
                    return connection.QueryFirstOrDefault<int>(query, new { NationalCode = nationalCode });
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<UserEntity?> FindById(int userId, CancellationToken cancellationToken)
            => await _entityContext.Include(a => a.Person).FirstOrDefaultAsync(a => a.Id == userId, cancellationToken);

        public async Task UpdateUserCustomerNumber(int userId, long customerNumber, CancellationToken cancellationToken)
        => await _entityContext.Where(a => a.Id == userId)
                       .ExecuteUpdateAsync(a =>
                    a.SetProperty(c => c.BankCustomerNumber, customerNumber),
                cancellationToken);

        public async Task<int> GetPersonIdAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _entityContext.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            return user.PersonId;
        }

        public async Task<PersonModel?> GetPersonDataWithInvitedCode(string invitedCode, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPersonDataWithInvitedCode]";

                    connection.Open();

                    var personData = (await connection.QueryFirstOrDefaultAsync<PersonModel>(query,
                            new
                            {
                                InvitedCode = invitedCode
                            },
                            commandType: System.Data.CommandType.StoredProcedure));

                    connection.Close();
                    return personData;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public async Task<List<UserEntity>> GetMyInvitedCode(string myInvitedCode, CancellationToken cancellationToken)
        {
            var persons = await _entityContext.Where(p => p.InvitedCode.Equals(myInvitedCode)).ToListAsync();
            return persons;
        }

        public async Task<UserEntity> FindByPersonId(int personId, CancellationToken cancellationToken)
            => await _entityContext.Include(a => a.Person).FirstOrDefaultAsync(a => a.Person.Id == personId, cancellationToken);

        public async Task<bool> IsActive(int userId, CancellationToken cancellationToken)
        {
            return await _entityContext.AnyAsync(a => a.Id == userId && a.IsActive, cancellationToken);
        }
        public async Task<PersonEntity> GetPersonByMobileNumber(string mobileNumber, CancellationToken cancellationToken)
        {
            return await _applicationDbContext.People.FirstOrDefaultAsync(x => x.MobileNumber == mobileNumber, cancellationToken);
        }
    }
}