using System.Net;
using FluentAssertions;
using IdentityService.Application.Handler.UserManagement;
using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;
using IdentityService.KycTest.Factories;
using IdentityService.KycTest.RequestsHelper;
using IdentityService.Test.Common.Constants;
using IdentityService.Test.Common.LocalDebugsOnly;
using IdentityService.Test.Common.Network;
using OmidBank.Common.Base;

namespace IdentityService.KycTest;

public class VinaraTest(IdentityServiceIntegrationEnvironment env)
    : IClassFixture<IdentityServiceIntegrationEnvironment>
{
    private readonly HttpClient _client = env.CreateClient();

    [Fact]
    public async Task Identity_WhenWagePaid_ShouldCreateBankUser()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetDeliverKycUmServiceRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/InternalKycTransaction/DeliverKycUmRegister", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<object>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenRetrieveKycUmTransactionLog_ShouldReturnKycTransactionLog()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetRetrieveKycUmTransactionLogRequest_Constant();
            
            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/InternalKycTransaction/RetrieveKycUmTransactionLog", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<RetrieveKycUmTransactionLogResponse>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenVinaraCallBackIsCalled_ShouldUpdateUserKycStatus()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetPushApiVerificationRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/ExternalInquiryResult/VinaraUpdateUm", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<VinaraBaseResponse<object>>(response, CancellationToken.None);
            result.Success.Should().BeTrue();
            result.Error.Should().Be(null);
            result.Result.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenFarashenasaCallBackIsCalled_ShouldUpdateUserKycStatus()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var registerCommand = CommandHelper.GetRegisterUMRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var registerResponse = await requestHelper.SendEncryptedAsync(HttpMethod.Post, "/v2/Authentication/RegisterUM", registerCommand,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var registerResult = await RequestHelper.ReadContentTask<BaseMessageResponse<RegisterUMResponse>>(registerResponse, CancellationToken.None);
            registerResult.IsSuccess.Should().BeTrue();
            registerResult.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            registerResult.Data.Should().NotBeNull();

            var startIndex = registerResult.Data.RedirectToUrl.IndexOf("Key=") + 4;
            
            // Arrange
            var command = CommandHelper.GetUpdateUmRequest_Constant(registerResult.Data.RedirectToUrl.Substring(startIndex));

            // Act
            var response = await requestHelper.PostAsync("/v2/ExternalInquiryResult/UpdateUM", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<VinaraBaseResponse<object>>(response, CancellationToken.None);
            result.Success.Should().BeTrue();
            result.Error.Should().Be(null);
            result.Result.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenVinaraCallRetrieveUserInformation_ShouldReturnCollectedUserInformation()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetRetrieveUserInformationRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/ExternalInquiryResult/RetrieveUserInfo", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<VinaraRetrieveUserInformationResponse>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenRegisterIsCalled_ShouldStartRegistration()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetRegisterUMRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.SendEncryptedAsync(HttpMethod.Post, "/v2/Authentication/RegisterUM", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<RegisterUMResponse>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenCheckUmStatusIsCalled_ShouldUpdateUserKycStatusAndReturnStatus()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.GetAsync("/v2/Authentication/CheckStatusUM",
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<CheckStatusUMResponse>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenRegisterFlowIsDone_ShouldCreateBankUser()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var registerCommand = CommandHelper.GetRegisterUMRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var registerResponse = await requestHelper.SendEncryptedAsync(HttpMethod.Post, "/v2/Authentication/RegisterUM", registerCommand,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var registerResult = await RequestHelper.ReadContentTask<BaseMessageResponse<RegisterUMResponse>>(registerResponse, CancellationToken.None);
            registerResult.IsSuccess.Should().BeTrue();
            registerResult.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            registerResult.Data.Should().NotBeNull();
            
            // Act
            var checkUmResponse = await requestHelper.GetAsync("/v2/Authentication/CheckStatusUM",
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            checkUmResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var checkUmResult = await RequestHelper.ReadContentTask<BaseMessageResponse<CheckStatusUMResponse>>(checkUmResponse, CancellationToken.None);
            checkUmResult.IsSuccess.Should().BeTrue();
            checkUmResult.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            checkUmResult.Data.Should().NotBeNull();
            
            // Arrange
            var deliverCommand = CommandHelper.GetDeliverKycUmServiceRequest_Constant();

            // Act
            var deliverResponse = await requestHelper.PostAsync("/v2/InternalKycTransaction/DeliverKycUmRegister", deliverCommand,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            deliverResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deliverResult = await RequestHelper.ReadContentTask<BaseMessageResponse<object>>(deliverResponse, CancellationToken.None);
            deliverResult.IsSuccess.Should().BeTrue();
            deliverResult.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            deliverResult.Data.Should().BeNull();
        });
    }
}