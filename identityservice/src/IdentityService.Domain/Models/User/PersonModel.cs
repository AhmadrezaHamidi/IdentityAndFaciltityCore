namespace IdentityService.Domain.Models.User
{
    public class PersonModel : BaseModel
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string MobileNumber { get; private set; }
        public string UserName { get; private set; }
        public string NationalCode { get; private set; }

        public DateTime CreationTime { get; set; }

        public PersonModel()
        { }

        public PersonModel(string mobileNumber)
        {
            MobileNumber = mobileNumber;
        }
        public PersonModel(int id, string firstName, string lastName, string nationalCode, string mobileNumber)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            NationalCode = nationalCode;
            MobileNumber = mobileNumber;
        }

        public PersonModel(int id, string firstName, string lastName, string nationalCode, string mobileNumber, DateTime creationTime)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            NationalCode = nationalCode;
            MobileNumber = mobileNumber;
            CreationTime = creationTime;
        }
    }
}
