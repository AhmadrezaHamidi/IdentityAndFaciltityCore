using IdentityService.Contracts.Admin.CommandResponses;

namespace IdentityService.Application.Handler.Admin.Commands.AdminLoginWithPhoneNumber;

public record AdminLoginWithPhoneNumberCommand(string MobileNumber) : BaseCommand<BaseMessageResponse<AdminLoginWithPhoneNumberDto>>;