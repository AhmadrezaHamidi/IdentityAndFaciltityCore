namespace IdentityService.Domain.Models
{

    public class ActivationCodeModel
    {
        public ObjectId _id { get; set; }
        public string MobileNumber { get; private set; }
        public string Code { get; private set; }
        public long TimeStamp { get; private set; }
        public bool IsActive { get; set; }
        public string PersianDateTime { get; set; }
        public Enums.ActivationCode Type { get; set; } = Enums.ActivationCode.Register;

        public ActivationCodeModel()
        {

        }

        [JsonConstructor]
        public ActivationCodeModel(string mobileNumber, string code)
        {
            MobileNumber = mobileNumber;
            Code = code;
        }

        public ActivationCodeModel(string mobileNumber)
        {
            MobileNumber = mobileNumber;
            IsActive = true;
        }

        public void GenerateCode(bool isTest)
        {
            if (isTest)
                Code = "123456";
            else
                Code = CommonHelper.GenerateOtpCode();
        }
        public void SetTime(DateTime datetime)
        {
            TimeStamp = datetime.ToTimeStampSeconds();
        }
    }
}
