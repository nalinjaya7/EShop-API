using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace EShopApi.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    { 
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext exceptionContext)
        {
            var error = "Something went wrong! Internal Server Error.";
            exceptionContext.HttpContext.Response.ContentType = "application/json";
            exceptionContext.HttpContext.Response.StatusCode = 500;
            exceptionContext.Result = new JsonResult(error);
            exceptionContext.ExceptionHandled = true;

            string USERNAME = ""; string EMAILID = ""; string Name = "";
            HttpContextAccessor contextAccessor = new();
            if (contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            { 
               Claim claim = contextAccessor.HttpContext.User.Claims.Where(b => b.Type == "USERNAME").FirstOrDefault();
                USERNAME = claim.Value;

                claim = contextAccessor.HttpContext.User.Claims.Where(b => b.Type == "EMAILID").FirstOrDefault();
                EMAILID = claim.Value;

                Name = contextAccessor.HttpContext.User.Identity.Name;
            }
            _logger.Log(LogLevel.Error, exceptionContext.Exception, "{Name},{USERNAME},{EMAILID},{TypeName}", new object[] { Name, USERNAME, EMAILID, this.GetType().Name });
            base.OnException(exceptionContext);
        }

        public override Task OnExceptionAsync(ExceptionContext exceptionContext)
        {
            var error = "Something went wrong! Internal Server Error.";
            exceptionContext.HttpContext.Response.ContentType = "application/json";
            exceptionContext.HttpContext.Response.StatusCode = 500;
            exceptionContext.Result = new JsonResult(error);
            exceptionContext.ExceptionHandled = true;

            string USERNAME = ""; string EMAILID = ""; string Name = "";
            HttpContextAccessor contextAccessor = new();
            if (contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            { 
               Claim claim = contextAccessor.HttpContext.User.Claims.Where(b => b.Type == "USERNAME").FirstOrDefault();
                USERNAME = claim.Value;

                claim = contextAccessor.HttpContext.User.Claims.Where(b => b.Type == "EMAILID").FirstOrDefault();
                EMAILID = claim.Value;

                Name = contextAccessor.HttpContext.User.Identity.Name;
            }
            _logger.Log(LogLevel.Error, exceptionContext.Exception , "{Name},{USERNAME},{EMAILID},{TypeName}", new object[] { Name, USERNAME, EMAILID, this.GetType().Name });
            return Task.CompletedTask;
        }
    }
}