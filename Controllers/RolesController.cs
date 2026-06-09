using System;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : Controller
    {
        private IRolesService _rolesService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRolesService rolesService, ILogger<RolesController> logger)
        {
            _rolesService = rolesService;
            _logger = logger;
        }

        /// <summary>
        /// get all roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllRoleList()
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var roles = _rolesService.GetAllRoleList();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get role details by id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/roleId")]
        public IActionResult GetRoleById(int roleId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var role = _rolesService.GetRoleById(roleId);
                
                if (role == null) return NotFound();
                
                return Ok(role);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// create or update role
        /// </summary>
        /// <param name="roleModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateRole(Role roleModel)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var model = _rolesService.CreateorUpdateRole(roleModel);
                return Ok(model);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// delete role
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteRole(int roleId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var model = _rolesService.DeleteRole(roleId);
                return Ok(model);
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
