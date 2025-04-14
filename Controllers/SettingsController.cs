using System;
using DF_PA_API.Models;
using DF_EvolutionAPI.Utils;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : Controller
    {
        private ISettingsService _settings;
        private readonly ILogger<SettingsController> _logger;
        public SettingsController(ISettingsService settings, ILogger<SettingsController> logger)
        {
            _settings = settings;
            _logger = logger;

        }

        /// <summary>
        /// get all rolemapping data for particular roleId
        /// </summary>
        /// <returns></returns>
       
        [HttpGet]
        [Route("GetPermissionsByRole/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRole(string roleId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _settings.GetPermissionsByRole(roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updating rolemapping for particular roleId
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> UpdatePermissionByRole(RoleMapping roleMapping)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _settings.UpdatePermissionByRole(roleMapping);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Inserting rolemapping for role
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> AddRoleMapping(List<RoleMapping> roleMappings)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _settings.AddRoleMapping(roleMappings);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all role
        /// </summary>
        /// <returns></returns>
        /// [HttpGet]
        [HttpGet]
        [Route("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _settings.GetAllRoles();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Create or update the role for user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public IActionResult CreateOrUpdateUserRole(UserRoles userRoles)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = _settings.CreateOrUpdateUserRole( userRoles);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all users role
        /// </summary>
        /// <returns></returns>
        /// [HttpGet]
        [HttpGet]
        [Route("GetAllUsersRole")]
        public async Task<IActionResult> GetAllUsersRole()
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _settings.GetAllUsersRole();
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get admin email id
        /// </summary>
        /// <returns></returns>
        /// [HttpGet]
        [HttpGet]
        [Route("GetAdminEmail")]
        public async Task<IActionResult> GetAdminEmail()
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var result = await _settings.GetAdminEmail();
                return Ok(result);
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
