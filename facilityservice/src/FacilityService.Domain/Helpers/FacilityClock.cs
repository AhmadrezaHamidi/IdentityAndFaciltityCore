namespace FacilityService.Domain.Helpers;

/// <summary>«امروز» بانک: تاریخ محلی تهران (برای روز انتقال اعتبار و تاریخ‌های آینده).</summary>
public static class FacilityClock
{
    private static readonly TimeZoneInfo Tehran = FindTehran();

    public static DateOnly Today(TimeProvider time) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(time.GetUtcNow(), Tehran).DateTime);

    private static TimeZoneInfo FindTehran()
    {
        foreach (var id in new[] { "Asia/Tehran", "Iran Standard Time" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }
        return TimeZoneInfo.CreateCustomTimeZone("Tehran", TimeSpan.FromMinutes(210), "Tehran", "Tehran");
    }
}
