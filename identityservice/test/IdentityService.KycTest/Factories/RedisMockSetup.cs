using IdentityService.Test.Common.Constants;
using Moq;
using OmidBank.Common.Cache;

namespace IdentityService.KycTest.Factories;

public static class RedisMock
{
    public static IRedisCacheManager Create()
    {
        var redisMock = new Mock<IRedisCacheManager>();
        redisMock.Setup(x => x.Read<object>(CacheKey.TosanSystemSessionId)).Returns(TestConstants.TosanSystemToken);
        redisMock.Setup(x => x.Read(string.Format(CacheKey.TosanUserSessionId, TestConstants.UserToken))).Returns(TestConstants.TosanUserToken);
        redisMock.Setup(x => x.Read(string.Format("MetaboomToken"))).Returns(TestConstants.TosanUserToken);
        return redisMock.Object;
    }
}