namespace FacilityService.Domain.Exceptions;

/// <summary>نقض قاعدهٔ کسب‌وکار. پیام فارسی مستقیم به کاربر نشان داده می‌شود.</summary>
public class FacilityDomainException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;

    public static FacilityDomainException NotFound(string message) => new(message, 404);
    public static FacilityDomainException Forbidden(string message = "به این مورد دسترسی ندارید.") => new(message, 403);
    public static FacilityDomainException Conflict(string message = "این رکورد هم‌زمان توسط کاربر دیگری تغییر کرده است؛ صفحه را دوباره بارگذاری کنید.") => new(message, 409);
}
