using EShopModels;
using EShopModels.Common;
using EShopRepository;
using EShopServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace EShopApi.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PosApiAuthenticationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext actionContext)
        {
            if (actionContext.HttpContext.Request.Headers != null)
            {               
                var authToken = actionContext.HttpContext.Request.Headers["Token"];
                var decodedAuthToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
                var emailPassword = decodedAuthToken.Split(':');
                if (IsAuthorizedUser(emailPassword[0], emailPassword[1]))
                {
                    System.Threading.Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(emailPassword[0]), null);
                }
                else
                {
                    actionContext.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                }
            }
            else
            {
                actionContext.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
        }
  
        public bool IsAuthorizedUser(string Email, string Password)
        {
            DbContextOptions<ApplicationDbContext> dbContextOptions = new();
            ApplicationDbContext dbContext = new(dbContextOptions);
            UnitOfWork unitOfWork = new(dbContext);
            EShopUserService userService = new(unitOfWork);
            EncryptDecrypt encrypt = new();
            Password = encrypt.Encryptor(Password);
            EShopUser user = userService.ValidateUser(new EShopUser(0,"","","","",Email,true,Guid.NewGuid(),"",Roles.User));
            return user != null;
        } 
    }
}