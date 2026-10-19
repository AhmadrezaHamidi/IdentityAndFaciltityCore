namespace IdentityService.Test.Common.Constants;

public static class TestConstants
{
    // قبلاً یک رشتهٔ ثابت به شکل sk_live_... بود. GitHub آن را کلید واقعی تشخیص می‌داد و
    // push را می‌بست؛ هیچ تستی هم به مقدار خاصش وابسته نبود.
    public static string UserToken { get; } = Guid.NewGuid().ToString();
    public static string TosanUserToken { get; } = Guid.NewGuid().ToString();
    public static string TosanSystemToken { get; } = Guid.NewGuid().ToString();
    public static Dictionary<string, string> AdditionalHeaders28 { get; } = new()
    {
        ["DeviceId"] = "09121111111",
        ["DeviceVersion"] = "2.8.0",
        ["AppLanguage"] = "fa",
        ["Authorization"] = $"Bearer {UserToken}",
        ["DeviceTypeId"] = "9",
        ["ApiKeyInternal"] = "586497ca-1a1d-41c4-bef1-3b9d0d08ccd0",
    };
    
    public static Dictionary<string, string> AdditionalHeaders29 { get; } = new()
    {
        ["DeviceId"] = "09121111111",
        ["DeviceVersion"] = "2.9.0",
        ["AppLanguage"] = "fa",
        ["Authorization"] = $"Bearer {UserToken}",
        ["DeviceTypeId"] = "9",
        ["ApiKeyInternal"] = "586497ca-1a1d-41c4-bef1-3b9d0d08ccd0",
    };
    
    public static Dictionary<string, string> AdditionalHeaders27 { get; } = new()
    {
        ["DeviceId"] = "09121111111",
        ["DeviceVersion"] = "2.7",
        ["AppLanguage"] = "fa",
        ["Authorization"] = $"Bearer {UserToken}",
        ["DeviceTypeId"] = "9",
        ["ApiKeyInternal"] = "586497ca-1a1d-41c4-bef1-3b9d0d08ccd0",
    };
}