namespace IdentityService.Domain.Models
{
    public class ActiveTokenModel : BaseModel, IAggregateRoot
    {
        public ObjectId _id { get; set; }
        public string Token { get; private set; }
        public string MobileNumber { get; private set; }
        public string DeviceInfo { get; private set; }
        public string Version { get; private set; }
        public string Ip { get; private set; }
        public long TimeStamp { get; private set; }
        public bool IsActive { get; private set; }

        public ActiveTokenModel()
        {
        }

        public ActiveTokenModel(string mobileNumber, string deviceInfo, string version,
            string ip, long timeStamp, bool isActive)
        {
            MobileNumber = mobileNumber;
            DeviceInfo = deviceInfo;
            Version = version;
            Ip = ip;
            TimeStamp = timeStamp;
            IsActive = isActive;
        }



        public ActiveTokenModel(string token, string mobileNumber, string deviceInfo, string version,
           string ip, long timeStamp)
        {
            MobileNumber = mobileNumber;
            DeviceInfo = deviceInfo;
            Version = version;
            Ip = ip;
            TimeStamp = timeStamp;
            IsActive = true;
        }

        public ActiveTokenModel(string token, string mobileNumber)
        {
            Token = token;
            MobileNumber = mobileNumber;
        }

        public static ActiveTokenModel CreateToken(string token, string mobileNumber, string deviceInfo, string version,
            string ip, long timeStamp)
            => new ActiveTokenModel(token, mobileNumber, deviceInfo, version, ip, timeStamp);


        public void GenerateToken()
        {
            Token = Guid.NewGuid().ToString();
        }
    }
}
