using IdentityService.Domain.Models;
using IdentityService.Domain.Models.UserBranchs;

namespace IdentityService.Domain.IRepositories
{
    public interface IUserRepository : IBaseGenericRepository<UserEntity, int>
    {
        Task<AddPersonStatusModel> AddPersonAsync(PersonModel person, CancellationToken cancellationToken);
        Task<PersonModel> GetPersonByIdAsync(int id, CancellationToken cancellationToken);
        Task<PersonModel> GetPersonByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<PersonModel>>> ReadPersonAsync(int offset, int count, string firstName, string lastName, string nationalCode, string mobileNumber, CancellationToken cancellationToken);
        Task<bool> AddApplicationUserAsync(string mobileNumber, string invitedCode, string firebaseKey, string myInviteCode, CancellationToken cancellationToken);
        Task<bool> AddApplicationUserAsync(int personId, string invitedCode, string firebaseKey, string myInviteCode, CancellationToken cancellationToken);
        Task<int> UpdateProfileAsync(ProfileModel profile, CancellationToken cancellationToken);
        Task<int> UpdatePersonVerified(int personId, bool isVerified, CancellationToken cancellationToken);
        Task<UserModel> GetByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken);
        Task<UserModel> GetPanelUserByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken);
        Task<ProfileModel> GetProfileAsync(string mobileNumber, CancellationToken cancellationToken);
        Task<int> CheckInvitedCodeAsync(string mobileNumber, string invitedCode, CancellationToken cancellationToken);
        Task<bool> UpdateInvitedCodeAsync(int personId, string invitedCode, CancellationToken cancellationToken);
        Task<bool> UpdateMyInviteCodeAsync(int id, string myInviteCode, CancellationToken cancellationToken);
        Task<bool> UpdateAvatarAsync(string userName, string mobileNumber, string fileName, CancellationToken cancellationToken);
        Task<bool> UpdaterKvcAsync(string mobileNumber, bool? nationalCard, bool? signImage, CancellationToken cancellationToken);
        Task<UserModel> CheckPanelUserAsync(string userName, string passwrod, CancellationToken cancellationToken);
        Task<UserModel> GetPanelUserByUserNameAsync(string userName, CancellationToken cancellationToken);
        Task<int> AddPanelUserAsync(UserModel user, CancellationToken cancellationToken);
        Task<bool> UpdatePanelUserAsync(UserModel user, CancellationToken cancellationToken);
        Task<string> IsValidUserAsync(string nationalCode, string mobileNumber, string email, CancellationToken cancellationToken);
        Task<bool> ResetPasswordAsync(string userName, string newPassword, CancellationToken cancellationToken);
        Task<bool> ChangePasswordAsync(string userName, string oldPassword, string newPassword, CancellationToken cancellationToken);
        Task<UserModel> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<UserModel>>> PanelReadAsync(int offset, int count, string firstName, string lastName, string nationalCode, string mobileNumber, string userName, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<UserModel>>> AppReadAsync(int offset, int count, string firstName, string lastName, string nationalCode, string mobileNumber, CancellationToken cancellationToken);
        Task<bool> AddUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken);
        Task<bool> RemoveUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken);
        Task<bool> AddOrganizationAsync(int userId, int organizationId, CancellationToken cancellationToken);
        Task<bool> RemoveOrganizationAsync(int userId, int organizationId, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<UserOrganizationModel>>> ReadUserOrganizationAsync(int offset, int count, int organizationId, CancellationToken cancellationToken);
        Task<string> IsValidAppUserAsync(string nationalCode, string mobileNumber, CancellationToken cancellationToken);
        Task<bool> DeleteUserMessageAsync(int messageId, int userId, CancellationToken cancellationToken);
        Task<bool> SetReadStatusMessage(int messageId, int userId, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<ReadUserMessagesModel>>> ReadUserMessageAsync(int offset, int count, int userId, CancellationToken cancellationToken);
        Task<bool> AddUserMessageAsync(int messageId, string mobileNumber, CancellationToken cancellationToken);
        Task<DataList<IEnumerable<UserMessageModel>>> PanelReadUserMessageAsync(int offset, int count, int messageId, byte? type, CancellationToken cancellationToken);
        Task UpdateUserFirebaseKey(int userId, string firebaseKey, CancellationToken cancellationToken);
        Task<FirebaseResultModel> InquiryFireBaseToken(int userId, CancellationToken cancellationToken);
        Task<int> ReadUserMessageCount(int userId, CancellationToken cancellationToken);
        int UpdatePersonWithCivilRegistery(string FirstName, string LastName, bool Gender, string NationalCode, CancellationToken cancellationToken);

        // for mobile application
        Task<MobileNumberFormatted> GetPersonMobileNumberByNationalCode(string nationalCode);
        Task<bool> CheckNationalCodeTryCount(string nationalCode);
        Task<string> GeneratePersonOpenAccountModel(GeneratePersonOpenAccountModel id, CancellationToken cancellationToken);
        Task<object> GetPersonDetail(int id, CancellationToken cancellationToken);

        Task<bool> UpdateFierbaseToken(string MobilNumber, string FierbaseToken);
        Task<PersonModel?> GetPersonByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);
        Task<bool> AddUserBranch(UserBranchModel model, CancellationToken cancellationToken);

        Task<bool> ExistInvitedCodeAsync(string invitedCode, int? personId, CancellationToken cancellationToken);
        Task<int?> GetPersonIdNationalCodeAsync(string nationalCode, CancellationToken cancellationToken);

        Task<UserEntity> FindById(int userId, CancellationToken cancellationToken);

        Task<UserEntity> FindByPersonId(int personId, CancellationToken cancellationToken);

        Task UpdateUserCustomerNumber(int userId, long customerNumber, CancellationToken cancellationToken);

        Task<int> GetPersonIdAsync(int id, CancellationToken cancellationToken);

        Task<PersonModel?> GetPersonDataWithInvitedCode(string invitedCode, CancellationToken cancellationToken);

        Task<List<UserEntity>?> GetMyInvitedCode(string myInvitedCode, CancellationToken cancellationToken);

        Task<bool> IsActive(int userId, CancellationToken cancellationToken);
        Task<PersonEntity> GetPersonByMobileNumber(string mobileNumber, CancellationToken cancellationToken);
        Task<UserModel> CheckPanelUserByUserNameAsync(string userName, CancellationToken cancellationToken);
    }
}
