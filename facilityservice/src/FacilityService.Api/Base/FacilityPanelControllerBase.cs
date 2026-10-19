using FacilityService.Api.Common;
using FacilityService.Application.Authorization;
using FacilityService.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace FacilityService.Api.Base;

/// <summary>پایهٔ کنترلرهای پنل: دانلود فایل و چک مجوزِ وابسته به ورودی (مثل تأیید یا رد).</summary>
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public abstract class FacilityPanelControllerBase(ILogger logger) : FacilityBaseController(logger)
{
    protected FacilityPanelUser CurrentUser => HttpContext.RequestServices.GetRequiredService<IFacilityCurrentUser>().Value;

    /// <summary>وقتی کلید لازم به ورودی بستگی دارد (Decision = Approve یا Reject).</summary>
    protected IActionResult? RequireAnywhere(string key) =>
        CurrentUser.Permissions.HasAnywhere(key)
            ? null
            : StatusCode(StatusCodes.Status403Forbidden, new { statusCode = 403, message = "به این عملیات دسترسی ندارید." });

    protected IActionResult FileResult(FacilityBaseMessageResponse<FileUpload> response) =>
        response is { IsSuccess: true, Data: { } file }
            ? File(file.Content, string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType, file.FileName)
            : HandleResult(response);
}
