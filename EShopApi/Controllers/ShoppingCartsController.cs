using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EShopModels;
using EShopModels.Services;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;

namespace EShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    [Authorize]
    public class ShoppingCartsController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartItemService _shoppingCartItemService;
        private readonly ILogger<ShoppingCartsController> _logger;
        private readonly IShoppingCart _shopping;

        public ShoppingCartsController(IShoppingCartService shoppingCartService, IShoppingCart shoppingCart, IShoppingCartItemService shoppingCartItemService, ILogger<ShoppingCartsController> logger)
        {
            _shoppingCartItemService = shoppingCartItemService;
            _shoppingCartService = shoppingCartService;
            _logger = logger;
            _shopping = shoppingCart;
        }

        [HttpPost(Name = "ShoppingCartItemCreated")]
        [Produces(typeof(ShoppingCartItem))]
        public async Task<IActionResult> PostShoppingCartItem(ShoppingCartItem cartItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var UserIDObj = User.Claims.Where(f => f.Type == "EShopUserID").FirstOrDefault().Value;
                await _shoppingCartItemService.AddNewShoppingCartItemAsync(cartItem, int.Parse(UserIDObj));
                return CreatedAtRoute("ShoppingCartItemCreated", new { id = cartItem.ID }, cartItem);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500, ex);
            }

        }

        [HttpGet]
        [Route("GetShoppingCartAsync")]
        public async Task<ActionResult> GetShoppingCartAsync()
        {
            
            try
            {
                return Ok(_shopping);
                // return Ok(await _shoppingCartService.GetShoppingCartAsync(2));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500, ex);
            }

        }

        [HttpDelete]
        [Route("DeleteCartItem/{id}")]
        public async Task<ActionResult> DeleteCartItem(int id)
        {
            try
            { 
                return Ok(await _shoppingCartItemService.DeleteCartItemAsync(id));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500, ex);
            }          
        }

        [HttpPost]
        [Route("UpdateQuantity")]
        public async Task<ActionResult> UpdateQuantity([FromBody] ShoppingCartItem shoppingCart)
        {
            try
            {
                return Ok(await _shoppingCartItemService.UpdateCartItemQtyAsync(shoppingCart));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500, ex);
            }
        }
         
        private async Task ErrorMessages(Exception exception)
        { 
            string USERNAME = ""; string EMAILID = ""; string Name = ""; 
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Claim claim = HttpContext.User.Claims.Where(b => b.Type == "USERNAME").FirstOrDefault();
                USERNAME = claim.Value;

                claim = HttpContext.User.Claims.Where(b => b.Type == "EMAILID").FirstOrDefault();
                EMAILID = claim.Value;

                Name = HttpContext.User.Identity.Name;
            }
            _logger.Log(LogLevel.Error,exception, "{Name},{USERNAME},{EMAILID},{TypeName}", new object[] { Name, USERNAME, EMAILID, this.GetType().Name });
        } 
    }
}
