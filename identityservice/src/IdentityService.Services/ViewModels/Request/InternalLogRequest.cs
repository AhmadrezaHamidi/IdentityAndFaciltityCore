namespace IdentityService.Services.ViewModels.Request
{
    public class InternalLogRequest
    {
        public string Url { get; set; }
        public string IP { get; set; }
        public string Token { get; set; }
        public string Version { get; set; }
        public byte AppId { get; set; }
        public byte DeviceTypeId { get; set; }
        public string DeviceInfo { get; set; }
        public string UserAgent { get; set; }
        public long RequestTime { get; set; }
        public double ExecutionTime { get; set; }
        public string Body { get; set; }
        public string UserName { get; set; }
        public string ItemId { get; private set; }
        public int? ServiceStatus { get; set; }
        public string? ResponseModel { get; set; }
    }
}
