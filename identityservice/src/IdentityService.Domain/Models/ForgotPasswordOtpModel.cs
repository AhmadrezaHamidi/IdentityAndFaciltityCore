namespace IdentityService.Domain.Models
{
    public class ForgotPasswordOtpModel
    {
        public ObjectId _id { get; set; }
        public string UserName { get; private set; }
        public string OtpCode { get; private set; }
        public string RefrenceId { get; private set; }
        public long TimeStamp { get; private set; }
        public byte DeliveryType { get; private set; }
        public string DeliveryAddress { get; private set; }

        public ForgotPasswordOtpModel(string userName, byte deliveryType, string deliveryAddress)
        {
            UserName = userName;
            DeliveryType = deliveryType;
            DeliveryAddress = deliveryAddress;
        }
        public ForgotPasswordOtpModel()
        {
        }

        public void GenerateOtp(bool isTest)
        {
            if (isTest)
                OtpCode = "123456";
            else
                OtpCode = CommonHelper.GenerateOtpCode();
        }
        public void GenerateRefrenceId()
        {
            RefrenceId = CommonHelper.GenerateLongRandomNumber().ToString();
        }

        public void SetOtp(string otp)
        {
            OtpCode = otp;
        }
        public void SetRefrenceId(string refrenceId)
        {
            RefrenceId = refrenceId;
        }

        public void SetUserName(string userName)
        {
            UserName = userName;
        }

        public void SetTime(DateTime datetime)
        {
            TimeStamp = datetime.ToTimeStampSeconds();
        }
    }
}
