using IdentityService.Domain.Models.MonitoringDashboard;

namespace IdentityService.Service.IService.MonitoringDashboard
{
    public interface IMonitoringDashboardService : IBaseService
    {
        List<SimpleReportViewModel> CreateBar(CancellationToken cancellationToken);
        List<CountChartClass> GetChartItem(CancellationToken cancellationToken);
        CountAllUserClass CountAllUser(CancellationToken cancellationToken);
        CountUserRegisterInCurrentDayClass CountUserRegisterInCurrentDay(CancellationToken cancellationToken);
        CountAllOnlineTransactionClass CountAllOnlineTransaction(CancellationToken cancellationToken);
        CountAllWalletTransactionClass CountAllWalletTransaction(CancellationToken cancellationToken);
        public CountAllChargeWalletTransactionClass CountAllChargeWalletTransaction(CancellationToken cancellationToken);
        public CountAllOnlineRejectTransactionClass CountAllOnlineRejectTransaction(CancellationToken cancellationToken);
        public CountAllWalletRejectTransactionClass CountAllWalletRejectTransaction(CancellationToken cancellationToken);
        public CountAllCardToCardInTodayClass CountAllCardToCardInToday(CancellationToken cancellationToken);
        public CountGetByIdCurrentDayClass CountGetByIdCurrentDay(CancellationToken cancellationToken);
        public CountAllGetByIdClass CountAllGetById(CancellationToken cancellationToken);
        public CountGetAddressClass CountGetAddress(CancellationToken cancellationToken);
        public SUMAllBalanceUserWalletClass SUMAllBalanceUserWallet(CancellationToken cancellationToken);

    }
}
