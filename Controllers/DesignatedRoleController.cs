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
        /// Get all the active desinations. 
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
        /// Get all the active desinations by function id. 
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDesignatedRoleByFunctionId/{functionId}")]
        public async Task<IActionResult> GetDesignatedRoleByFunctionId(int functionId)
        {
            try
            {
                var resource = await _designatedRoleService.GetDesignatedRoleByFunctionId(functionId);
               return Ok(resource);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
