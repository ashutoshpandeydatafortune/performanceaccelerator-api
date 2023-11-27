using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceFunctionController : ControllerBase
    {
        private IResourceFunctionService _resourceFunctionService;

        public ResourceFunctionController(IResourceFunctionService resourceFunctionService)
        {
            _resourceFunctionService = resourceFunctionService;
        }

        /// <summary>
        /// get all clinets
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllFunctions()
        {
            try
            {
                var functions = await _resourceFunctionService.GetAllFunctions();
                return Ok(functions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get organization function by id
        /// </summary>
        /// <param name="functionId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/functionId")]
        public async Task<IActionResult> GetFunctionById(int functionId)
        {
            try
            {
                var client = await _resourceFunctionService.GetFunctionById(functionId);

                if (client == null) return NotFound();

                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
