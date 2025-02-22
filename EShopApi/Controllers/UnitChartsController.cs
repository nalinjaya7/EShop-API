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
    public class UnitChartsController : ControllerBase
    { 
        private readonly IUnitChartService _unitChartService;
        private readonly ILogger<UnitChartsController> _logger;
        public UnitChartsController(IUnitChartService unitChartService, IHttpContextAccessor httpContextAccessor, ILogger<UnitChartsController> logger)
        {
            _unitChartService = unitChartService;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetUnitCharts/{PageNumber}")]
        public async Task<IActionResult> GetUnitCharts(int PageNumber)
        {
            try { 
            return Ok(await _unitChartService.SearchAsync(PageNumber));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Route("GetUnitChartsByProduct/{ProductID}")]
        public async Task<IActionResult> GetUnitChartsByProduct(int ProductID)
        {
            try { 
            return Ok(await _unitChartService.GetUnitChartsByProductIDAsync(ProductID));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpGet]
        [Produces(typeof(UnitChart))]
        public async Task<IActionResult> GetUnitChart(int id)
        {
            try { 
            return Ok(await _unitChartService.GetSingleAsync(filter: h => h.ID == id));
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPut]
        [Route("PutUnitChart/{id}")]
        [Produces(typeof(void))]
        public async Task<IActionResult> PutUnitChart(int id, UnitChart unitChart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try { 
            unitChart.ID = id; 
            await _unitChartService.UpdateAsync(unitChart);
            return StatusCode(StatusCodes.Status202Accepted, unitChart);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [HttpPost(Name = "UnitChartCreated")]
        [Produces(typeof(UnitChart))]
        public async Task<IActionResult> PostUnitChart(UnitChart unitChart)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _unitChartService.AddAsync(unitChart);
                return CreatedAtRoute("UnitChartCreated", new { id = unitChart.ID }, unitChart);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        [Produces(typeof(void))]
        [Route("UpdateUnitChart/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUnitChart(int id, [FromBody]UnitChart[] unitcharts)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try { 
            object unitchars = await _unitChartService.UpdateUnitChartsAsync(id, unitcharts.ToList());
            return StatusCode(StatusCodes.Status202Accepted, unitchars);
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }
      
        [Route("DeleteUnitChart/{UnitChartid}/{ProductID}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUnitChart(int UnitChartid, int ProductID)
        {
            try { 
            UnitChart unitChart = await _unitChartService.GetSingleAsync(filter: m => m.ID == UnitChartid);
            unitChart.IsDeleted = !unitChart.IsDeleted;
            await _unitChartService.UpdateAsync(unitChart);

            object unitchars = await _unitChartService.GetUnitChartsByProductIDAsync(ProductID);
            return StatusCode(StatusCodes.Status202Accepted, unitchars);
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