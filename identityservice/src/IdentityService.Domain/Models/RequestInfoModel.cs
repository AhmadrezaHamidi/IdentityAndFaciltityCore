namespace IdentityService.Domain.Models
{
    public class RequestInfoModel : BaseModel
    {
        public ObjectId _id { get; set; }
        public string Url { get; private set; }
        public string IP { get; private set; }
        public string Token { get; private set; }
        public string UserName { get; private set; }
        public string Version { get; private set; }
        public byte AppId { get; private set; }
        public byte DeviceTypeId { get; private set; }
        public string DeviceInfo { get; private set; }
        public string UserAgent { get; private set; }
        public long RequestTime { get; private set; }
        public long ExecutionTime { get; private set; }
        public string Body { get; private set; }
        public string ItemId { get; private set; }
        public int? ServiceStatus { get; private set; }
        public string? ResponseModel { get; private set; }

        public RequestInfoModel()
        {

        }
        public RequestInfoModel(string url, string ip, string token, string version, byte appId, byte deviceTypeId, string deviceInfo,
           string userAgent, long executionTime, string body, long requestTime, string userName, int? serviceStatus, string? responseModel)
        {
            Url = url;
            IP = ip;
            Token = token;
            Version = version;
            AppId = appId;
            DeviceTypeId = deviceTypeId;
            DeviceInfo = deviceInfo;
            UserAgent = userAgent;
            ExecutionTime = executionTime;
            Body = body;
            RequestTime = requestTime;
            UserName = userName;
            ServiceStatus = serviceStatus;
            ResponseModel = responseModel;
        }

        public void SetItemId()
        {
            ItemId = _id.ToString();
        }
    }
}