// stub: انواع موجود پروژهٔ FacilityService.Api، فقط برای کامپایل این ریپو (جزو تحویلی نیست).
// نام‌ها از BulkUserImportController برداشته شده‌اند.
using FacilityService.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FacilityService.Api.Common
{
    [ApiController]
    public abstract class FacilityBaseController(ILogger logger) : ControllerBase
    {
        protected readonly ILogger _logger = logger;
        protected IMediator _mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

        protected IActionResult HandleResult<T>(FacilityBaseMessageResponse<T> response) =>
            StatusCode(response.StatusCode == 0 ? 200 : response.StatusCode, response);
    }
}

namespace FacilityPanel.Api.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class FacilityAnonymousAttribute : System.Attribute { }
}

namespace Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ApiVersionAttribute(string version) : System.Attribute { public string Version { get; } = version; }
}
