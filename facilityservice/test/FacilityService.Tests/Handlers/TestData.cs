#nullable enable
using FacilityService.Application.Common;

namespace FacilityService.Tests.Handlers;

public static class TestData
{
    public static FileUpload Pdf(string name = "letter.pdf", int size = 2048) => new(new byte[size], name, "application/pdf");

    public static FileUpload Csv(string content, string name = "personnel.csv") =>
        new(System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(content)).ToArray(), name, "text/csv");

    public static T Ok<T>(this FacilityBaseMessageResponse<T> r)
    {
        Assert.True(r.IsSuccess, $"{r.StatusCode}: {r.Message}");
        return r.Data!;
    }

    public static void Fails<T>(this FacilityBaseMessageResponse<T> r, int status, string? messagePart = null)
    {
        Assert.Equal(status, r.StatusCode);
        if (messagePart != null) Assert.Contains(messagePart, r.Message);
    }
}
