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
    public class UnitTypesController : ControllerBase
    { 
        private readonly IUnitTypeService _unitTypeService;
        private readonly ILogger<UnitTypesController> _logger;

        public UnitTypesController(IUnitTypeService unitTypeService, IHttpContextAccessor httpContextAccessor, ILogger<UnitTypesController> logger)
        { 
            _unitTypeService = unitTypeService; 
            _logger = logger;
        }


        [HttpGet]
        [Route("GetUnitTypes")]
        public async Task<IActionResult> GetUnitTypes()
        {
            try { 
            return Ok(await _unitTypeService.GetAllAsync());
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetUnitTypes/{PageNumber}")]
        public async Task<IActionResult> GetUnitTypes(int PageNumber)
        {
            try { 
            return Ok(await _unitTypeService.SearchAsync(PageNumber));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Produces(typeof(UnitType))]
        [Route("GetUnitType/{id}")]
        public async Task<IActionResult> GetUnitType(int id)
        {
            try { 
            return Ok(await _unitTypeService.GetSingleAsync(filter: n=>n.ID == id));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPut]
        [Produces(typeof(void))]
        [Route("PutUnitType/{id}")]
        public async Task<IActionResult> PutUnitType(int id, UnitType unitType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try { 
            unitType.ID = id; 
            await _unitTypeService.UpdateUnitTypeAsync(unitType);
            return StatusCode(StatusCodes.Status202Accepted, unitType);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPost(Name = "UnitTypeCreated")]
        [Produces(typeof(UnitType))]
        public async Task<IActionResult> PostUnitType(UnitType unitType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try { 
            await _unitTypeService.InsertUnitTypeAsync(unitType);
            return CreatedAtRoute("UnitTypeCreated", new { id = unitType.ID }, unitType);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpDelete]
        [Route("DeleteUnitType/{id}")]
        [Produces(typeof(UnitType))]
        public async Task<IActionResult> DeleteUnitType(int id)
        {
            try { 
            UnitType unitType = await _unitTypeService.GetSingleAsync(filter: m => m.ID == id);
            unitType.IsDeleted = !unitType.IsDeleted;
            await _unitTypeService.UpdateAsync(unitType);
            return Ok(unitType);
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