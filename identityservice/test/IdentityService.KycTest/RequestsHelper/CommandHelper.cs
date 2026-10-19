using IdentityService.Application.Handler.Identities.Commands.RegisterUser;
using IdentityService.Application.Handler.User;
using IdentityService.Application.Handler.UserManagement;
using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;
using IdentityService.Services.ViewModels.Request;
using OmidBank.Common.JwtHelper;

namespace IdentityService.KycTest.RequestsHelper;

public static class CommandHelper
{
    public static DeliverKycUmServiceRequest GetDeliverKycUmServiceRequest_Constant()
    {
        return new DeliverKycUmServiceRequest(1);
    }

    public static RetrieveKycUmTransactionLogRequest GetRetrieveKycUmTransactionLogRequest_Constant()
    {
        return new RetrieveKycUmTransactionLogRequest(1);
    }

    public static VinaraPushApiVerificationRequest GetPushApiVerificationRequest_Constant()
    {
        return new VinaraPushApiVerificationRequest("123456", "accept", new VinaraError { Code = 1000, Message = string.Empty });
    }

    public static ExternalUpdateUMRequest GetUpdateUmRequest_Constant(string uniqueKey = "90c86a7f-3fd7-4ea1-ae1f-cb8c07c1569f")
    {
        return new ExternalUpdateUMRequest(uniqueKey, "APPROVED");
    }

    public static VinaraRetrieveUserInformationRequest GetRetrieveUserInformationRequest_Constant()
    {
        return new VinaraRetrieveUserInformationRequest("6bb08dab-b4fd-4e9e-9253-f80d570825d3");
    }

    public static RegisterUMRequest GetRegisterUMRequest_Constant()
    {
        return new RegisterUMRequest
        {
            BirthDate = DateTime.Parse("1991-04-03 17:18:40.000").Second,
            NationalCardSeries = "4G44108447",
        };
    }
    
    public static MakeChangeMobileNumberRequest GetMakeChangeMobileNumberRequest_Constant()
    {
        return new MakeChangeMobileNumberRequest("09121111111", "09121111112", "0070080090");
    }
    
    public static ChangeMobileNumberResultRequest GetChangeMobileNumberResultRequest_Constant()
    {
        return new ChangeMobileNumberResultRequest("1a430c60-56aa-4fd2-ba89-c66740e762e6");
    }
            
    public static ChangeMobileNumberSendOtpRequest GetSendOtpRequest_Constant()
    {
        return new ChangeMobileNumberSendOtpRequest("09121111111", "09121111112", "0070080090");
    }
    
    public static ChangeMobileNumberVerifyOtpRequest GetVerifyOtpRequest_Constant(string otp = "")
    {
        return new ChangeMobileNumberVerifyOtpRequest(otp, "09121111111");
    }
    
    public static ChangeMobileNumberRequest GetChangeMobileNumberRequest_Constant()
    {
        return new ChangeMobileNumberRequest("1a430c60-56aa-4fd2-ba89-c66740e762e6", "The ReferenceCode");
    }
    
    public static ReverseChangeMobileNumberRequest GetReverseChangeMobileNumberRequest_Constant()
    {
        return new ReverseChangeMobileNumberRequest("1a430c60-56aa-4fd2-ba89-c66740e762e6");
    }
    
    public static CheckChangeMobileNumberPaymentRequest GetCheckChangeMobileNumberRequest_Constant()
    {
        return new CheckChangeMobileNumberPaymentRequest("1a430c60-56aa-4fd2-ba89-c66740e762e6");
    }
    
    public static InternalCheckUserManagmentRequest InternalCheckUserManagmentRequest_Constant()
    {
        return new InternalCheckUserManagmentRequest(1);
    }
    
    public static RegisterUserCommand GetRegisterRequest_Constant(string token)
    {
        return new RegisterUserCommand("0070080090", "09121111111", "", token);
    }
    
    public static UserAuthenticationRequest GetUserAuthenticationRequest_Constant(string token)
    {
        return new UserAuthenticationRequest
        {
            ActiveToken = token,
            UserWithNewMobileNumber = true,
            MobileNumber = "09121111111",
        };
    }
}