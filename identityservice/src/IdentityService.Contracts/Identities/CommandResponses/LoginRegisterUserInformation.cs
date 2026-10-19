using IdentityService.Services.ViewModels.AradPushNotification;

namespace IdentityService.Contracts.Identities.CommandResponses;

public record LoginRegisterUserInformation(
    bool? UserExists,
    string? FirstName,
    string? LastName,
    int? CityId,
    string? CityTitle,
    string? CityEnTitle,
    int? ProvinceId,
    string? ProvinceTitle,
    string? ProvinceEnTitle,
    string? AvatarUrl,
    string? MobileNumber,
    string? Token,
    string? InvitedCode,
    string? NationalCode,
    string? MyInvitedCode,
    bool? ChangePasswordWallet,
    long? OmidBankWalletAccountNumber,
    string? RegisterToken,
    ResSendNotificationDataViewModel? NotificationDataViewModel,
    string? UserName = null);