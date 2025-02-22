using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace EShopApi.Filters
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        public GlobalExceptionHandlingMiddleware(RequestDelegate request, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = request;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                string USERNAME = "", EMAILID = "", Name = "";
                HttpContextAccessor contextAccessor = new();
                if (contextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    Claim claim = contextAccessor.HttpContext.User.Claims.Where(b => b.Type == "USERNAME").FirstOrDefault();
                    USERNAME = claim.Value;

                    claim = contextAccessor.HttpContext.User.Claims.Where(b => b.Type == "EMAILID").FirstOrDefault();
                    EMAILID = claim.Value;

                    Name = contextAccessor.HttpContext.User.Identity.Name;
                }
                _logger.Log(LogLevel.Error, ex, "{Name},{USERNAME},{EMAILID},{TypeName}", new object[] { Name, USERNAME, EMAILID, this.GetType().Name });
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            var response = httpContext.Response;
            string exce;
            switch (ex)
            {
                case ApplicationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    exce = "Application error occured : " + ex;
                    break;
                case FileNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    exce = "Resource not found error occured : " + ex;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    exce = "Internal server error occured : " + ex;
                    break;
            }

            var exresult = JsonSerializer.Serialize(exce);
            await httpContext.Response.WriteAsync(exresult);
        }
    }
}
