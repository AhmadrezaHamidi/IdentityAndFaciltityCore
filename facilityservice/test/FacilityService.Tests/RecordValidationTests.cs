using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace FacilityService.Tests;

/// <summary>
/// ASP.NET Core برای record موقعیتی که از درخواست Bind می‌شود و Attribute اعتبارسنجی روی پارامتر سازنده یا Property همنامش دارد
/// خطای 500 می‌دهد (در اجرای واقعی، ورود به پنل از کار افتاده بود). قرارداد: اعتبارسنجی فقط روی init property ِ record غیرموقعیتی.
/// </summary>
public class RecordValidationTests
{
    [Fact]
    public void Positional_records_have_no_validation_attributes()
    {
        var offenders = new[] { typeof(FacilityService.Contracts.Common.FileUpload).Assembly, typeof(FacilityService.Application.Handler.Shared.FacilityPanelHandlerBase).Assembly }
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetMethod("<Clone>$") != null)
            .SelectMany(t => t.GetConstructors().SelectMany(c => c.GetParameters())
                .Select(p => (Type: t, Parameter: p, Property: t.GetProperty(p.Name!, BindingFlags.Public | BindingFlags.Instance)))
                .Where(x => x.Property != null && (x.Parameter.GetCustomAttributes<ValidationAttribute>().Any()
                                                   || x.Property.GetCustomAttributes<ValidationAttribute>(inherit: false).Any()))
                .Select(x => $"{x.Type.Name}.{x.Parameter.Name}"))
            .Distinct().ToList();
        Assert.True(offenders.Count == 0, "Attribute اعتبارسنجی روی record موقعیتی: " + string.Join("، ", offenders));
    }
}
