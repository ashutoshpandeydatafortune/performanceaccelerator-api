using DF_PA_API.Services.RolesMaster;
using Microsoft.AspNetCore.Mvc;
using System;
using DF_PA_API.Models;
using DF_PA_API.Services;
using System.Threading.Tasks;

namespace DF_PA_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RolesMasterController : Controller
    {
        private IRolesMasterService _rolesMasterService;
        public RolesMasterController(IRolesMasterService rolesMasterService)
        {
            _rolesMasterService = rolesMasterService;
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
                var response = await _rolesMasterService.CreateRoleMaster(roleMaster);
                return Ok(response);
            }
            catch (Exception ex)
            {
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


