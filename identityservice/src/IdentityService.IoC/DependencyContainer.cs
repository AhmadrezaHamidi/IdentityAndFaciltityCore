using IdentityService.Infrastructure.Repositories;
using IdentityService.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace IdentityService.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //type 1 (اسکراتور)
            //اسکراتور
            services.Scan(current => current
                //اسمبلی جاری
                //.FromCallingAssembly()
                //اسمبلی که بیس ریپوزیتوری داخلش هست را جستجو کن
                .FromAssemblies(typeof(BaseRepository).GetTypeInfo().Assembly)
                //کلاس هایی را رجیستر کن که در نیم اسپیس بیس ریپوزیتوری هستند
                .AddClasses(theClass => theClass.InNamespaceOf(typeof(BaseRepository)))
                //برای بدون اینترفیس ها
                .AsSelf()
                //برای اینترفیس دارها
                .AsImplementedInterfaces()
                //اضافه کردن طول عمر
                .WithScopedLifetime()

                //اسمبلی جاری
                //.FromCallingAssembly()
                //اسمبلی که بیس ریپوزیتوری داخلش هست را جستجو کن
                .FromAssemblies(typeof(BaseService).GetTypeInfo().Assembly)
                //کلاس هایی رو رجیستر کن که در نیم اسپیس بیس ریپوزیتوری هستند
                .AddClasses(theClass => theClass.InNamespaceOf(typeof(BaseService)))
                //برای بدون اینترفیس ها
                .AsSelf()
                //برای اینترفیس دارها
                .AsImplementedInterfaces()
                //اضافه کردن طول عمر
                .WithScopedLifetime()



                ////اسمبلی جاری
                ////.FromCallingAssembly()
                ////اسمبلی که بیس داخلش هست را جستجو کن
                //.FromAssemblies(typeof(BaseCommon).GetTypeInfo().Assembly)
                ////کلاس هایی رو رجیستر کن که در نیم اسپیس بیس هستند
                //.AddClasses(theClass => theClass.InNamespaceOf(typeof(BaseCommon)))
                ////.AddClasses(predicate, false)
                ////برای بدون اینترفیس ها
                //.AsSelf()
                ////برای اینترفیس دارها
                //.AsImplementedInterfaces()
                ////اضافه کردن طول عمر
                //.WithScopedLifetime()
            );
        }
    }

    public static class ConfigurationService
    {
        public static IServiceCollection AddCors(this IServiceCollection services, string CORSPolicy)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CORSPolicy, configure => configure
                    .WithOrigins("https://mysite.com")
                    .AllowCredentials() //خطاهای سیگنال آر رو رفع میکنه
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                );
            });

            return services;
        }
    }
}
