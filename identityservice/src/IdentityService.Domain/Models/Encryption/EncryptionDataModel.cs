namespace IdentityService.Domain.Models.Encryption
{
    public class EncryptionDataModel : BaseModel
    {
        public string EncryptedKey { get; set; }
        public string EncryptedData { get; set; }
    }
}
