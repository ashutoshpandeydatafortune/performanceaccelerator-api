using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuarterController : ControllerBase
    {
        private IQuarterService _quarterService;

        public QuarterController(IQuarterService quarterService)
        {
            _quarterService = quarterService;
        }

        /// <summary>
        /// get all quarter list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{type?}")]      // null or current
        public async Task<IActionResult> GetAllQuarterList(string type)
        {
            try
            {
                var quarters = await _quarterService.GetAllQuarterList(type);
                return Ok(quarters);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get user quarter by id
        /// </summary>
        /// <param name="quarterId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/quarterId")]
        public async Task<IActionResult> GetQuarterDetailsById(int quarterId)
        {
            try
            {
                var quarterDetails = await _quarterService.GetQuarterDetailsById(quarterId);

                if (quarterDetails == null) return NotFound();

                return Ok(quarterDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create or Update User quarter details 
        /// </summary>
        /// <param name="quarterdetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateorUpdateQuarter(QuarterDetails quarterdetailsModel)
        {
            try
            {
                var response = await _quarterService.CreateorUpdateQuarter(quarterdetailsModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// delete quarter
        /// </summary>
        /// <param name="quarterId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteQuarter(int quarterId)
        {
            try
            {
                var response = await _quarterService.DeleteQuarter(quarterId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get Status Details By QuarterId
        /// </summary>
        /// <param name="quarterId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetStatusDetailsByQuarterId(int quarterId)
        {
            try
            {
                var model = await _quarterService.GetStatusDetailsByQuarterId(quarterId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get Quarter Details By Status Id
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetQuarterDetailsByStatusId(int statusId)
        {
            try
            {
                var model = await _quarterService.GetStatusDetailsByQuarterId(statusId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

