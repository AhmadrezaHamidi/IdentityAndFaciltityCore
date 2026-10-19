using System.Net;
using FluentAssertions;
using IdentityService.Application.Handler.User;
using IdentityService.Contracts.Identities.CommandResponses;
using IdentityService.Domain.Entity.ChangeMobileNumber;
using IdentityService.Domain.Models.User;
using IdentityService.KycTest.Factories;
using IdentityService.KycTest.RequestsHelper;
using IdentityService.Test.Common.Constants;
using IdentityService.Test.Common.LocalDebugsOnly;
using IdentityService.Test.Common.Network;
using OmidBank.Common.Base;
using OmidBank.Common.JwtHelper;
using OmidBank.Common.Models;

namespace IdentityService.KycTest;

public class ChangePhoneNumberTest(IdentityServiceIntegrationEnvironment env)
    : IClassFixture<IdentityServiceIntegrationEnvironment>
{
    private readonly HttpClient _client = env.CreateClient();
    
    [Fact]
    public async Task Identity_WhenRequestIs_ShouldReturnPaymentStatus()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetCheckChangeMobileNumberRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/InternalIdentity/CheckChangeMobileNumberPayment", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None, false, false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<CheckChangeMobileNumberPaymentResponse>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task Identity_WhenChangeMobileNumberRequested_ShouldCreateChangeRequest()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetMakeChangeMobileNumberRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/User/mobile-number-change", command,
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
    public async Task Identity_WhenAgreementsRead_ShouldReturnAgreements()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/User/mobile-number-change/agreements", new {},
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
    public async Task Identity_WhenIntroductionRead_ShouldReturnIntroduction()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/User/mobile-number-change/introduction", new {},
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<ChangeMobileNumberIntroduction>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenRequestForResultOfChangeRequest_ShouldReturnResult()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetChangeMobileNumberResultRequest_Constant();
            
            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/User/mobile-number-change/result", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<ChangeMobileNumberResultResponse>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenWageRequested_ShouldReturnWage()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/User/mobile-number-change/wage", new {},
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<ChangeMobileNumberPaymentResponse>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenSendOtpIsCalled_ShouldSendOtp()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetSendOtpRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/User/mobile-number-change/otp/send", command,
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
    public async Task Identity_WhenVerifyOtpIsCalled_ShouldVerifyOtpAndReturnWageData()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetVerifyOtpRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/User/mobile-number-change/otp/verify", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<ChangeMobileNumberPaymentResponse>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenConfirmTheMobileNumberChange_ShouldChangeTheNumber()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetChangeMobileNumberRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/InternalIdentity/ChangeMobileNumber", command,
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
    public async Task Identity_WhenReverseTheMobileNumberChange_ShouldReverseTheNumberChange()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetReverseChangeMobileNumberRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/InternalIdentity/ReverseChangeMobileNumber", command,
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
    public async Task Identity_WhenGetNumberChangePaymentStatus_ShouldReturnPaymentStatus()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = CommandHelper.GetCheckChangeMobileNumberRequest_Constant();

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync("/v2/InternalIdentity/CheckChangeMobileNumberPayment", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<BaseMessageResponse<CheckChangeMobileNumberPaymentResponse>>(response, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            result.Data.Should().NotBeNull();
        });
    }
    
    [Fact]
    public async Task Identity_WhenRegisterWithNewNumber_ShouldReturnOldNumberAndMakeToken()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Register Arrange
            var jwtService = env.Services.GetService(typeof(IJwtService)) as IJwtService;
            var token = jwtService!.GenerateAsync(100, "09121111111");
            var registerCommand = CommandHelper.GetRegisterRequest_Constant(token);

            // Register Act
            var requestHelper = new RequestHelper(_client);
            var registerResponse = await requestHelper.PostAsync("/v3/Identity/register", registerCommand,
                TestConstants.AdditionalHeaders29, CancellationToken.None);

            // Register Assert
            registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var registerResult = await RequestHelper.ReadContentTask<BaseMessageResponse<RegisterUserDto>>(registerResponse, CancellationToken.None);
            registerResult.IsSuccess.Should().BeTrue();
            registerResult.StatusCode.Should().Be(EResponseStatusCode.YouAreAlreadyRegisteredAndHaveAnAccountWithAnotherMobileNumber);
            registerResult.Data.Should().NotBeNull();
            
            // Authenticate Arrange
            var authenticateCommand = CommandHelper.GetUserAuthenticationRequest_Constant(registerResult.Data!.Token!);

            // Authenticate Act
            var authenticateResponse = await requestHelper.PostAsync("/v2/InternalIdentity/Authenticate", authenticateCommand,
                TestConstants.AdditionalHeaders29, CancellationToken.None);

            // Authenticate Assert
            authenticateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var authenticateResult = await RequestHelper.ReadContentTask<BaseMessageResponse<CurrentUserModel>>(authenticateResponse, CancellationToken.None);
            authenticateResult.IsSuccess.Should().BeTrue();
            authenticateResult.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
            authenticateResult.Data.Should().NotBeNull();
        });
    }
}