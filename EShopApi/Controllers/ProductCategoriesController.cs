using EShopModels;
using EShopModels.Common;
using EShopModels.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    ///[Authorize]
    public class ProductCategoriesController : ControllerBase
    { 
        private readonly IProductCategoryService _productCategoryService;
        private readonly ILogger<ProductCategoriesController> _logger;
        public ProductCategoriesController(IProductCategoryService productCategoryService, IHttpContextAccessor httpContextAccessor,ILogger<ProductCategoriesController> logger)
        {
            _productCategoryService = productCategoryService; 
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllProductCategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProductCategories()
        {
            try
            {
                return Ok(await _productCategoryService.SelectAllAsync());
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetSearchBoxItems")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSearchBoxItems()
        {
            try
            {
                return Ok(await _productCategoryService.GetAllSearchBoxItemsAsync());
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500, ex);
            }
        }

        [HttpGet]
        [Route("GetSearchITemDetail")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSearchITemDetail(int id)
        {
            try
            {
                return Ok(await _productCategoryService.GetSearchITemDetailAsync(id));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500, ex);
            }
        }

        [HttpGet]
        [Route("GetProductCategories/{PageNumber}")]
        public async Task<IActionResult> GetProductCategories(int PageNumber)
        {
            try
            {
                return Ok(await _productCategoryService.SearchAsync(PageNumber));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Produces(typeof(ProductCategory))]
        [Route("GetProductCategory/{id}")]
        public async Task<IActionResult> GetProductCategory(int id)
        {
            try { 
            return Ok(await _productCategoryService.GetSingleAsync(filter: y=>y.ID == id));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPut]
        [Route("PutProductCategory/{id}")]
        [Produces(typeof(void))]
        public async Task<IActionResult> PutProductCategory(int id, ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                productCategory.ID = id;
                await _productCategoryService.UpdateProductCategoryAsync(productCategory);
                return StatusCode(StatusCodes.Status202Accepted, productCategory);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPost(Name = "ProductCategoryCreated")]
        [Produces(typeof(ProductCategory))]
        public async Task<IActionResult> PostProductCategory(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try { 
            await _productCategoryService.AddAsync(productCategory);
            return CreatedAtRoute("ProductCategoryCreated", new { id = productCategory.ID }, productCategory);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpDelete]
        [Route("DeleteProductCategory/{id}")]
        [Produces(typeof(ProductCategory))]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            try { 
            ProductCategory productCategory = await _productCategoryService.GetSingleAsync(filter: m => m.ID == id);
            productCategory.IsDeleted = !productCategory.IsDeleted;
            await _productCategoryService.UpdateAsync(productCategory);
            return Ok(productCategory);
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
            _logger.Log(LogLevel.Error, exception, "{Name},{USERNAME},{EMAILID},{TypeName}", new object[] { Name, USERNAME, EMAILID, this.GetType().Name });
        }
    }
}