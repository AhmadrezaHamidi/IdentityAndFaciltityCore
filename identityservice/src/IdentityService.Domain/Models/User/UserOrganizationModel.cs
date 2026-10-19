namespace IdentityService.Domain.Models.User
{
    public class UserOrganizationModel : BaseModel
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string NationalCode { get; private set; }
        public string MobileNumber { get; private set; }
        public int OrganizationId { get; private set; }

        public string OrganizationTitle { get; private set; }

        public UserOrganizationModel()
        {
        }

        public UserOrganizationModel(int id, int userId, int organizationId, string firstName, string lastName, string nationalCode, string mobileNumber)
        {
            Id = id;
            UserId = userId;
            OrganizationId = organizationId;
            FirstName = firstName;
            LastName = lastName;
            NationalCode = nationalCode;
            MobileNumber = mobileNumber;
        }

        public void SetOrganizationTitle(string organizationTitle)
        {
            OrganizationTitle = organizationTitle;
        }
    }
}
