using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IdentityService.Infrastructure.Repositories
{
    public class ActiveTokenRepository : BaseRepository, IActiveTokenRepository
    {
        private readonly IMongoDBContext _identityContext;
        private readonly IMongoCollection<ActivationCodeModel> _activationCode;
        private readonly IMongoCollection<ActiveTokenModel> _activeToken;
        private readonly IMongoCollection<ActiveTokenModelBeforeLogin> _activeTokenBeforeLogin;
        private readonly IMongoCollection<ForgotPasswordOtpModel> _forgotPasswordOtp;
        private readonly ILogger<ActiveTokenRepository> _logger;

        private readonly IMongoCollection<RequestInfoModel> _requestInfo;

        public ActiveTokenRepository(IMongoDBContext mongoIdentityContext, ILogger<ActiveTokenRepository> logger)
        {
            _identityContext = mongoIdentityContext;
            _activationCode = _identityContext.GetCollection<ActivationCodeModel>("ActivationCode");
            _activeToken = _identityContext.GetCollection<ActiveTokenModel>("ActiveToken");
            _activeTokenBeforeLogin = _identityContext.GetCollection<ActiveTokenModelBeforeLogin>("ActiveTokenBeforeLogin");
            _forgotPasswordOtp = _identityContext.GetCollection<ForgotPasswordOtpModel>("ForgotPasswordOtp");
            _requestInfo = _identityContext.GetCollection<RequestInfoModel>("RequestLog");
            _logger = logger;
        }

        public async Task AddActivationCodeAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken)
        {
            await _activationCode.InsertOneAsync(activationCode);
        }

        public async Task<object> CheckTokenAsync(ActiveTokenModel activeToken, CancellationToken cancellationToken)
        {
            var findToken = await _activeToken.Find(x => x.MobileNumber == activeToken.MobileNumber && x.Token == activeToken.Token && x.IsActive == true)
                .FirstOrDefaultAsync(cancellationToken);
            return findToken != null;
        }
        
        public async Task<ActiveTokenModelBeforeLogin> RetrieveTokenBeforeLoginAsync(ActiveTokenModelBeforeLogin activeToken, CancellationToken cancellationToken)
        {
            var findToken = await _activeTokenBeforeLogin.Find(x =>
                    x.ExpiresAt > DateTime.Now &&
                    x.MobileNumber == activeToken.MobileNumber && 
                    x.Token == activeToken.Token && x.IsActive == true)
                .FirstOrDefaultAsync(cancellationToken);
            return findToken;
        }

        public async Task<string> GetActiveTokenByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken)
        {
            var findToken = await _activeToken.Find(x => x.MobileNumber == mobileNumber && x.IsActive == true)
                .FirstOrDefaultAsync(cancellationToken);
            return findToken != null ? findToken.Token : null;
        }

        public async Task GenerateTokenAsync(ActiveTokenModel activeToken, CancellationToken cancellationToken)
        {
            // update and DeActive old tokens
            await DeActiveTokenAsync(activeToken.MobileNumber, null, cancellationToken);

            // set new token
            await _activeToken.InsertOneAsync(activeToken);
        }
        
        public async Task InsertTokenAsync(ActiveTokenModel activeToken, CancellationToken cancellationToken)
        {
            // set new token
            await _activeToken.InsertOneAsync(activeToken);
        }
                
        public async Task InsertTokenBeforeLoginAsync(ActiveTokenModelBeforeLogin activeToken, CancellationToken cancellationToken)
        {
            // set new token
            await _activeTokenBeforeLogin.InsertOneAsync(activeToken);
        }

        public async Task DeActiveTokenAsync(string mobileNumber, string token, CancellationToken cancellationToken)
        {
            // update and DeActive old tokens
            //var filter = Builders<ActiveToken>.Filter.Eq(items => items.MobileNumber, mobileNumber);
            if (token != null)
            {
                var filter = Builders<ActiveTokenModel>.Filter
                    .Where(x =>
                        x.MobileNumber == mobileNumber &&
                        x.Token == token);
                var update = Builders<ActiveTokenModel>.Update.Set(items => items.IsActive, false);
                var result = await _activeToken.UpdateManyAsync(filter, update);
            }
            else
            {
                var filter = Builders<ActiveTokenModel>.Filter
                    .Where(x =>
                        x.MobileNumber == mobileNumber);
                var update = Builders<ActiveTokenModel>.Update.Set(items => items.IsActive, false);
                var result = await _activeToken.UpdateManyAsync(filter, update);
            }
        }
        
        public async Task DeActiveTokenBeforeLoginAsync(string mobileNumber, string token, CancellationToken cancellationToken)
        {
            // update and DeActive old tokens
            //var filter = Builders<ActiveToken>.Filter.Eq(items => items.MobileNumber, mobileNumber);
            if (token != null)
            {
                var filter = Builders<ActiveTokenModelBeforeLogin>.Filter
                    .Where(x =>
                        x.MobileNumber == mobileNumber &&
                        x.Token == token);
                var update = Builders<ActiveTokenModelBeforeLogin>.Update.Set(items => items.IsActive, false);
                var result = await _activeTokenBeforeLogin.UpdateManyAsync(filter, update);
            }
            else
            {
                var filter = Builders<ActiveTokenModelBeforeLogin>.Filter
                    .Where(x =>
                        x.MobileNumber == mobileNumber);
                var update = Builders<ActiveTokenModelBeforeLogin>.Update.Set(items => items.IsActive, false);
                var result = await _activeTokenBeforeLogin.UpdateManyAsync(filter, update);
            }
        }

        public async Task DeActiveTokenAsync(string token, CancellationToken cancellationToken)
        {
            // update and DeActive old tokens
            //var filter = Builders<ActiveToken>.Filter.Eq(items => items.MobileNumber, mobileNumber);
            if (token == null)
                return;

            var filter = Builders<ActiveTokenModel>.Filter
                .Where(x =>
                    x.Token == token);
            var update = Builders<ActiveTokenModel>.Update.Set(items => items.IsActive, false);
            var result = await _activeToken.UpdateManyAsync(filter, update);
        }
        
        public async Task DeActiveTokenBeforeLoginAsync(string token, CancellationToken cancellationToken)
        {
            // update and DeActive old tokens
            //var filter = Builders<ActiveToken>.Filter.Eq(items => items.MobileNumber, mobileNumber);
            if (token == null)
                return;

            var filter = Builders<ActiveTokenModelBeforeLogin>.Filter
                .Where(x =>
                    x.Token == token);
            var update = Builders<ActiveTokenModelBeforeLogin>.Update.Set(items => items.IsActive, false);
            var result = await _activeTokenBeforeLogin.UpdateManyAsync(filter, update);
        }

        public async Task<bool> VerifyActivationCodeAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken)
        {
            var now = DateTime.Now.AddMinutes(-3).ToTimeStampSeconds();

            var findCode = await _activationCode.Find(x => x.MobileNumber == activationCode.MobileNumber &&
                                                           x.Code == activationCode.Code && x.TimeStamp > now && x.IsActive == true)
                .Sort("{ _id: -1}").FirstOrDefaultAsync();

            return findCode != null;
        }

        public async Task ForgotPasswordOtpAsync(ForgotPasswordOtpModel forgotPasswordOtp, CancellationToken cancellationToken)
        {
            await _forgotPasswordOtp.InsertOneAsync(forgotPasswordOtp);
        }

        //    public async Task<bool> GetTryCount(string mobileNumber, string url, CancellationToken cancellationToken)
        //    {
        //        var now = DateTime.Now.AddDays(-1).ToTimeStamp();

        //        var filter = Builders<RequestInfoModel>.Filter
        //.Where(x => x.UserName == mobileNumber &&
        //            x.Url == url && x.RequestTime > now);


        //        var findCode = _requestInfo.Find(filter);

        //        var totalCount = await _requestInfo.CountDocumentsAsync(filter);

        //        if (totalCount < 5)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }

        public async Task<bool> VerifyForgotPasswordOtpAsync(ForgotPasswordOtpModel forgotPasswordOtp, CancellationToken cancellationToken)
        {
            var now = DateTime.Now.AddMinutes(-15).ToTimeStampSeconds();

            var findCode = await _forgotPasswordOtp.Find(x => x.DeliveryAddress == forgotPasswordOtp.DeliveryAddress &&
                                                              x.OtpCode == forgotPasswordOtp.OtpCode && x.TimeStamp > now)
                .Sort("{ _id: -1}").FirstOrDefaultAsync();

            if (findCode != null)
            {
                forgotPasswordOtp.SetRefrenceId(findCode.RefrenceId);
                forgotPasswordOtp.SetUserName(findCode.UserName);
            }

            return findCode != null;
        }

        public async Task UpdateActivationCodeAsync(ActivationCodeModel activationCode, CancellationToken cancellationToken)
        {
            if (activationCode.Code == null)
            {
                var filter = Builders<ActivationCodeModel>.Filter
                    .Where(x =>
                        x.MobileNumber == activationCode.MobileNumber && x.IsActive == true);
                var update = Builders<ActivationCodeModel>.Update.Set(items => items.IsActive, false);
                var result = await _activationCode.UpdateManyAsync(filter, update);
            }
            else
            {
                var filter = Builders<ActivationCodeModel>.Filter
                    .Where(x =>
                        x.MobileNumber == activationCode.MobileNumber &&
                        x.Code == activationCode.Code);
                var update = Builders<ActivationCodeModel>.Update.Set(items => items.IsActive, false);
                var result = await _activationCode.UpdateManyAsync(filter, update);
            }
        }

        public async Task<bool> CheckRefrenceIdResetPasswordAsync(ForgotPasswordOtpModel forgotPasswordOtp, CancellationToken cancellationToken)
        {
            var now = DateTime.Now.AddMinutes(-15).ToTimeStampSeconds();

            var findCode = await _forgotPasswordOtp.Find(x => x.UserName == forgotPasswordOtp.UserName &&
                                                              x.RefrenceId == forgotPasswordOtp.RefrenceId && x.TimeStamp > now)
                .Sort("{ _id: -1}").FirstOrDefaultAsync();

            return findCode != null;
        }

        public async Task<DataList<IEnumerable<ActivationCodeModel>>> ReadActivationCodeAsync(int offset, int count, DateTime? DateFrom, DateTime? DateTo, string mobileNumber, CancellationToken cancellationToken)
        {
            var result = new DataList<IEnumerable<ActivationCodeModel>>();
            try
            {
                long? df = null;
                long? dt = null;
                if (DateFrom.HasValue)
                    df = DateFrom.Value.ToTimeStampSeconds();

                if (DateTo.HasValue)
                    dt = DateTo.Value.ToTimeStampSeconds();

                var filter = Builders<ActivationCodeModel>.Filter
                    .Where(x =>
                        (DateFrom == null || x.TimeStamp >= df) &&
                        (string.IsNullOrEmpty(mobileNumber) || x.MobileNumber == mobileNumber) &&
                        (DateTo == null || x.TimeStamp <= dt));


                var data = await _activationCode.Find(filter)
                    .Skip(offset)
                    .Limit(count)
                    .ToListAsync();

                var totalCount = await _activationCode.CountDocumentsAsync(filter);
                foreach (var item in data)
                {
                    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    dateTime = dateTime.AddSeconds(item.TimeStamp).ToLocalTime();
                    PersianCalendar pc12 = new PersianCalendar();
                    item.PersianDateTime = string.Format("{0}/{1}/{2} {3}:{4}:{5}", pc12.GetYear(dateTime), pc12.GetMonth(dateTime),
                        pc12.GetDayOfMonth(dateTime), pc12.GetHour(dateTime), pc12.GetMinute(dateTime), pc12.GetSecond(dateTime));
                }

                result.Data = data;
                result.RecordsFiltered = Convert.ToInt32(totalCount);
                result.RecordsTotal = Convert.ToInt32(totalCount);
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            }

            return result;
        }

        public async Task<DataList<IEnumerable<ForgotPasswordOtpModel>>> ReadForgotPasswordOtpAsync(int offset, int count, DateTime? DateFrom, DateTime? DateTo, CancellationToken cancellationToken)
        {
            var result = new DataList<IEnumerable<ForgotPasswordOtpModel>>();
            try
            {
                long? df = null;
                long? dt = null;

                if (DateFrom.HasValue)
                    df = DateFrom.Value.ToTimeStampSeconds();

                if (DateTo.HasValue)
                    dt = DateTo.Value.ToTimeStampSeconds();

                var filter = Builders<ForgotPasswordOtpModel>.Filter
                    .Where(x =>
                        (DateFrom == null || x.TimeStamp >= df) &&
                        (DateTo == null || x.TimeStamp <= dt));


                var data = await _forgotPasswordOtp.Find(filter)
                    .Skip(offset)
                    .Limit(count)
                    .ToListAsync();

                var totalCount = await _forgotPasswordOtp.CountDocumentsAsync(filter);

                result.Data = data;
                result.RecordsFiltered = Convert.ToInt32(totalCount);
                result.RecordsTotal = Convert.ToInt32(totalCount);
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
            }

            return result;
        }

        public async Task<bool> LimitationLoginWithIpAddress(string IpAddress, CancellationToken cancellationToken)
        {
            // این متود برای ولیدیشن ورود به اپ با آدرس آی پی در 24 ساعت می باشد
            try
            {
                var today = DateTime.Now;
                var yesterdayTime = today.AddDays(-1);
                var realYesterdayTime = new DateTime(yesterdayTime.Year, yesterdayTime.Month, yesterdayTime.Day, 0, 0, 0);
                var realToday = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);
                var list = new List<ActiveTokenModel>();

                var findSmsLoginCount = await _activeToken.Find(x => x.Ip == IpAddress).ToListAsync();

                foreach (var item in findSmsLoginCount)
                {
                    if (item.TimeStamp.ToDateTime() >= realYesterdayTime && item.TimeStamp.ToDateTime() <= realToday)
                    {
                        list.Add(item);
                    }
                }

                if (list.Count > 4)
                    return false;
                else
                    return true;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
        }

        public async Task<bool> LimitationSendActivationCode(string mobileNumber, CancellationToken cancellationToken)
        {
            // این متود برای ولیدیشن ارسال پیامک کد فعالسازی در 5 دقیقه می باشد
            try
            {
                var now = DateTime.Now;
                var fiveMinutesBefor = now.AddMinutes(-5);
                var list = new List<ActivationCodeModel>();

                var findSmsLoginCount = await _activationCode.Find(x => x.MobileNumber == mobileNumber).ToListAsync();

                foreach (var item in findSmsLoginCount)
                {
                    if (item.TimeStamp.ToDateTime() >= fiveMinutesBefor && item.TimeStamp.ToDateTime() <= now)
                        list.Add(item);
                }

                if (list.Count >= 5)
                    return false;
                else
                    return true;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
        }

        public async Task<bool> LimitationVerifyActivationCode(string mobileNumber, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.Now;
                var fiveMinutesBefor = now.AddMinutes(-5);
                var list = new List<ActiveTokenModel>();

                var findVerifySmsCount = await _activeToken.Find(x => x.MobileNumber == mobileNumber).ToListAsync();

                foreach (var item in findVerifySmsCount)
                {
                    if (item.TimeStamp.ToDateTime() >= fiveMinutesBefor && item.TimeStamp.ToDateTime() <= now)
                        list.Add(item);
                }

                if (list.Count >= 3)
                    return false;
                else
                    return true;
            }
            catch (Exception exp)
            {
                _logger.LogError(ExceptionHelper.ErrorDetail(MethodBase.GetCurrentMethod(), exp));
                return false;
            }
        }

        //public async Task<bool> GetTryCount(string mobileNumber, string url, int serviceStatus, CancellationToken cancellationToken)
        //{
        //    var now = DateTime.Now.AddDays(-1).ToTimeStamp();
        //    var filter = Builders<RequestInfoModel>.Filter
        //    .Where(x => x.UserName == mobileNumber && x.ServiceStatus==serviceStatus &&
        //        x.Url == url && x.RequestTime > now);
        //    try
        //    {
        //        var findCode = _requestInfo.Find(filter);
        //        var totalCount = await _requestInfo.CountDocumentsAsync(filter);
        //        if (totalCount <= 6)
        //            return true;
        //        else
        //        {
        //            now = DateTime.Now.AddHours(-1).ToTimeStamp();
        //            findCode = _requestInfo.Find(filter);
        //            totalCount = await _requestInfo.CountDocumentsAsync(filter);
        //            _logger.LogError("totalCount here 2");
        //            _logger.LogError(totalCount.ToString());
        //            if (totalCount <= 3)
        //                return true;
        //        }
        //        return false;

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Here");
        //        _logger.LogError(ex.Message);
        //        return false;
        //    }

        //}

        public async Task<DataList<IEnumerable<ActiveTokenModel?>>> GetActiveTokenAsync(string mobileNumber, long? timStamp, CancellationToken cancellationToken)
        {
            var result = new DataList<IEnumerable<ActiveTokenModel>>();
            try
            {
                var findToken = await _activeToken.Find(
                        x => x.MobileNumber == mobileNumber ||
                             (timStamp != null && timStamp != 0 && x.TimeStamp == timStamp))
                    .ToListAsync();

                result.Data = findToken;
                return result;
            }
            catch (Exception exp)
            {
                return null;
            }
        }


        public async Task UpdateActivationCodeByMobileNumber(string mobileNumber, CancellationToken cancellationToken)
        {

                var filter = Builders<ActivationCodeModel>.Filter
                    .Where(x =>
                        x.MobileNumber == mobileNumber && x.IsActive == true);
                var update = Builders<ActivationCodeModel>.Update.Set(items => items.IsActive, false);
                var result = await _activationCode.UpdateManyAsync(filter, update);

        }

        public async Task<bool> IsTokenActiveAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;
            var currentTimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            try
            {
                var activeToken = await _activeToken
                    .Find(x => x.Token == token &&
                    x.IsActive &&
                    (x.TimeStamp == 0 || x.TimeStamp > currentTimeStamp))
         .FirstOrDefaultAsync();

                return activeToken != null;
            }
            catch (Exception exp)
            {
                _logger.LogError(exp, "خطا در بررسی فعال بودن توکن");
                return false;
            }
        }

    }
}