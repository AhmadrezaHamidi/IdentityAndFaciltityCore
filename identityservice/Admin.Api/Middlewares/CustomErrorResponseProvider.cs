using Microsoft.AspNetCore.Mvc.Versioning;

namespace Admin.Api.Middlewares
{
    public class CustomErrorResponseProvider : DefaultErrorResponseProvider
    {
        public override IActionResult CreateResponse(ErrorResponseContext context)
        {
            switch (context.ErrorCode)
            {
                case "UnsupportedApiVersion":
                    context = new ErrorResponseContext(
                    context.Request,
                    500,
                    "Internal server error",
                    null,
                     context.MessageDetail);
                    break;
            }

            return base.CreateResponse(context);
        }
    }
}