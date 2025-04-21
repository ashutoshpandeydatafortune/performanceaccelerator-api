using DF_EvolutionAPI.Services.Designations;
using DF_PA_API.Services.DesignatedRoles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace DF_PA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignatedRoleController : Controller
    {
        private IDesignatedRoleService _designatedRoleService;

        public DesignatedRoleController(IDesignatedRoleService designatedRoleService)
        {
            _designatedRoleService = designatedRoleService;
        }

        /// <summary>
        /// Get all the active designated role. 
        /// </summary>
        /// <param name="designatedRoleName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllDesignatedRoles")]
        public async Task<IActionResult> GetAllDesignatedRoles()
        {
            try
            {                
                return Ok(await _designatedRoleService.GetAllDesignatedRoles());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Get all the active designated role by function id. 
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDesignatedRoleByFunctionId/{functionId}")]
        public async Task<IActionResult> GetDesignatedRoleByFunctionId(int functionId)
        {
            try
            {               
               return Ok(await _designatedRoleService.GetDesignatedRoleByFunctionId(functionId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Gets the reporting designated roles for a specified user
        /// </summary>
        /// <param name="designatedRoleName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReportingDesignatedRoles/{userName}")]
        public async Task<IActionResult> GetReportingDesignatedRoles(string userName)
        {
            try
            {                
                return Ok(await _designatedRoleService.GetReportingDesignatedRoles(userName));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get resources by designated role Name
        /// </summary>
        /// <param name="designationName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetResourcesByDesignatedRoleReporter/{designationName}/{resourceId}")]
        public async Task<IActionResult> GetResourcesByDesignatedRoleReporter(string designationName, int resourceId)
        {
            try
            {
                var resources = await _designatedRoleService.GetResourcesByDesignatedRoleReporter(designationName, resourceId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all the active designated role by businessunit id. 
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDesignatedRolesByBusinessunitId/{businessUnitId}")]
        public async Task<IActionResult> GetDesignatedRolesByBusinessunitId(int? businessUnitId)
        {
            try
            {
                return Ok(await _designatedRoleService.GetDesignatedRolesByBusinessunitId(businessUnitId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
