using IdentityService.Infrastructure.Contexts;

namespace IdentityService.Infrastructure.Repositories
{
    public class PanelMenuRepository : BaseRepository, IPanelMenuRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly ApplicationDBContext _applicationDbContext;

        public PanelMenuRepository(IConfiguration configuration, ILogger<PanelMenuRepository> logger, ApplicationDBContext applicationDbContext)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("IdentityConnection");
            _applicationDbContext = applicationDbContext;
        }

        public async Task<bool> AddAsync(PanelMenuModel menu, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[AddPanelMenu]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                            new
                            {
                                menu.Title,
                                menu.ParentId,
                                menu.Url,
                                menu.IconName,
                                menu.ActionName,
                                menu.ControllerName,
                                menu.IsRootMenu,
                                menu.IsInternalMenu,
                                menu.IsActive,
                                menu.EnTitle
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

        public async Task<DataList<IEnumerable<PanelMenuModel>>> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await ReadAsync(0, 2000, null, cancellationToken);
            return result;
        }

        public async Task<PanelMenuModel> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPanelMenuById]";

                    connection.Open();

                    var role = (await connection.QueryAsync<PanelMenuModel>(query,
                            new
                            {
                                Id = id
                            },
                            commandType: System.Data.CommandType.StoredProcedure))
                            .Select(row => new PanelMenuModel(
                                row.Id,
                                row.ParentId,
                                row.Title,
                                row.ActionName,
                                row.ControllerName,
                                row.IsRootMenu,
                                row.IsInternalMenu,
                                row.IsActive,
                                row.Url,
                                row.IconName,
                                row.CreationTime,
                                row.EnTitle
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


        public async Task<IEnumerable<PanelMenuModel>> GetPanelRoleMenuAsyncv2(string userName, CancellationToken cancellationToken)
        {
            try
            {
                var menus = await
                   (
                       from u in _applicationDbContext.PanelUser.AsNoTracking()
                       join ur in _applicationDbContext.PanelUserRole.AsNoTracking()
                           on u.Id equals ur.PanelUserId
                       join r in _applicationDbContext.Role.AsNoTracking()
                           on ur.RoleId equals r.Id
                       join rpm in _applicationDbContext.RolePanelMenu.AsNoTracking()
                           on r.Id equals rpm.RoleId
                       join pm in _applicationDbContext.PanelMenu.AsNoTracking()
                           on rpm.PanelMenuId equals pm.Id
                       where u.UserName == userName
                             && u.IsActive
                             && r.IsActive
                             && pm.IsActive
                       select pm
                   )
                   .Distinct()
                   .ToListAsync(cancellationToken);

                return menus.Select(pm => new PanelMenuModel(
                    pm.Id,
                    pm.ParentId,
                    pm.Title,
                    pm.ActionName,
                    pm.ControllerName,
                    pm.IsRootMenu,
                    pm.IsInternalMenu,
                    pm.IsActive,
                    pm.Url,
                    pm.IconName,
                    pm.CreationTime,
                    pm.EnTitle)).ToList();
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }

        public async Task<IEnumerable<PanelMenuModel>> GetPanelRoleMenuAsync(string userName, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPanelRoleMenu]";

                    connection.Open();

                    var menus = (await connection.QueryAsync<PanelMenuModel>(query,
                            new
                            {
                                UserName = userName
                            },
                            commandType: System.Data.CommandType.StoredProcedure))
                            .Select(row => new PanelMenuModel(
                                row.Id,
                                row.ParentId,
                                row.Title,
                                row.ActionName,
                                row.ControllerName,
                                row.IsRootMenu,
                                row.IsInternalMenu,
                                row.IsActive,
                                row.Url,
                                row.IconName,
                                row.CreationTime,
                                row.EnTitle))
                            .ToList();

                    connection.Close();

                    return menus;
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

        public async Task<IEnumerable<PanelMenuModel>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetPanelRoleMenuByRoleId]";

                    connection.Open();

                    var menus = (await connection.QueryAsync<PanelMenuModel>(query,
                            new
                            {
                                RoleId = roleId
                            },
                            commandType: System.Data.CommandType.StoredProcedure))
                            .Select(row => new PanelMenuModel(
                                row.Id,
                                row.ParentId,
                                row.Title,
                                row.ActionName,
                                row.ControllerName,
                                row.IsRootMenu,
                                row.IsInternalMenu,
                                row.IsActive,
                                row.Url,
                                row.IconName,
                                row.CreationTime,
                                row.EnTitle))
                            .ToList();

                    connection.Close();

                    return menus;
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

        public async Task<DataList<IEnumerable<PanelMenuModel>>> ReadAsync(int offset, int count, string term, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                var queryResult = new DataList<IEnumerable<PanelMenuModel>>();
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[ReadPanelMenu]";

                    connection.Open();

                    var result = await connection.QueryMultipleAsync(query,
                            new
                            {
                                Title = term,
                                EnTitle = term,
                                Skip = offset,
                                Fetch = count

                            },
                            commandType: System.Data.CommandType.StoredProcedure);


                    var menus = result.Read<PanelMenuModel>()
                                         .Select(row => new PanelMenuModel(
                                             row.Id,
                                             row.ParentId,
                                             row.Title,
                                             row.ActionName,
                                             row.ControllerName,
                                             row.IsRootMenu,
                                             row.IsInternalMenu,
                                             row.IsActive,
                                             row.Url,
                                             row.IconName,
                                             row.CreationTime,
                                             row.EnTitle))
                                         .ToList();

                    var counts = result.Read<int>().FirstOrDefault();

                    queryResult.Data = menus;
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

        public async Task<bool> UpdateAsync(PanelMenuModel menu, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[UpdatePanelMenu]";

                    connection.Open();

                    await connection.ExecuteAsync(query,
                            new
                            {
                                menu.Id,
                                menu.Title,
                                menu.ParentId,
                                menu.Url,
                                menu.IconName,
                                menu.ActionName,
                                menu.ControllerName,
                                menu.IsRootMenu,
                                menu.IsInternalMenu,
                                menu.IsActive,
                                menu.EnTitle
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

        public async Task<List<PanelMenuRoleModel>> InquiryUserPanelMenu(long RoleId, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[InquiryUserPanelMenu]";

                    connection.Open();

                    var result = (await connection.QueryAsync<PanelMenuRoleModel>(query,
                            new
                            {
                                RoleId
                            },
                            commandType: System.Data.CommandType.StoredProcedure)).ToList();

                    connection.Close();

                    return result;
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
