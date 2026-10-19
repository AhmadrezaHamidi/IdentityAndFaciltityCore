using Dapper;
using IdentityService.Domain.Models.MonitoringDashboard;
using IdentityService.Service.IService.MonitoringDashboard;
using IdentityService.Services.Services;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace IdentityService.Service.Service.MonitoringDashboard
{
    public class MonitoringDashboardService : BaseService, IMonitoringDashboardService
    {
        public readonly string _connectionStringIdentity;
        public readonly string _connectionStringTransaction;
        public readonly string _connectionStringWallet;
        public readonly string _connectionStringApiManager;
        private readonly ILogger<MonitoringDashboardService> _logger;

        private const string IdentityConnectionString = "IdentityConnection";
        private const string TransactionConnectionString = "TransactionConnection";
        private const string WalletConnectionString = "WalletConnection";
        private const string ApiManagerConnectionString = "ApiManagerConnection";

        private const string NotFound = "Not Found";
        private const string Zero = "0";
        private const string Separator = "/";

        private const string CountChartQuery = "[dbo].[ReadCountPersonLastWeek]";
        private const string CountAllUserQuery = "select COUNT(Id) as CountAllUser from Person where IsVerified = 1";
        private const string CountUserRegisterInCurrentDayQuery = "select COUNT(Id) as UserRegisterCurrentDay from Person where CreationTime < DateAdd(Day, DateDiff(Day, 0, GetDate())+1, 0) AND CreationTime >= DateAdd(Day, DateDiff(Day, 0, GetDate()), 0)";
        private const string CountAllOnlineTransactionQuery = "select COUNT(tr.Id) as AllOnlineTransaction from [Transaction] tr join [OnlineTransaction] ot on tr.Id = ot.TransactionId where ot.Status = 1";
        private const string CountAllWalletTransactionQuery = "select COUNT(tr.Id) as AllWalletTransaction from [Transaction] tr join [WalletTransaction] wt on tr.Id = wt.TransactionId where wt.Status = 1";
        private const string CountAllChargeWalletTransactionQuery = "select COUNT(tr.Id) as AllWalletTransaction from [Transaction] tr join [OnlineTransaction] wt on tr.Id = wt.TransactionId where tr.ServiceId = 5009";
        private const string CountAllOnlineRejectTransactionQuery = "select COUNT(tr.Id) as AllOnlineTransaction from [Transaction] tr join [OnlineTransaction] ot on tr.Id = ot.TransactionId where ot.Status = 0";
        private const string CountAllWalletRejectTransactionQuery = "select COUNT(Id) as CountGetAddress from [Address] where CreationTime < DateAdd(Day, DateDiff(Day, 0, GetDate())+1, 0) AND CreationTime >= DateAdd(Day, DateDiff(Day, 0, GetDate()), 0)";
        private const string CountAllCardToCardInTodayQuery = "select COUNT(Id) as AllCardToCard from TransactionNoPayment where CreationTime < DateAdd(Day, DateDiff(Day, 0, GetDate())+1, 0) AND CreationTime >= DateAdd(Day, DateDiff(Day, 0, GetDate()), 0) and Status = 1";
        private const string CountGetByIdCurrentDayQuery = "select COUNT(Id) as CountGetById from ActivityLog where Request ='/v1/Part/GetById' and CreationTime < DateAdd(Day, DateDiff(Day, 0, GetDate())+1, 0) AND CreationTime >= DateAdd(Day, DateDiff(Day, 0, GetDate()), 0)";
        private const string CountAllGetByIdQuery = "select COUNT(Id) as CountGetById from ActivityLog where Request ='/v1/Part/GetById'";
        private const string CountGetAddressQuery = "select COUNT(Id) as CountGetAddress from [Address]";
        private const string SUMAllBalanceUserWalletQuery = "select SUM(wb.Amount) as SumAmount from WalletBalance wb inner join Wallet w on wb.WalletId = w.Id where w.WalletTypeId <> 2";

        public MonitoringDashboardService(IConfiguration configuration, ILogger<MonitoringDashboardService> logger)
        {
            _connectionStringIdentity = configuration.GetConnectionString(IdentityConnectionString);
            _connectionStringTransaction = configuration.GetConnectionString(TransactionConnectionString);
            _connectionStringWallet = configuration.GetConnectionString(WalletConnectionString);
            _connectionStringApiManager = configuration.GetConnectionString(ApiManagerConnectionString);
            _logger = logger;
        }

        public List<SimpleReportViewModel> CreateBar(CancellationToken cancellationToken)
        {
            var res = GetChartItem(cancellationToken);

            var simpleReportList = new List<SimpleReportViewModel>();

            for (int i = 0; i <= 6; i++)
            {
                simpleReportList.Add(new SimpleReportViewModel()
                {
                    DimensionOne = res.ElementAtOrDefault(i) != null ? GetBarDimensionOne(DateTime.Parse(res[i].Dates)) : NotFound,
                    Quantity = res.ElementAtOrDefault(i) != null ? res[i].Count : Zero
                });
            }

            return simpleReportList;
        }

        public List<CountChartClass> GetChartItem(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountChartClass>(_connectionStringIdentity, CountChartQuery).ToList();
        }

        public CountAllUserClass CountAllUser(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountAllUserClass>(_connectionStringIdentity, CountAllUserQuery).FirstOrDefault();
        }

        public CountUserRegisterInCurrentDayClass CountUserRegisterInCurrentDay(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountUserRegisterInCurrentDayClass>(_connectionStringIdentity, CountUserRegisterInCurrentDayQuery).FirstOrDefault();
        }

        public CountAllOnlineTransactionClass CountAllOnlineTransaction(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountAllOnlineTransactionClass>(_connectionStringTransaction, CountAllOnlineTransactionQuery).FirstOrDefault();
        }

        public CountAllWalletTransactionClass CountAllWalletTransaction(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountAllWalletTransactionClass>(_connectionStringTransaction, CountAllWalletTransactionQuery).FirstOrDefault();
        }

        public CountAllChargeWalletTransactionClass CountAllChargeWalletTransaction(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountAllChargeWalletTransactionClass>(_connectionStringTransaction, CountAllChargeWalletTransactionQuery).FirstOrDefault();
        }

        public CountAllOnlineRejectTransactionClass CountAllOnlineRejectTransaction(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountAllOnlineRejectTransactionClass>(_connectionStringTransaction, CountAllOnlineRejectTransactionQuery).FirstOrDefault();
        }

        public CountAllWalletRejectTransactionClass CountAllWalletRejectTransaction(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountAllWalletRejectTransactionClass>(_connectionStringApiManager, CountAllWalletRejectTransactionQuery).FirstOrDefault();
        }

        public CountAllCardToCardInTodayClass CountAllCardToCardInToday(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountAllCardToCardInTodayClass>(_connectionStringTransaction, CountAllCardToCardInTodayQuery).FirstOrDefault();
        }

        public CountGetByIdCurrentDayClass CountGetByIdCurrentDay(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountGetByIdCurrentDayClass>(_connectionStringApiManager, CountGetByIdCurrentDayQuery).FirstOrDefault();
        }

        public CountAllGetByIdClass CountAllGetById(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountAllGetByIdClass>(_connectionStringApiManager, CountAllGetByIdQuery).FirstOrDefault();
        }

        public CountGetAddressClass CountGetAddress(CancellationToken cancellationToken)
        {
            return RequestToDatabase<CountGetAddressClass>(_connectionStringApiManager, CountGetAddressQuery).FirstOrDefault();
        }

        public SUMAllBalanceUserWalletClass SUMAllBalanceUserWallet(CancellationToken cancellationToken)
        {
            return RequestToDatabase<SUMAllBalanceUserWalletClass>(_connectionStringWallet, SUMAllBalanceUserWalletQuery).FirstOrDefault();
        }


        private static string GetBarDimensionOne(DateTime date)
        {
            var persianCalendar = new PersianCalendar();
            return persianCalendar.GetYear(date) + Separator + persianCalendar.GetMonth(date) + Separator + persianCalendar.GetDayOfMonth(date);
        }

        private IQueryable<T> RequestToDatabase<T>(string connectionString, string query) where T : class
        {
            try
            {
                IQueryable<T> Result;
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Result = connection.Query<T>(query).AsQueryable();
                    connection.Close();
                }
                return Result;

            }
            catch (Exception exp)
            {
                _logger.LogError("Error:" + exp);
                return null;
            }
        }
    }
}
