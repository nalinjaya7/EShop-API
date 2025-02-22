using Microsoft.AspNetCore.Mvc;
using EShopModels.Services;

namespace EShopApi.Controllers
{
    public class HomeController : Controller
    {  
        public HomeController(IHttpContextAccessor httpContextAccessor)
        {   
        }

        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
          return View();
        }
 
    }
}
