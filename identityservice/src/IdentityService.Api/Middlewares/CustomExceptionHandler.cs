using System.Net;
using Microsoft.AspNetCore.Builder;
using static OmidBank.Common.Helper.BaseConfiguration;

namespace IdentityService.Api.Middlewares
{
    /// <summary>
    /// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    /// </summary>
    public class CustomExceptionHandler
    {
        #region Fields
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandler> _logger;
        private readonly ElasticConfiguration elasticConfig = new ElasticConfiguration();

        #endregion


        #region Constractors
        /// <summary>
        /// CustomExceptionHandler
        /// </summary>
        /// <param name="next"></param>
        public CustomExceptionHandler(RequestDelegate next,
            ILogger<CustomExceptionHandler> logger
            , IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            configuration.GetSection("ElasticSetting").Bind(elasticConfig);
        }
        #endregion


        #region Methods
        /// <summary>
        /// اینوک
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                //request pipeline

                await _next(httpContext);

                //response pipeline
            }
            catch (Exception exp)
            {
                //لاگ کردن خطا
                if (elasticConfig.EnableFileLog)
                {
                    _logger.LogError(exception: exp, message: exp.Message + " - "
                     + httpContext.Request?.QueryString.Value + " + "
                     + httpContext.Request?.Path.Value + " * "
                     + httpContext.Request?.RouteValues);
                }
                //

                //برای اینکه جزییات خطا را برای کاربر نفرستد و نتیجه را انکپسوله کند و بفرستد
                await GetResponse(httpContext, exp);
            }
            finally
            {
                if (elasticConfig.EnableFileLog)
                {
                    _logger.LogInformation(
                            "Request {method} {url} => {statusCode}",
                    httpContext.Request?.Method,
                    httpContext.Request?.Path.Value,
                    httpContext.Response?.StatusCode);
                }
            }
        }

        private Task GetResponse(HttpContext httpContext, Exception exp)
        {
            //تنظیم پیام خطا برای ریسپانس
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;


            //switch (httpContext.Response.StatusCode)
            //{
            //    case 500:
            //        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //        break;
            //    case 600:
            //        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //        break;
            //}

            //httpContext.Response.StatusCode = ex switch
            //{
            //    //BadRequestException => StatusCodes.Status400BadRequest,
            //    NotFoundException => StatusCodes.Status404NotFound,
            //    _ => StatusCodes.Status500InternalServerError
            //};


            //تبدیل مدل ویو خطا به جیسون
            var json = JsonSerializer.Serialize(new Message
            {
                Code = 0,
                MessageString = Messages.InternalServerError,
                EnglishMessageString = Messages.EnglishInternalServerError

                //برای مخفی کردن جزییات خطا
                //Message = httpContext.Response.StatusCode.ToString()
            }, JsonHelper.JsonCamelCaseOption());

            //json = ExceptionType(httpContext, ex);

            //ارسال ریسپانس با جیسون مدل ویو
            return httpContext.Response.WriteAsync(json);
        }

        //تفکیک انواع خطا
        private string ExceptionType(HttpContext httpContext, Exception exp)
        {
            string json = string.Empty;
            //sample
            //DivideByZeroException 
            //IndexOutOfRangeException 
            //ArgumentOutOfRangeException 
            if (exp.GetType() == typeof(ThreadAbortException))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                json = JsonSerializer.Serialize(new Message
                {
                    Code = 0,
                    MessageString = Messages.InternalServerError,
                    EnglishMessageString = Messages.EnglishInternalServerError
                }
                , JsonHelper.JsonCamelCaseOption());
            }
            return json;
        }
        #endregion
    }

    /// <summary>
    /// Extension method used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class CustomExceptionHandlerExtensions
    {
        /// <summary>
        /// UseCustomExceptionHandler
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandler>();
        }
    }
}
