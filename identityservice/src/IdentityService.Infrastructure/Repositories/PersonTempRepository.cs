namespace IdentityService.Infrastructure.Repositories
{
    public class PersonTempRepository : BaseRepository, IPersonTempRepository
    {
        public readonly string _connectionString;
        public readonly ILogger _logger;
        private readonly int _ipLimitationCount;

        public PersonTempRepository(IConfiguration configuration, ILogger<PanelMenuRepository> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("IdentityConnection");
            _ipLimitationCount = int.Parse(configuration["IplimitationCount"]);
        }

        public PersonTempModel GetPersonTempById(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                PersonTempModel personTemp = null;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    personTemp = connection.Query<PersonTempModel>("Select Id,MobileNumber,NationalCode,TryValidateByShahkarCount,LastValidateByShahkarDateTime,IsVerifiedByShahkar,VerifiedByShahkarDateTime,ActivationCode,SentActivateionCodesCount,LastSentActivationCodeDateTime,IsVerifiedActivationCode,VerifiedActivationCodeDateTime,PersonId,LastIpAddress,LastDeviceInfo,RegisterDate,TryValidateActivationCodeCount from PersonTemp where isdeleted!=1 and id=@Id", new { Id = id }).FirstOrDefault();
                    connection.Close();
                }
                return personTemp;
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

        public PersonTempModel GetPersonTempByMobileNumber(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                PersonTempModel personTemp = null;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    personTemp = connection.Query<PersonTempModel>("Select Id,MobileNumber,NationalCode,TryValidateByShahkarCount,LastValidateByShahkarDateTime,IsVerifiedByShahkar,VerifiedByShahkarDateTime,ActivationCode,SentActivateionCodesCount,LastSentActivationCodeDateTime,IsVerifiedActivationCode,VerifiedActivationCodeDateTime,PersonId,LastIpAddress,LastDeviceInfo,RegisterDate,TryValidateActivationCodeCount from PersonTemp where isdeleted!=1 and MobileNumber=@MobileNumber", new { MobileNumber = mobileNumber }).FirstOrDefault();
                    connection.Close();
                }
                return personTemp;
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
        public async Task<IEnumerable<PersonTempModel>> GetUserLoginReportAsync(string nationalCode, string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection _connection = null;
            try
            {
                using (_connection = new SqlConnection(_connectionString))
                {
                    var query = "[dbo].[GetUserLoginReport]";

                    _connection.Open();

                    var result = (await _connection.QueryAsync<PersonTempModel>(query,
                            new
                            {
                                MobileNumber = mobileNumber,
                                NationalCode = nationalCode
                            },
                            commandType: System.Data.CommandType.StoredProcedure)).ToList();
                    _connection.Close();
                    return result;
                }
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return null;
            }
        }

        public bool GetPersonTempByIpAddress(string ip, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                int count;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    count = (int)connection.ExecuteScalar("SELECT COUNT(*) from PersonTemp where isdeleted!=1 and LastIpAddress=@LastIpAddress and cast( LastSentActivationCodeDateTime as date ) = cast( getdate() as date )", new { LastIpAddress = ip });
                    connection.Close();
                }
                if (count >= _ipLimitationCount)
                    return true;
                return false;
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

        public bool GetPersonTempRegisterCountByIpAddress(string ip, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                int count;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    count = (int)connection.ExecuteScalar("SELECT COUNT(*) from PersonTemp where isdeleted!=1 and LastIpAddress=@LastIpAddress and cast( RegisterDate as date ) = cast( getdate() as date )", new { LastIpAddress = ip });
                    connection.Close();
                }
                if (count >= _ipLimitationCount)
                    return true;
                return false;
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

        public bool GetTestUser(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                int count;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    count = (int)connection.ExecuteScalar("SELECT COUNT(*)  FROM TestUsers where mobileNumber=@MobileNumber", new { MobileNumber = mobileNumber });
                    connection.Close();
                }
                if (count == 1)
                    return true;
                return false;
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

        public List<PersonTempModel> GetAllPersonTemp(CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                List<PersonTempModel> personTemps = new List<PersonTempModel>();
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    personTemps = connection.Query<PersonTempModel>("Select Id,MobileNumber,NationalCode,TryValidateByShahkarCount,LastValidateByShahkarDateTime,IsVerifiedByShahkar,VerifiedByShahkarDateTime,ActivationCode,SentActivateionCodesCount,LastSentActivationCodeDateTime,IsVerifiedActivationCode,VerifiedActivationCodeDateTime,PersonId,LastIpAddress,LastDeviceInfo,RegisterDate,TryValidateActivationCodeCount from PersonTemp where isdeleted!=1").ToList();
                    connection.Close();
                }
                return personTemps;
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

        public int InsertPersonTemp(PersonTempModel personTemp, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute("Insert into PersonTemp (MobileNumber,NationalCode,TryValidateByShahkarCount,LastValidateByShahkarDateTime,IsVerifiedByShahkar,VerifiedByShahkarDateTime,ActivationCode,SentActivateionCodesCount,LastSentActivationCodeDateTime,IsVerifiedActivationCode,VerifiedActivationCodeDateTime,PersonId,LastIpAddress,LastDeviceInfo,InvitedCode) values (@MobileNumber,@NationalCode,@TryValidateByShahkarCount,@LastValidateByShahkarDateTime,@IsVerifiedByShahkar,@VerifiedByShahkarDateTime,@ActivationCode,@SentActivateionCodesCount,@LastSentActivationCodeDateTime,@IsVerifiedActivationCode,@VerifiedActivationCodeDateTime,@PersonId,@LastIpAddress,@LastDeviceInfo,@InvitedCode)", new { personTemp.MobileNumber, personTemp.NationalCode, personTemp.TryValidateByShahkarCount, personTemp.LastValidateByShahkarDateTime, personTemp.IsVerifiedByShahkar, personTemp.VerifiedByShahkarDateTime, personTemp.ActivationCode, personTemp.SentActivateionCodesCount, personTemp.LastSentActivationCodeDateTime, personTemp.IsVerifiedActivationCode, personTemp.VerifiedActivationCodeDateTime, personTemp.PersonId, personTemp.LastIpAddress, personTemp.LastDeviceInfo, personTemp.InvitedCode });
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

        public int UpdatePersonTemp(PersonTempModel personTemp, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                string query;
                int affectedRows = 0;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    if (personTemp.LastIpAddress != "::1")
                    {
                        query = "Update PersonTemp set MobileNumber = @MobileNumber,NationalCode = @NationalCode,TryValidateByShahkarCount = @TryValidateByShahkarCount,LastValidateByShahkarDateTime = @LastValidateByShahkarDateTime,IsVerifiedByShahkar = @IsVerifiedByShahkar,VerifiedByShahkarDateTime = @VerifiedByShahkarDateTime,ActivationCode = @ActivationCode,SentActivateionCodesCount = @SentActivateionCodesCount,LastSentActivationCodeDateTime = @LastSentActivationCodeDateTime,IsVerifiedActivationCode = @IsVerifiedActivationCode,VerifiedActivationCodeDateTime = @VerifiedActivationCodeDateTime,PersonId = @PersonId,LastIpAddress = @LastIpAddress,LastDeviceInfo = @LastDeviceInfo,TryValidateActivationCodeCount=@TryValidateActivationCodeCount Where Id = @Id";
                        affectedRows = connection.Execute(query, new { personTemp.Id, personTemp.MobileNumber, personTemp.NationalCode, personTemp.TryValidateByShahkarCount, personTemp.LastValidateByShahkarDateTime, personTemp.IsVerifiedByShahkar, personTemp.VerifiedByShahkarDateTime, personTemp.ActivationCode, personTemp.SentActivateionCodesCount, personTemp.LastSentActivationCodeDateTime, personTemp.IsVerifiedActivationCode, personTemp.VerifiedActivationCodeDateTime, personTemp.PersonId, personTemp.LastIpAddress, personTemp.LastDeviceInfo, personTemp.TryValidateActivationCodeCount });
                    }

                    else
                    {
                        query = "Update PersonTemp set MobileNumber = @MobileNumber,NationalCode = @NationalCode,TryValidateByShahkarCount = @TryValidateByShahkarCount,LastValidateByShahkarDateTime = @LastValidateByShahkarDateTime,IsVerifiedByShahkar = @IsVerifiedByShahkar,VerifiedByShahkarDateTime = @VerifiedByShahkarDateTime,ActivationCode = @ActivationCode,SentActivateionCodesCount = @SentActivateionCodesCount,LastSentActivationCodeDateTime = @LastSentActivationCodeDateTime,IsVerifiedActivationCode = @IsVerifiedActivationCode,VerifiedActivationCodeDateTime = @VerifiedActivationCodeDateTime,PersonId = @PersonId,LastDeviceInfo = @LastDeviceInfo,TryValidateActivationCodeCount=@TryValidateActivationCodeCount Where Id = @Id ";
                        affectedRows = connection.Execute(query, new { personTemp.Id, personTemp.MobileNumber, personTemp.NationalCode, personTemp.TryValidateByShahkarCount, personTemp.LastValidateByShahkarDateTime, personTemp.IsVerifiedByShahkar, personTemp.VerifiedByShahkarDateTime, personTemp.ActivationCode, personTemp.SentActivateionCodesCount, personTemp.LastSentActivationCodeDateTime, personTemp.IsVerifiedActivationCode, personTemp.VerifiedActivationCodeDateTime, personTemp.PersonId, personTemp.LastDeviceInfo, personTemp.TryValidateActivationCodeCount });
                    }
                    // var affectedRows = connection.Execute(query, new { personTemp.Id, personTemp.MobileNumber, personTemp.NationalCode, personTemp.TryValidateByShahkarCount, personTemp.LastValidateByShahkarDateTime, personTemp.IsVerifiedByShahkar, personTemp.VerifiedByShahkarDateTime, personTemp.ActivationCode, personTemp.SentActivateionCodesCount, personTemp.LastSentActivationCodeDateTime, personTemp.IsVerifiedActivationCode, personTemp.VerifiedActivationCodeDateTime, personTemp.PersonId, personTemp.LastIpAddress, personTemp.LastDeviceInfo, personTemp.TryValidateActivationCodeCount });
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

        public int DeletePersonTemp(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute("update PersonTemp set isdeleted=1 Where Id = @Id", new { Id = id });
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

        public PersonTempModel GetLastPersonTempByMobileNumber(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                PersonTempModel personTemp = null;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    personTemp = connection.Query<PersonTempModel>("Select TOP 1 Id,MobileNumber,NationalCode,TryValidateByShahkarCount,LastValidateByShahkarDateTime,IsVerifiedByShahkar,VerifiedByShahkarDateTime,ActivationCode,SentActivateionCodesCount,LastSentActivationCodeDateTime,IsVerifiedActivationCode,VerifiedActivationCodeDateTime,PersonId,LastIpAddress,LastDeviceInfo,RegisterDate,TryValidateActivationCodeCount from PersonTemp where isdeleted!=1 and MobileNumber=@MobileNumber order by LastSentActivationCodeDateTime desc", new { MobileNumber = mobileNumber }).FirstOrDefault();
                    connection.Close();
                }
                return personTemp;
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

        public async Task<PersonTempModel> GetPersonTempInfo(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                PersonTempModel personTemp = null;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    personTemp = connection.Query<PersonTempModel>("Select Id,MobileNumber,NationalCode,TryValidateByShahkarCount,LastValidateByShahkarDateTime,IsVerifiedByShahkar,VerifiedByShahkarDateTime,ActivationCode,SentActivateionCodesCount,LastSentActivationCodeDateTime,IsVerifiedActivationCode,VerifiedActivationCodeDateTime,PersonId,LastIpAddress,LastDeviceInfo,RegisterDate,TryValidateActivationCodeCount from PersonTemp where isdeleted!=1 and MobileNumber=@MobileNumber", new { MobileNumber = mobileNumber }).FirstOrDefault();
                    connection.Close();
                }
                return personTemp;
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

        public async Task UpdateApplicationVersion(int id, string applicationVersion, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute($"update PersonTemp set applicationversion={applicationVersion} Where Id = @Id", new { Id = id });
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
    }
}
