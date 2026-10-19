using FacilityService.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FacilityService.Application.Handler.Shared;

/// <summary>
/// FacilityDomainException و خطای همزمانی EF را به FacilityBaseMessageResponse با StatusCode مناسب تبدیل می‌کند
/// تا Handlerها با throw قاعده را اعلام کنند. ثبت: cfg.AddOpenBehavior(typeof(FacilityExceptionBehavior&lt;,&gt;)).
/// </summary>
public sealed class FacilityExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (FacilityDomainException ex) when (IsMessageResponse)
        {
            return Fail(ex.StatusCode, ex.Message);
        }
        catch (DbUpdateConcurrencyException) when (IsMessageResponse)
        {
            return Fail(409, FacilityDomainException.Conflict().Message);
        }
    }

    private static readonly bool IsMessageResponse =
        typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(FacilityBaseMessageResponse<>);

    private static TResponse Fail(int status, string message)
    {
        var response = Activator.CreateInstance(typeof(TResponse))!;
        typeof(TResponse).GetProperty(nameof(FacilityBaseMessageResponse<object>.StatusCode))!.SetValue(response, status);
        typeof(TResponse).GetProperty(nameof(FacilityBaseMessageResponse<object>.Message))!.SetValue(response, message);
        return (TResponse)response;
    }
}
