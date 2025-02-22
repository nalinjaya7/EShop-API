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
    public class ProductSubCategoriesController : ControllerBase
    {
        private readonly IProductSubCategoryService _productSubCategoryService;
        private readonly ILogger<ProductSubCategoriesController> _logger;
        public ProductSubCategoriesController(IProductSubCategoryService productSubCategoryService, IHttpContextAccessor httpContextAccessor, ILogger<ProductSubCategoriesController> logger)
        {
            _productSubCategoryService = productSubCategoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductSubCategories()
        {
            try
            {
                return Ok(await _productSubCategoryService.GetAllAsync());
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetProductSubCategories/{PageNumber}")]
        public async Task<IActionResult> GetProductSubCategories(int PageNumber)
        {
            try
            {
                return Ok(await _productSubCategoryService.SearchAsync(PageNumber));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetSortedProductSubCategories")]
        public async Task<IActionResult> GetSortedProductSubCategories()
        {
            try
            {
                return Ok(await _productSubCategoryService.SearchAsync());
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetProductSubCategories/{PageNumber}/{CategoryID}")]
        public async Task<IActionResult> GetProductSubCategories(int PageNumber, int CategoryID)
        {
            try
            {
                return Ok(await _productSubCategoryService.GetSubCategoriesByCategoryAsync(CategoryID, PageNumber));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetSubCategoriesByCategory/{CategoryID}")]
        public async Task<IActionResult> GetSubCategoriesByCategory(int CategoryID)
        {
            try
            {
                return Ok(await _productSubCategoryService.GetSubCategoriesByCategoryAsync(CategoryID));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Produces(typeof(ProductSubCategory))]
        [Route("GetProductSubCategory/{id}")]
        public async Task<IActionResult> GetProductSubCategory(int id)
        {
            try
            {
                return Ok(await _productSubCategoryService.GetSubCategoryByID(id));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPut]
        [Produces(typeof(void))]
        [Route("PutProductSubCategory/{id}")]
        public async Task<IActionResult> PutProductSubCategory(int id, ProductSubCategory productSubCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                productSubCategory.ID = id;
                return Ok(await _productSubCategoryService.UpdateAsync(productSubCategory));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPost(Name = "ProductSubCategoryCreated")]
        [Produces(typeof(ProductSubCategory))]
        public async Task<IActionResult> PostProductSubCategory(ProductSubCategory productSubCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _productSubCategoryService.AddAsync(productSubCategory);
                return CreatedAtRoute("ProductSubCategoryCreated", new { id = productSubCategory.ID }, productSubCategory);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpDelete]
        [Route("DeleteProductSubCategory/{id}")]
        [Produces(typeof(ProductSubCategory))]
        public async Task<IActionResult> DeleteProductSubCategory(int id)
        {
            try
            {
                ProductSubCategory productSubCategory = await _productSubCategoryService.GetSingleAsync(filter: m => m.ID == id);
                productSubCategory.IsDeleted = !productSubCategory.IsDeleted;
                await _productSubCategoryService.UpdateAsync(productSubCategory);
                return Ok(productSubCategory);
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