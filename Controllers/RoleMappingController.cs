using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleMappingController : Controller
    {
        private IRoleMappingService _roleMappingService;

        public RoleMappingController(IRoleMappingService roleMappingService)
        {
            _roleMappingService = roleMappingService;
        }

        /// <summary>
        /// get all roles mapping list
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllRoleMappingList()
        {
            try
            {
                var rolesmappinglist = await _roleMappingService.GetAllRoleMappingList();
                return Ok(rolesmappinglist);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get role mapping by id
        /// </summary>
        /// <param name="rolesMappingId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/rolesMappingId")]
        public async Task<IActionResult> GetRoleMappingById(int roleId)
        {
            try
            {
                var roleMappings = await _roleMappingService.GetRoleMappingByRoleId(roleId);
                return Ok(roleMappings);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create or update role mapping
        /// </summary>
        /// <param name="roleMappingModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateorUpdateRoleMapping(RoleMapping rolesMappingModel)
        {
            try
            {
                var model = await _roleMappingService.CreateorUpdateRoleMapping(rolesMappingModel);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// delete role mapping
        /// </summary>
        /// <param name="roleMappingId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteRoleMapping(int roleMappingId)
        {
            try
            {
                var model = await _roleMappingService.DeleteRoleMapping(roleMappingId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get role mapping details by roleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetRoleMappingbyRoleId(int roleId)
        {
            try
            {
                var model = await _roleMappingService.GetRoleMappingByRoleId(roleId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get role mapping details by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetRoleMappingbyUserId(int userId)
        {
            try
            {
                var model = await _roleMappingService.GetRoleMappingByUserId(userId);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
