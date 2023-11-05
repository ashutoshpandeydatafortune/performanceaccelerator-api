using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleMappingController : Controller
    {
        IRoleMappingService _roleMappingService;
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
        public IActionResult GetAllRoleMappingList()
        {
            try
            {
                var rolesmappinglist = _roleMappingService.GetAllRoleMappingList();
                if (rolesmappinglist.Result == null) return NotFound();
                return Ok(rolesmappinglist.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get role mapping by id
        /// </summary>
        /// <param name="rolesMappingId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/rolesMappingId")]
        public IActionResult GetRoleMappingById(int rolesMappingId)
        {
            try
            {
                var rolesMapping = _roleMappingService.GetRoleMappingByRoleId(rolesMappingId);
                if (rolesMapping.Result == null) return NotFound();
                return Ok(rolesMapping.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Create or update role mapping
        /// </summary>
        /// <param name="roleMappingModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateorUpdateRoleMapping(RoleMapping rolesMappingModel)
        {
            try
            {
                var model = _roleMappingService.CreateorUpdateRoleMapping(rolesMappingModel);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// delete role mapping
        /// </summary>
        /// <param name="roleMappingId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteRoleMapping(int roleMappingId)
        {
            try
            {
                var model = _roleMappingService.DeleteRoleMapping(roleMappingId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get role mapping details by roleId
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRoleMappingbyRoleId(int roleId)
        {
            try
            {
                var model = _roleMappingService.GetRoleMappingByRoleId(roleId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get role mapping details by userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetRoleMappingbyUserId(int userId)
        {
            try
            {
                var model = _roleMappingService.GetRoleMappingByUserId(userId);
                return Ok(model.Result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
