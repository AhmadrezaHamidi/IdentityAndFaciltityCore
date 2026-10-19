using AspNetCoreRateLimit;
using IdentityService.Api.Middlewares;
using IdentityService.Service.IService.MonitoringDashboard;
using IdentityService.Service.Service.MonitoringDashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OmidBank.Common.Attribute.Swagger;
using OmidBank.Common.Middleware;
using OmidBank.Common.Mongo;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO.Compression;
using IdentityService.Application;
using OmidBank.Common.Cache;
using OmidBank.Common.ExternalThirdParty.Tosan;
using OmidBank.Common.InternalThirdParty.OmidTechAuthentication.Config;
using OmidBank.Common.InternalThirdParty.OmidTechAuthentication.Helper;
using IdentityService.Infrastructure;
using IdentityService.IoC;
using IdentityService.Infrastructure.Contexts;
using OmidBank.Common.Helper.Provider;
using static OmidBank.Common.InternalThirdParty.ShahkarService.ShahkarServiceProvider;
using static OmidBank.Common.InternalThirdParty.PostalCodeService.PostalCodeServiceProvider;
using OmidBank.Common.InternalThirdParty.SmsService.Request;
using System.Net.Http;
using IdentityService.Domain.Entity.ChangeMobileNumber;
using OmidBank.Common.Handler.Admin;
using OmidBank.Common.Helper.Minio;
using OmidBank.Common.Interface;


var builder = WebApplication.CreateBuilder(args);
const string CORSPolicy = "_ADMIN_CORS_POLICY";

// builder.AddBaseConfiguration();
builder.AddBaseConfigurationNew(true);
builder.AddTosanConfiguration();

builder.AddOmidTechAuthenticationConfiguration(builder.Configuration);
// builder.AddOmidTechBaseConfiguration();


//var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
//builder.WebHost.UseSentry();
builder.Services.AddMemoryCache();
builder.Services.AddRedisConfiguration(builder.Configuration);
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<ProviderOptions>(
    builder.Configuration.GetSection("SabtAhvalProviders"));

builder.Services.Configure<ShahkarProviderOptions>(
    builder.Configuration.GetSection("ShahkarProviders"));

builder.Services.Configure<PostalProviderOptions>(
    builder.Configuration.GetSection("PostalProviders"));

builder.Services.Configure<SmsProviderOptions>(
    builder.Configuration.GetSection("SmsProviders"));

builder.Services.Configure<JwtSettings>
    (builder.Configuration.GetSection("JwtSettings"));

builder.Services.Configure<ChangeMobileNumberOptions>(
    builder.Configuration.GetSection("ChangeMobileNumber"));

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();
builder.Services.InfrastructureRegisterServices(builder.Configuration).AddApplicationService(builder.Configuration);

//اضافه کردن کورس به روش اکستنشن - میشه تو آی او سی هم گذاشت
builder.Services.AddCors(options =>
{
    options.AddPolicy(CORSPolicy, builder => builder
            //.SetIsOriginAllowed(origin => true) //یا WithOrigins("https://mysite.com")
            .AllowAnyOrigin() // با AllowCredentials استفاده بشود خطا میدهد
            .AllowAnyHeader()
            .AllowAnyMethod()
        //.AllowCredentials() //خطاهای سیگنال آر رو رفع میکنه
    );
});
builder.Services.AddAutoMapper(
    typeof(IdentityService.Services.Helper.AutoMapper), 
    typeof(IdentityService.Infrastructure.Entities.Mapping.ActivationCodeProfile),
    typeof(IdentityService.Application.Handler.Identities.Mapping.IdentityProfile));

builder.Services.AddScoped<IMonitoringDashboardService, MonitoringDashboardService>();

//فشرده کردن دیتای ارسالی به کلاینت
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.SmallestSize; });
builder.Services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });

//اضافه کردن کنترلر و ای پی آی
builder.Services.AddControllers(options =>
    {
        //تنظیمات کش در سمت کلاینت
        // این کش با زمان کش در ردیس هم پوشانی دارند یعنی اگر یکیش فعال باشه اون یکی مهم نیست فعال باشه یا نه
        options.CacheProfiles.Add("default",
            new
                CacheProfile() //بالای هر کنترلر که میخواهیم کش شود کلمه [ResponseCache(CacheProfileName = "default")] میگذاریم
                {
                    Duration = 1, //زمان کش 10 ثانیه
                    Location = ResponseCacheLocation.Any //کش ذدر کجا ذخیره شود
                });
    })
    .AddJsonOptions(options =>
    {
        //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(null, false));//برای اینکه لیست متغیرهای انوم را در سواگر به صورت رشته متنی نمایش دهد
        //options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;//برای کمل کیس کردن در تبدیل جیسون ها ولی کار نمیکنه باید داخل هر متد گذاشتش
        //options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        //options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        //options.JsonSerializerOptions.WriteIndented = true;
    });

