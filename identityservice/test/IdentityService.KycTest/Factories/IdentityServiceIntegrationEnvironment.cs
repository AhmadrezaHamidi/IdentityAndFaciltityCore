using IdentityService.Domain.IRepositories;
using IdentityService.Domain.Models;
using IdentityService.Domain.Models.User;
using IdentityService.KycTest.TestContainers;
using IdentityService.Test.Common.Constants;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using OmidBank.Common.Base.Shared;
using OmidBank.Common.Cache;
using OmidBank.Common.Cache.MemoryCache;

namespace IdentityService.KycTest.Factories;

public class IdentityServiceIntegrationEnvironment : WebApplicationFactory<Program>, IAsyncLifetime
{
    private BankTestInfrastructure Infrastructure { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            var dict = new Dictionary<string, string?>
            {
                // ["ConnectionStrings:DefaultConnection"] = Env.Postgres.GetConnectionString(),
                // ["Kafka:BootstrapServers"] = Env.Kafka.GetBootstrapAddress(),
                // ["ConnectionStrings:Redis"] = Env.Redis.GetConnectionString()
            };

            config.AddInMemoryCollection(dict);
        });
        builder.ConfigureServices(services =>
        {
            // Remove the real Redis registration
            var descriptor = services.Single(d => d.ServiceType == typeof(IRedisCacheManager));
            services.Remove(descriptor);

            // Register mock instead
            var redisMock = RedisMock.Create();
            services.AddSingleton(redisMock);

            var descriptor2 = services.Single(d => d.ServiceType == typeof(IActiveTokenRepository));
            services.Remove(descriptor2);
            var mongoMock = new Mock<IActiveTokenRepository>();
            mongoMock
                .Setup(x => x.CheckTokenAsync(
                    It.Is<ActiveTokenModel>(m =>
                        m.Token == TestConstants.UserToken &&
                        m.MobileNumber == "09121111111"),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            services.AddScoped(typeof(IActiveTokenRepository), (IServiceProvider _) => mongoMock.Object);

            var descriptor3 = services.Single(d => d.ServiceType == typeof(IUserRepository));
            services.Remove(descriptor3);
            var userMock = new Mock<IUserRepository>();
            userMock
                .Setup(x => x.GetByMobileNumberAsync(It.Is<string>(m => m == "09121111111"),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockUser());
            userMock
                .Setup(x => x.GetByIdAsync(It.Is<int>(m => m == 1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockUser());
            userMock
                .Setup(x => x.GetPersonByNationalCodeAsync(It.Is<string>(m => m == "0070080090"),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateMockPerson());
            services.AddScoped(typeof(IUserRepository), (IServiceProvider _) => userMock.Object);
        });
    }

    public async Task InitializeAsync()
    {
        await Infrastructure.InitializeAsync();
    }

    public new async Task DisposeAsync()
    {
        await Infrastructure.DisposeAsync();
    }

    private static PersonModel CreateMockPerson()
    {
        return new PersonModel(5, "test", "testian", "0070080090", "09121111111");
    }

    private static UserModel CreateMockUser()
    {
        var user = new UserModel(
            id: 78,
            personId: 79,
            firstName: "Ali",
            lastName: "Ahmadi",
            mobileNumber: "09121111111",
            userName: "ali.ahmadi",
            creationTime: DateTime.Now,
            nationalCode: "0013223518",
            avatarName: "avatar.png",
            isActive: true,
            isVerified: true,
            password: "hashedPassword",
            invitedCode: "INV123",
            email: "ali@example.com",
            cityId: 1,
            myInviteCode: "MYCODE01"
        );

        user.SetRoles(new List<RoleModel>
        {
            new RoleModel(1, "Admin", "مدیر", true),
            new RoleModel(2, "User", "کاربر", true)
        });

        user.SetRoleMenuModels(new List<RoleMenuModel>
        {
            new RoleMenuModel { /* fill fields */ }
        });

        user.GeneratePanelJwtToken("mock-jwt-token");

        return user;
    }
}
