namespace IdentityService.Domain.Models.User
{
    public class GetMyInvitedResponse
    {
        public string MobileNumber { get; set; } = null!;

        public long CreationTime { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
