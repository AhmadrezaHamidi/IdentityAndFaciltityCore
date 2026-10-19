using System.Linq;
using System.Text;
using OmidBank.Common.Base.Shared;

namespace IdentityService.Domain.Models.User
{
    public class UserModel : BaseModel, IAggregateRoot
    {
        public int Id { get; private set; }
        public int PersonId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string MobileNumber { get; private set; }
        public string UserName { get; private set; }
        public string NationalCode { get; private set; }
        public string AvatarName { get; private set; }
        public string InvitedCode { get; private set; }
        public string? MyInviteCode { get; set; }
        public long? BankCustomerNumber { get; set; }
        public DateTime? BirthDate { get; set; }


        [JsonIgnore]
        public string Password { get; private set; }
        public float? Lat { get; private set; }
        public float? Long { get; private set; }
        public string Address { get; private set; }
        public int? CityId { get; private set; }
        public bool IsActive { get; private set; }
        public string Token { get; private set; }
        public string Email { get; private set; }
        public bool? IsVerified { get; private set; }
        public DateTime CreationTime { get; private set; }


        private List<RoleModel> roles;
        public IEnumerable<RoleModel> Roles => roles;


        private List<string> menuUrls;
        public IEnumerable<string> MenuUrls => menuUrls;


        private List<RoleMenuModel> roleMenuModels;
        public IEnumerable<RoleMenuModel> RoleMenuModels => roleMenuModels;


        public UserModel()
        {
        }

        public UserModel(int id, int personId, string firstName, string lastName, string mobileNumber, string userName, DateTime creationTime,
             string nationalCode = null, string avatarName = null, bool isActive = true, bool? isVerified = null, string password = ***;
            PersonId = personId;
            FirstName = firstName;
            LastName = lastName;
            MobileNumber = mobileNumber;
            NationalCode = nationalCode;
            AvatarName = avatarName;
            UserName = userName;
            IsActive = isActive;
            IsVerified = isVerified;
            CreationTime = creationTime;
            Password = ***;
            InvitedCode = invitedCode;
            Email = email;
            CityId = cityId;
            MyInviteCode = myInviteCode;
        }

        public UserModel(string mobileNumber)
        {
            MobileNumber = mobileNumber;
            IsActive = true;
        }

        public void AddRoleItem(int roleId, string title, string persianTitle, bool isActive)
        {
            var existRole = roles.Where(items => items.Id == Id)
                .SingleOrDefault();

            if (existRole == null)
            {
                var newRole = new RoleModel(roleId, title, persianTitle, isActive);
                roles.Add(newRole);
            }
        }
        public void SetRoles(List<RoleModel> roles)
        {
            if (roles == null) roles = new List<RoleModel>();
            this.roles = roles;
        }

        public void SetRoleMenuModels(List<RoleMenuModel> roleMenuModels)
        {
            if (roleMenuModels == null) roleMenuModels = new List<RoleMenuModel>();
            this.roleMenuModels = roleMenuModels;
        }


        public void SetToken(string token)
        {
            Token = token;
        }

        public void GenerateAdminPanelToken()
        {
            var userIdentifer = $"{UserName}:{Password}";
            Token = Convert.ToBase64String(Encoding.Default.GetBytes(userIdentifer));
        }

        public void GeneratePanelJwtToken(string token)
        {
            Token = token;
        }

        public void SetPassword(string password)
        {
            // convert to md5 hash password and set 
            //Password = ***;
            Password = ***;

        }

        public void SetLocation(string address, int? cityId, float? latitude, float? longitude)
        {
            Address = address;
            CityId = cityId;
            Lat = latitude;
            Long = longitude;
        }

        public void SetCreationTime(DateTime time)
        {
            CreationTime = time;
        }
    }
}
