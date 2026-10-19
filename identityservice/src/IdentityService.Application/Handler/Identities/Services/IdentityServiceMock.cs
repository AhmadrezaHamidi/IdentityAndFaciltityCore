// using IdentityService.Application.Handler.Identities.Commands.LoginWithPhoneNumber;
// using IdentityService.Application.Handler.Identities.Commands.RegisterUser;
// using IdentityService.Application.Handler.Identities.Commands.VerifyLoginOtp;
// using IdentityService.Contracts.Identities.CommandResponses;
//
// namespace IdentityService.Application.Handler.Identities.Services;
//
// public class IdentityServiceMock : IIdentityService
// {
//     public Task<BaseMessageResponse<LoginWithPhoneNumberDto>> LoginWithPhoneNumber(LoginWithPhoneNumberCommand request, CancellationToken cancellationToken)
//     {
//         return Task.FromResult(MessageResponse.GenerateMessageResponse<LoginWithPhoneNumberDto>(EResponseStatusCode.DoneSuccessfully))!;
//     }
//     
//     public Task<BaseMessageResponse<VerifyLoginOtpDto>> VerifyLoginOtp(VerifyLoginOtpCommand request, CancellationToken cancellationToken)
//     {
//         var result = new VerifyLoginOtpDto(true, "سید تست", "تستیان", 3, "تهران", "tehran", 4, 
//             "تهران", "تهران", "", "09121111111", Guid.NewGuid().ToString(), 
//             "YVY2LMP9", "1234567890", "YVY2LMP9",
//             false, 12456, null);
//         return Task.FromResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, result))!;
//     }
//     
//     public Task<BaseMessageResponse<RegisterUserDto>> RegisterUser(RegisterUserCommand request, CancellationToken cancellationToken)
//     {
//         var result = new RegisterUserDto(true, "سید تست", "تستیان", 3, "تهران", "tehran", 4, 
//             "تهران", "تهران", "", "09121111111", Guid.NewGuid().ToString(), 
//             "YVY2LMP9", "1234567890", "YVY2LMP9",
//             false, 12456, null);
//         return Task.FromResult(MessageResponse.GenerateMessageResponse(EResponseStatusCode.DoneSuccessfully, result))!;
//     }
// }