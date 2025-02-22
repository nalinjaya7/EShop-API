using EShopModels;
using EShopModels.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {  
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, IHttpContextAccessor httpContextAccessor, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetProducts/{PageNumber}")]
        public async Task<IActionResult> GetProducts(int PageNumber)
        {
            try { 
            return Ok(await _productService.SearchAsync(PageNumber));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }
               
        [HttpGet]
        [Route("GetAllProducts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            try { 
            return Ok(await _productService.GetAllProductsAsync());
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetProductsByCategory")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategory(int SubCategoryID)
        {
            try { 
            return Ok(await _productService.GetProductsByCategoryAsync(SubCategoryID));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPost]
        [Route("ProductSearch/{PageNumber}")]
        [AllowAnonymous]
        public async Task<IActionResult> ProductSearch(int PageNumber)
        {
            try {        
            ValueTask<Product> content = HttpContext.Request.ReadFromJsonAsync<Product>();
            if (content.IsCompletedSuccessfully)
            {
               return  Ok(await _productService.SearchAsync(PageNumber, content.Result));
            }
            else
            {
                return BadRequest("Error");
            }
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetProducts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProducts()
        {
            try { 
            return Ok(await _productService.GetAllProductsAsync());
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetProduct/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try { 
            return Ok(await _productService.GetByIDAsync(id));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Produces(typeof(Product))]
        [AllowAnonymous]
        [Route("GetProductInventories/{ProductID}")]
        public async Task<IActionResult> GetProductInventories(int ProductID)
        {
            try { 
            return Ok(await _productService.GetProductInventoriesAsync(ProductID));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPost(Name = "ProductCreated")]
        [Produces(typeof(Product))]
        public async Task<IActionResult> PostProduct(Product product)
        {
            try { 
            product.CreatedDate = DateTime.Now;
            product.ModifiedDate = DateTime.Now;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _productService.AddAsync(product);
            return CreatedAtRoute("ProductCreated", new { id = product.ID }, product);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPut]
        [Route("PutProduct/{id}")]
        [Produces(typeof(void))]        
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try { 
            return Ok(await _productService.UpdateAsync(id, product));           
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }
 
        [HttpDelete]
        [Route("DeleteProduct/{id}")]
        [Produces(typeof(Product))]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                Product product = await _productService.GetSingleAsync(filter: m => m.ID == id);
                product.IsDeleted = !product.IsDeleted;
                await _productService.UpdateAsync(product);
                return Ok(product);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
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