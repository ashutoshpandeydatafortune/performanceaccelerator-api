using System;
using DF_PA_API.Models;
using DF_EvolutionAPI.Utils;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DF_PA_API.Services.RolesMaster;

namespace DF_PA_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RolesMasterController : Controller
    {
        private IRolesMasterService _rolesMasterService;
        private readonly ILogger<RolesMasterController> _logger;
        public RolesMasterController(IRolesMasterService rolesMasterService, ILogger<RolesMasterController> logger)
        {
            _rolesMasterService = rolesMasterService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new role.
        /// </summary>
        /// <param name="roleMaster"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateRoleMaster(RoleMaster roleMaster)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var response = await _rolesMasterService.CreateRoleMaster(roleMaster);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

            /// <summary>
            /// Update an existing role.
            /// </summary>
            /// <param name="roleMaster"></param>
            /// <returns></returns>
            [HttpPut]
            [Route("[Action]")]
            public async Task<IActionResult> UpdateRoleMaster(RoleMaster roleMaster)
            {
                try
                {
                    var response = await _rolesMasterService.UpdateRoleMaster(roleMaster);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        /// <summary>
        /// Get all roles.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetAllRolesMaster()
        {
            try
            {
                var response = await _rolesMasterService.GetAllRolesMaster();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a role by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteRoleMaster/{id}")]
        public async Task<IActionResult> DeleteRoleMasterById(string id)
        {
            try
            {
                var response = await _rolesMasterService.DeleteRoleMasterById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}


