// stub: انواع پایهٔ موجود در پروژهٔ Facility، فقط برای کامپایل این ریپو (جزو تحویلی نیست).
// نام‌ها از کد BulkUserImport/UploadExcelCommand برداشته شده‌اند.
using MediatR;

namespace FacilityService.Application.Common
{
    public class FacilityBaseMessageResponse<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public bool IsSuccess => StatusCode is >= 200 and < 300;
    }

    public abstract record FacilityBaseCommand<TResponse> : IRequest<TResponse>;

    /// <summary>⚠ اگر پروژهٔ Facility «FacilityBaseQuery» ندارد، Queryها را از FacilityBaseCommand ارث بده.</summary>
    public abstract record FacilityBaseQuery<TResponse> : IRequest<TResponse>;

    public interface IFacilityBaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> { }

    public static class MessageResponse
    {
        public static FacilityBaseMessageResponse<T> GenerateMessageResponse<T>(int statusCode, T? data, string? message) =>
            new() { StatusCode = statusCode, Data = data, Message = message };
    }
}
