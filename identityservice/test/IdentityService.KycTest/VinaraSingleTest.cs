using System.Net;
using FluentAssertions;
using IdentityService.Application.Handler.UserManagementKycLog;
using IdentityService.Application.Services.Kyc.Vinara.ExternalRequirement.RequestResponse;
using IdentityService.Domain.Models.Encryption;
using IdentityService.KycTest.Factories;
using IdentityService.KycTest.RequestsHelper;
using IdentityService.Test.Common.Constants;
using IdentityService.Test.Common.LocalDebugsOnly;
using IdentityService.Test.Common.Network;
using OmidBank.Common.Base;

namespace IdentityService.KycTest;

public class VinaraSingleTest(IdentityServiceIntegrationEnvironment env)
    : IClassFixture<IdentityServiceIntegrationEnvironment>
{
    private readonly HttpClient _client = env.CreateClient();

    [Fact]
    public async Task Identity_WhenWagePaid_ShouldCreateBankUser()
    {
        await RepeatUntilFail.Run(async () =>
        {
            // Arrange
            var command = new
            {
                UniqueKey = "cd43b725-3164-4436-b2fa-c2b803a4c8bf"
            };

            // Act
            var requestHelper = new RequestHelper(_client);
            var response = await requestHelper.PostAsync( "/v2/ExternalInquiryResult/RetrieveUserInfo", command,
                TestConstants.AdditionalHeaders28, CancellationToken.None);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await RequestHelper.ReadContentTask<VinaraBaseResponse<VinaraRetrieveUserInformationResponse>>(response, CancellationToken.None);
            result.Success.Should().BeTrue();
            result.Result.Should().NotBeNull();
        });
    }
}