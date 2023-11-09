using DF_EvolutionAPI.Services.Designations;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public IActionResult GetDesignationDetails(string designationName)
        {
            try
            {
                var designation = _designationService.GetDesignationDetailsByDesignationName(designationName);

                if (designation == null) return NotFound();
                
                return Ok(designation);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get resources by designation Name
        /// </summary>
        /// <param name="designationName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetResourcesByDesignationName/{designationName}")]
        public IActionResult GetResourcesByDesignationName(string designationName)
        {
            try
            {
                var resources = _designationService.GetResourcesByDesignationName(designationName);

                return Ok(resources);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
