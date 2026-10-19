namespace IdentityService.Domain.Models
{
    public class ActiveTokenModelBeforeLogin : BaseModel, IAggregateRoot
    {
        public ObjectId _id { get; set; }
        public string Token { get; private set; }
        public string MobileNumber { get; private set; }
        public string OldMobileNumber { get; private set; }
        public string DeviceInfo { get; private set; }
        public string Version { get; private set; }
        public string Ip { get; private set; }
        public long TimeStamp { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsActive { get; private set; }

        public ActiveTokenModelBeforeLogin()
        {
        }

        public ActiveTokenModelBeforeLogin(string mobileNumber, string oldMobileNumber, string deviceInfo, string version,
            string ip, long timeStamp, DateTime expiresAt, bool isActive)
        {
            MobileNumber = mobileNumber;
            OldMobileNumber = oldMobileNumber;
            DeviceInfo = deviceInfo;
            Version = version;
            Ip = ip;
            TimeStamp = timeStamp;
            ExpiresAt = expiresAt;
            IsActive = isActive;
        }

        public ActiveTokenModelBeforeLogin(string token, string mobileNumber)
        {
            Token = token;
            MobileNumber = mobileNumber;
        }


        public void GenerateToken()
        {
            Token = Guid.NewGuid().ToString();
        }
    }
}
