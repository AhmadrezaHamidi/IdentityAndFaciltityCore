using IdentityService.Domian.DTO;
using static IdentityService.Domain.IRepositories.IRoleRepository;

namespace IdentityService.Infrastructure.Repositories
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        private readonly ILogger<RoleRepository> _logger;
        private readonly string _connectionString;

        public RoleRepository(IConfiguration configuration, ILogger<RoleRepository> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("IdentityConnection");
        }

        
        public async Task<Result> AddAsync(RoleModel role, CancellationToken cancellationToken)
        {
            Result result= new Result();

            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddRole]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                            new
                            {
                                role.Title,
                                role.PersianTitle,
                                role.IsActive
                            },
                            commandType: System.Data.CommandType.StoredProcedure);

                    connection.Close();                    
                    result.Data = true;                    
                    return result;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                result.Message = exp.Message;
                result.Data = false;
                return result; 
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

        public async Task<IEnumerable<RoleModel>> GetAllAsync(CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetAllRole]";

                    connection.Open();

                    var roles = (await connection.QueryAsync<RoleModel>(query,
                            commandType: System.Data.CommandType.StoredProcedure))
                            .Select(row => new RoleModel(
                                row.Id,
                                row.Title,
                                row.PersianTitle,
                                row.IsActive
                                )).ToList();

                    connection.Close();

                    return roles;
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

        public async Task<RoleModel> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetRoleById]";

                    connection.Open();

                    var role = (await connection.QueryAsync<RoleModel>(query,
                            new
                            {
                                Id = id
                            },
                            commandType: System.Data.CommandType.StoredProcedure))
                            .Select(row => new RoleModel(
                                row.Id,
                                row.Title,
                                row.PersianTitle,
                                row.IsActive
                                )).FirstOrDefault();

                    connection.Close();
                    return role;
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

        public async Task<bool> UpdateAsync(RoleModel role, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[UpdateRole]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                            new
                            {
                                role.Id,
                                role.Title,
                                role.PersianTitle,
                                role.IsActive
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

        public async Task<bool> AddRoleMenuAsync(int roleId, int menuId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddRolePanelMenu]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                            new
                            {
                                RoleId = roleId,
                                PanelMenuId = menuId
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

        public async Task<bool> RemoveRoleMenuAsync(int roleId, int menuId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[DeletePanelRoleMenu]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                            new
                            {
                                RoleId = roleId,
                                PanelMenuId = menuId
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

        public async Task<IEnumerable<RoleModel>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPanelUserRoleByUserId]";

                    connection.Open();

                    var roles = (await connection.QueryAsync<RoleModel>(query,
                            new
                            {
                                UserId = userId
                            },
                            commandType: System.Data.CommandType.StoredProcedure))
                            .Select(row => new RoleModel(
                                row.Id,
                                row.Title,
                                row.PersianTitle,
                                row.IsActive
                                )).ToList();

                    connection.Close();

                    return roles;
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
    }
}
