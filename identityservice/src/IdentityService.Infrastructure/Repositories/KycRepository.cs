namespace IdentityService.Infrastructure.Repositories
{
    public class KycRepository : BaseRepository, IKycRepository
    {
        public readonly string _connectionString;
        public readonly ILogger _logger;

        public KycRepository(IConfiguration configuration, ILogger<PanelMenuRepository> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("IdentityConnection");
        }

        public KycModel GetKycById(int id, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                KycModel kycItem = null;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    kycItem = connection.Query<KycModel>("Select Id,NationalCode,MobileNumber,PersonId,KycDate,NationalCodeSeries,VideoLink,Birthdate,AuthenticationState,AuthenticationResponseReason from Kyc where Id=@Id", new { Id = id }).FirstOrDefault();
                    connection.Close();
                }
                return kycItem;
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

        public KycModel GetKycByMobileNumber(string mobileNumber, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                KycModel kycItem = null;
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    kycItem = connection.Query<KycModel>("Select Id,NationalCode,MobileNumber,PersonId,KycDate,NationalCodeSeries,VideoLink,Birthdate,AuthenticationState,AuthenticationResponseReason from Kyc where MobileNumber=@MobileNumber", new { MobileNumber = mobileNumber }).FirstOrDefault();
                    connection.Close();
                }
                return kycItem;
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

        public int InsertKyc(KycModel kycItem, CancellationToken cancellationToken)
        {
            SqlConnection conn = null;
            SqlConnection connection = null;
            try
            {
                int yourValue = 0;
                string sql = "select Id from Person Where NationalCode = '" + kycItem.NationalCode + "'";
                using (conn = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    yourValue = (int)cmd.ExecuteScalar();
                }
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute("Insert into Kyc (NationalCode,MobileNumber,PersonId,KycDate,NationalCodeSeries,VideoLink,Birthdate,AuthenticationState,AuthenticationResponseReason) values (@NationalCode,@MobileNumber,@PersonId,@KycDate,@NationalCodeSeries,@VideoLink,@Birthdate,@AuthenticationState,@AuthenticationResponseReason)", new { kycItem.NationalCode, kycItem.MobileNumber, PersonId = yourValue, KycDate = DateTime.Now, kycItem.NationalCodeSeries, kycItem.VideoLink, Birthdate = kycItem.BirthdateDatetime, kycItem.AuthenticationState, AuthenticationResponseReason = "" });
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
                    conn.Close();
                    conn.Dispose();
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public int UpdateKyc(KycModel kycItem, CancellationToken cancellationToken)
        {
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var affectedRows = connection.Execute("Update Kyc set AuthenticationState=@AuthenticationState,AuthenticationResponseReason=@AuthenticationResponseReason Where NationalCode = @NationalCode", new { kycItem.NationalCode, kycItem.AuthenticationState, kycItem.AuthenticationResponseReason });
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
