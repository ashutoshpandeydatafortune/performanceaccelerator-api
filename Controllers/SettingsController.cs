using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : Controller
    {
        private ISettingsService _settings;
        public SettingsController(ISettingsService settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// get all rolemapping data for particular roleId
        /// </summary>
        /// <returns></returns>
       
        [HttpGet]
        [Route("GetPermissionByRole/{roleId}")]
        public async Task<IActionResult> GetPermissionByRole(string roleId)
        {
            try
            {
                var result = await _settings.GetPermissionByRole(roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// get all role
        /// </summary>
        /// <returns></returns>
        /// [HttpGet]
        //[HttpGet]
        //[Route("GetAllRoles")]
        //public async Task<IActionResult>GetAllRoles()
        //{
        //    try
        //    {
        //        var result = await _settings.GetAllRoles();
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    }
}
