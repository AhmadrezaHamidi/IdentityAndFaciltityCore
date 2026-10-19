namespace IdentityService.Domain.Models
{
    public class AddUserWalletModel
    {
        public string MobileNumber { get; set; }
        public int WalletTypeId { get; set; }
        public int UserId { get; set; }
    }
}
