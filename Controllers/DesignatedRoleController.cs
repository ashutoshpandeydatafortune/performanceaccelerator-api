using DF_EvolutionAPI.Services.Designations;
using DF_PA_API.Services.DesignatedRoles;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using DF_EvolutionAPI.Controllers;
using Microsoft.Extensions.Logging;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Models;

namespace DF_PA_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignatedRoleController : Controller
    {
        private IDesignatedRoleService _designatedRoleService;
        private readonly ILogger<ResourceController> _logger;

        public DesignatedRoleController(IDesignatedRoleService designatedRoleService, ILogger<ResourceController> logger)
        {
            _designatedRoleService = designatedRoleService;
            _logger = logger;
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
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);

                return Ok(await _designatedRoleService.GetAllDesignatedRoles());
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

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
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);

                return Ok(await _designatedRoleService.GetDesignatedRoleByFunctionId(functionId));
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

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
            {   // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);

                return Ok(await _designatedRoleService.GetReportingDesignatedRoles(userName));
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

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
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var resources = await _designatedRoleService.GetResourcesByDesignatedRoleReporter(designationName, resourceId);

                return Ok(resources);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

                return BadRequest(ex.Message);
            }
        }

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

        /// <summary>
        /// Get mangers reportees
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReporteesByManagerId/{resourceId}")]
        public async Task<IActionResult> GetReporteesByManagerId(int resourceId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var resources = await _designatedRoleService.GetReporteesByManagerId(resourceId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get mangers reportees by designated role
        /// </summary>
        /// <param name="resourceId" designatedRole="designationName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReporteesByDesignationRole/{resourceId}/{designationName}")]
        public async Task<IActionResult> GetReporteesByDesignationRole(int resourceId, string designationName)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var resources = await _designatedRoleService.GetReporteesByDesignationRole(resourceId, designationName);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));

                return BadRequest(ex.Message);
            }
        }
    }
}
