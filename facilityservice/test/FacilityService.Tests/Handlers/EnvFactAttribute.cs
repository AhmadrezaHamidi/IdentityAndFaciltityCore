namespace FacilityService.Tests.Handlers;

/// <summary>تستی که به سرویس خارجی نیاز دارد؛ بدون متغیر محیطی Skip می‌شود (نه Pass) تا گزارش گمراه‌کننده نباشد.</summary>
public sealed class EnvFactAttribute : FactAttribute
{
    public EnvFactAttribute(string variable)
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(variable)))
            Skip = $"متغیر محیطی {variable} تنظیم نشده است.";
    }
}
