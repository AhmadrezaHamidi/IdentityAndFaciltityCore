namespace IdentityService.Infrastructure.Repositories
{
    public class FileRepository : BaseRepository, IFileRepository
    {
        public readonly string _connectionString;
        public readonly ILogger _logger;

        public FileRepository(IConfiguration configuration, ILogger<FileRepository> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("IdentityConnection");
        }

        public FileModel GetFileItemById(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;

            try
            {
                FileModel fileItem = null;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    fileItem = connection.Query<FileModel>("Select Id,MobileNumber,Title, FileTypeId, FileUsageTypeId, Url, Name, NameWithExtension, Extension, Size, MimeType, IsDeleted, IsActive, ParentId, EntityId, EntityName, RegisterUserId, RegisterDate, DisplaySize, CdnNumber, UploadFileName, BaseFileName from [File] where isdeleted!=1 and id=@Id", new { Id = id }).FirstOrDefault();
                    connection.Close();
                }
                return fileItem;
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

        public FileModel GetFileItemByMobileNumber(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                FileModel fileItem = null;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    fileItem = connection.Query<FileModel>("Select Id,MobileNumber,Title, FileTypeId, FileUsageTypeId, Url, Name, NameWithExtension, Extension, Size, MimeType, IsDeleted, IsActive, ParentId, EntityId, EntityName, RegisterUserId, RegisterDate, DisplaySize, CdnNumber, UploadFileName, BaseFileName from [File] where isdeleted!=1 and MobileNumber=@MobileNumber", new { MobileNumber = mobileNumber }).FirstOrDefault();
                    connection.Close();
                }
                return fileItem;
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

        public List<FileModel> GetAllFileItem(CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                List<FileModel> fileItems = new List<FileModel>();
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    fileItems = connection.Query<FileModel>("Select Id,MobileNumber,Title, FileTypeId, FileUsageTypeId, Url, Name, NameWithExtension, Extension, Size, MimeType, IsDeleted, IsActive, ParentId, EntityId, EntityName, RegisterUserId, RegisterDate, DisplaySize, CdnNumber, UploadFileName, BaseFileName from [File] where isdeleted!=1").ToList();
                    connection.Close();
                }
                return fileItems;
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

        public int InsertFileItem(FileModel fileItem, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute("Insert into [File](MobileNumber,Title, FileTypeId, FileUsageTypeId, Url, Name, NameWithExtension, Extension, Size, MimeType, ParentId, EntityId, EntityName, RegisterUserId, DisplaySize, CdnNumber, UploadFileName, BaseFileName) values (@MobileNumber,@Title, @FileTypeId, @FileUsageTypeId, @Url, @Name, @NameWithExtension, @Extension, @Size, @MimeType, @ParentId, @EntityId, @EntityName, @RegisterUserId, @DisplaySize, @CdnNumber, @UploadFileName, @BaseFileName)",
                        new { fileItem.MobileNumber, fileItem.Title, fileItem.FileTypeId, fileItem.FileUsageTypeId, fileItem.Url, fileItem.Name, fileItem.NameWithExtension, fileItem.Extension, fileItem.Size, fileItem.MimeType, fileItem.ParentId, fileItem.EntityId, fileItem.EntityName, fileItem.RegisterUserId, fileItem.DisplaySize, fileItem.CdnNumber, fileItem.UploadFileName, fileItem.BaseFileName });
                    connection.Close();
                    return affectedRows;
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

        public int DeleteFileItem(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute("update [File] set isdeleted=1 Where Id = @Id", new { Id = id });
                    connection.Close();
                    return affectedRows;
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
    }
}