//builder.Services.ConfigureHttpJsonOptions(options =>
//{
//    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//    options.SerializerOptions.WriteIndented = true;
//});

builder.Services.AddRazorPages();
builder.Services.AddMvcCore().AddApiExplorer();
builder.Services.AddHttpContextAccessor();

const int LimitSize = 1048576;
//افزایش حجم رکوئست
builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = LimitSize; });
builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = LimitSize; });
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = LimitSize;
    options.MultipartBodyLengthLimit = LimitSize;
    options.MultipartHeadersLengthLimit = LimitSize;
});

//ثبت سرویس ها
DependencyContainer.RegisterServices(builder.Services);

builder.Services.AddTransient<IRequestHelper, RequestHelper>();
builder.Services.AddTransient<IEncryption, Encryption>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.AddTransient<ISmsThrottleService, SmsThrottleService>();
builder.Services.AddScoped<IMongoDBContext, MongoIdentityContext>();
builder.Services.AddScoped<IAdminHandlerUtility, AdminHandlerUtility>();


builder.Services.Configure<MediaCenterServerSettingsViewModel>(builder.Configuration.GetSection("MediaCenterServerSettings"));
builder.Services.Configure<MongoSettings>(options =>
{
    options.Connection = builder.Configuration.GetSection("MongoIdentityServiceSettings:Connection").Value;
    options.DatabaseName = builder.Configuration.GetSection("MongoIdentityServiceSettings:DatabaseName").Value;
});

//ارسال اطلاعات ورژن های ای پی آی در هدر و ریسلالت 
builder.Services.AddApiVersioning(opt =>
{
    opt.ErrorResponses = new CustomErrorResponseProvider();
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new MediaTypeApiVersionReader("x-api-version"));
});
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

//اضافه کردن تنظیمات سواگر
builder.Services.AddSwaggerGen(c =>
{
    //برا رفع مشکل تداخل ورژن در ای پی آی ها
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    //نمایش فایل ایکس ام ال راهنمای فارسی در سواگر
    //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Pipeline.Documention.xml"), true);
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity.WebAPI", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "Identity.WebAPI", Version = "v2" });
    c.SwaggerDoc("v3", new OpenApiInfo { Title = "Identity.WebAPI", Version = "v3" });

    //برای نمایش ورژنینگ
    c.DocInclusionPredicate((doc, apiDescription) =>
    {
        if (!apiDescription.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

        var version = methodInfo.DeclaringType
            .GetCustomAttributes<ApiVersionAttribute>(true)
            .SelectMany(attr => attr.Versions);

        return version.Any(v => $"v{v}" == doc);
    });

    //برای آتنتیکیشن در داخل سواگر
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Bearer",
        BearerFormat = "JWT",
        Scheme = "bearer",
        Description = "Specify the authorization token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    c.EnableAnnotations();
    c.OperationFilter<DefaultHeaderParameter>();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // اجبار مرورگر به HTTPS برای دفعات بعد
}

app.UseHttpsRedirection(); 

// اضافه کردن هدرهای امنیتی
app.Use(async (context, next) =>
{
    // برای جلوگیری از هدر تکراری در پاسخ‌های خطا یا ریدایرکت
    context.Response.OnStarting(() =>
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'";
        }
        else
        {
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; script-src 'self'; object-src 'none'";
            context.Response.Headers["X-Frame-Options"] = "DENY";
        }

        context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";

        return Task.CompletedTask;
    });

    await next();
});
//app.UseMiddleware<RequestResponseLoggingMiddleware>();


var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();


//فعال سازی سواگر و ساخت فایل جیسون
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.ToLower() == "local")
{
    app.AddSwaggerBaseConfiguration();
    app.UseSwagger();
    //تبدیل جیسون به یو آی
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"../swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}
else
{
    app.UseMiddleware<ExceptionMiddleware>();
}


app.UseForwardedHeaders();

//فعال سازی روتینگ
//تمام مسیرهای که در مپ کنترل تعیین شده را اسکن میکند و با رکوئست دریافتی مقایسه میکند
app.UseRouting();

app.UseIpRateLimiting();

//فعال سازی کورس
app.UseCors(policyName: CORSPolicy);

app.UseForwardedHeaders();

app.UseForwardedHeaders(new ForwardedHeadersOptions() { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.All });

//فعال سازی فشرده کردن دیتای ارسالی به کلاینت
app.UseResponseCompression();

//مدیریت خطاها
app.UseCustomExceptionHandler();

app.UseAuthorization();

//فعال سازی کش سمت کلاینت
app.UseResponseCaching();

// ✅ لاگ بعد از اجرای Controller
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

await app.InitialiseDatabaseAsync();

app.Run();

public partial class Program { }