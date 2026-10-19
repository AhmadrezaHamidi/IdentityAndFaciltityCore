using MongoDB.Bson;

namespace IdentityService.Services.ViewModels.Admin
{
    public class ResponseForgotPasswordOtpViewModel : BaseViewModel
    {
        [JsonIgnore]
        public ObjectId _id { get; set; }
        public string ItemId { get; set; }
        public string UserName { get; set; }
        public string OtpCode { get; set; }
        public string RefrenceId { get; set; }
        public long TimeStamp { get; set; }
        public byte DeliveryType { get; set; }
        public string DeliveryAddress { get; set; }

        public void SetItemId()
        {
            ItemId = _id.ToString();
        }
    }
}
