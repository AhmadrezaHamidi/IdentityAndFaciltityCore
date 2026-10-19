using System.Net;
using System.Text;
using FacilityService.Api.Authorization;
using FacilityService.Application.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace FacilityService.Tests;

public class FileRulesTests
{
    private static FileUpload File(string name, int size) => new(new byte[size], name, "application/octet-stream");

    [Fact]
    public void Validates_extension_size_and_required()
    {
        Assert.Null(FileRules.Validate(File("a.PDF", 2048), FileRules.LetterExtensions, FileRules.FiveMb, true));
        Assert.NotNull(FileRules.Validate(File("a.exe", 2048), FileRules.LetterExtensions, FileRules.FiveMb, true));
        Assert.NotNull(FileRules.Validate(File("a.pdf", 10), FileRules.LetterExtensions, FileRules.FiveMb, true));
        Assert.NotNull(FileRules.Validate(File("a.pdf", (int)FileRules.FiveMb + 1), FileRules.LetterExtensions, FileRules.FiveMb, true));
        Assert.NotNull(FileRules.Validate(null, FileRules.LetterExtensions, FileRules.FiveMb, true));
        Assert.Null(FileRules.Validate(null, FileRules.LetterExtensions, FileRules.FiveMb, false));
    }
}

public class IdentityAuthorizationClientTests
{
    private sealed class FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public int Calls;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Calls++;
            return Task.FromResult(respond(request));
        }
    }

    private static (IdentityAuthorizationClient Client, FakeHandler Handler) Create(HttpStatusCode status, string json)
    {
        var handler = new FakeHandler(_ => new HttpResponseMessage(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") });
        var http = new HttpClient(handler) { BaseAddress = new Uri("http://identity/") };
        return (new IdentityAuthorizationClient(http, new MemoryCache(new MemoryCacheOptions()), NullLogger<IdentityAuthorizationClient>.Instance), handler);
    }

    private const string Ok = """
        {"isSuccess":true,"statusCode":0,"message":"ok","data":{"panelUserId":7,"userName":"branch1","fullName":"کارشناس","userCode":"399",
         "branchCode":"150002","branchName":"شعبه بازار","permissionVersion":4,
         "grants":[{"key":"facility.companies.view","effect":1,"scopeType":1,"scopeId":"150002"}]}}
        """;

    [Fact]
    public async Task Maps_identity_response_and_caches_by_token()
    {
        var (client, handler) = Create(HttpStatusCode.OK, Ok);

        var user = await client.ResolveAsync("tok", default);
        await client.ResolveAsync("tok", default);

        Assert.Equal(7, user.PanelUserId);
        Assert.Equal("شعبه بازار", user.BranchName);
        Assert.True(user.Permissions.Can(FacilityPermissionKeys.CompaniesView, FacilityResource.OfCompany("150002", "1")));
        Assert.False(user.Permissions.Can(FacilityPermissionKeys.CompaniesView, FacilityResource.OfCompany("150003", "1")));
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public async Task Unauthorized_or_failed_body_returns_null()
    {
        Assert.Null(await Create(HttpStatusCode.Unauthorized, "{}").Client.ResolveAsync("tok", default));
        Assert.Null(await Create(HttpStatusCode.OK, """{"isSuccess":false,"message":"SessionRevoked"}""").Client.ResolveAsync("tok", default));
    }
}
