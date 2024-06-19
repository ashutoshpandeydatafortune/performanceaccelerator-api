using DF_EvolutionAPI.Services.Designations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignationController : Controller
    {
        private IDesignationService _designationService;

        public DesignationController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        /// <summary>
        /// get designation details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetDesignationDetails(string designationName)
        {
            try
            {
                var designation = await _designationService.GetDesignationDetailsByDesignationName(designationName);

                if (designation == null) return NotFound();

                return Ok(designation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get designation details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/{designationId}")]
        public async Task<IActionResult> GetDesignationById(int designationId)
        {
            try
            {
                var designation = await _designationService.GetDesignationById(designationId);

                if (designation == null) return NotFound();

                return Ok(designation);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get resources by designation Name
        /// </summary>
        /// <param name="designationName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReportingDesignations/{userName}")]
        public async Task<IActionResult> GetReportingDesignations(string userName)
        {
            try
            {
                var designations = await _designationService.GetReportingDesignations(userName);
                return Ok(designations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get resources by designation Name
        /// </summary>
        /// <param name="designationName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetResourcesByDesignationName/{designationName}")]
        public async Task<IActionResult> GetResourcesByDesignationName(string designationName)
        {
            try
            {
                var resources = await _designationService.GetResourcesByDesignationName(designationName);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get resources by designation Name
        /// </summary>
        /// <param name="designationName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetResourcesByDesignationReporter/{designationName}/{resourceId}")]
        public async Task<IActionResult> GetResourcesByDesignationReporter(string designationName, int resourceId)
        {
            try
            {
                var resources = await _designationService.GetResourcesByDesignationReporter(designationName, resourceId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all the active desinations. 
        /// </summary>
        /// <param name="designationName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllDesignations")]
        public async Task<IActionResult> GetAllDesignations()
        {
            try
            {
                var resources = await _designationService.GetAllDesignations();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Get all the active desinations by function id. 
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDesignationByFunctionId/{functionId}")]
        public async Task<IActionResult> GetDesignationByFunctionId(int functionId)
        {
            try
            {
                var resources = await _designationService.GetDesignationByFunctionId(functionId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
