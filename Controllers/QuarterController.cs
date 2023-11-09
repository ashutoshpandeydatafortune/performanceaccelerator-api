using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;

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
        [Route("[action]")]
        public IActionResult GetAllQuarterList()
        {
            try
            {
                var quarters = _quarterService.GetAllQuarterList();

                return Ok(quarters);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get user quarter by id
        /// </summary>
        /// <param name="quarterId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/quarterId")]
        public IActionResult GetQuarterDetailsById(int quarterId)
        {
            try
            {
                var quarterDetails = _quarterService.GetQuarterDetailsById(quarterId);
                
                if (quarterDetails == null) return NotFound();
                
                return Ok(quarterDetails);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Create or Update User quarter details 
        /// </summary>
        /// <param name="quarterdetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateQuarter(QuarterDetails quarterdetailsModel)
        {
            try
            {
                var response = _quarterService.CreateorUpdateQuarter(quarterdetailsModel);
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// delete quarter
        /// </summary>
        /// <param name="quarterId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteQuarter(int quarterId)
        {
            try
            {
                var response = _quarterService.DeleteQuarter(quarterId);
                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get Status Details By QuarterId
        /// </summary>
        /// <param name="quarterId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetStatusDetailsByQuarterId(int quarterId)
        {
            try
            {
                var model = _quarterService.GetStatusDetailsByQuarterId(quarterId);
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get Quarter Details By Status Id
        /// </summary>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetQuarterDetailsByStatusId(int statusId)
        {
            try
            {
                var model = _quarterService.GetStatusDetailsByQuarterId(statusId);
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}

