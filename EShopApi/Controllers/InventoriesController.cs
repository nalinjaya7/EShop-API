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
    public class InventoriesController : ControllerBase
    { 
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoriesController> _logger;
        public InventoriesController(IInventoryService inventoryService, IHttpContextAccessor httpContextAccessor, ILogger<InventoriesController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetInventories")]
        public async Task<IActionResult> GetInventories()
        {
            try
            {
                return Ok(await _inventoryService.GetAllAsync());
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetInventories/{PageNumber}")]
        public async Task<IActionResult> GetInventories(int PageNumber)
        {
            try
            {
                return Ok(await _inventoryService.SearchAsync(PageNumber));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [Route("GetInventoriesForSearch")]
        [HttpPost]
        public async Task<IActionResult> GetInventoriesForSearch(Inventory inventory)
        {
            try
            {
                return Ok(await _inventoryService.GetInventoriesForSearchAsync(inventory));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Produces(typeof(Inventory))]
        [Route("GetInventory/{id}")]
        public async Task<IActionResult> GetInventory(int id)
        {
            try
            {
                return Ok(await _inventoryService.GetSingleAsync(filter: m => m.ID == id));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Produces(typeof(Inventory))]
        [Route("GetInventoryByCode/{code}")]
        public async Task<IActionResult> GetInventoryByCode(string code)
        {
            try { return Ok(await _inventoryService.GetSingleAsync(filter: m => m.Code == code)); }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500, ex);
            }
        }

        [HttpPut]
        [Produces(typeof(void))]
        [Route("PutInventory/{id}")]
        public async Task<IActionResult> PutInventory(int id, Inventory inventory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _inventoryService.UpdateAsync(inventory);
                return StatusCode(StatusCodes.Status202Accepted, inventory);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPost(Name = "InventoryCreated")]
        [Produces(typeof(Inventory))]
        public async Task<IActionResult> PostInventory(Inventory inventory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _inventoryService.AddAsync(inventory);
                return CreatedAtRoute("InventoryCreated", new { id = inventory.ID }, inventory);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpDelete]
        [Route("DeleteInventory/{id}")]
        [Produces(typeof(Inventory))]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            try
            {
                Inventory inventory = await _inventoryService.GetSingleAsync(filter: m => m.ID == id);
                inventory.IsDeleted = !inventory.IsDeleted;
                await _inventoryService.UpdateAsync(inventory);
                return Ok(inventory);
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