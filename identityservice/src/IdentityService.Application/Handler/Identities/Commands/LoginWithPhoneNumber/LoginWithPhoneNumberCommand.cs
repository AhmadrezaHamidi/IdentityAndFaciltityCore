using IdentityService.Contracts.Identities.CommandResponses;

namespace IdentityService.Application.Handler.Identities.Commands.LoginWithPhoneNumber;

public record LoginWithPhoneNumberCommand(string MobileNumber,string? InitToken) : BaseCommand<BaseMessageResponse<LoginWithPhoneNumberDto>>;