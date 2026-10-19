using FluentAssertions;
using IdentityService.KycTest.Factories;
using IdentityService.KycTest.RequestsHelper;
using IdentityService.Test.Common.Constants;
using IdentityService.Test.Common.LocalDebugsOnly;
using IdentityService.Test.Common.Network;
using OmidBank.Common.Base;
using System.Net;

namespace IdentityService.KycTest
{
    public class UserManagmentTest(IdentityServiceIntegrationEnvironment env)
    : IClassFixture<IdentityServiceIntegrationEnvironment>
    {
        private readonly HttpClient _client = env.CreateClient();

        [Fact]
        public async Task Identity_CheckUserManagment()
        {
            await RepeatUntilFail.Run(async () =>
            {
                // Arrange
                var command = CommandHelper.InternalCheckUserManagmentRequest_Constant();

                // Act
                var requestHelper = new RequestHelper(_client);
                var response = await requestHelper.PostAsync("/v2/InternalUserManagement/CheckUserManagment", command,
                    TestConstants.AdditionalHeaders28, CancellationToken.None);

                // Assert
                var json = await response.Content.ReadAsStringAsync();
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                var result = await RequestHelper.ReadContentTask<BaseMessageResponse<object>>(response, CancellationToken.None);
                result.IsSuccess.Should().BeTrue();
                result.StatusCode.Should().Be(EResponseStatusCode.DoneSuccessfully);
                result.Data.Should().NotBeNull();
            });
        }
    }
}
