using System;
using System.Threading.Tasks;
using DF_EvolutionAPI.Utils;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceFunctionController : ControllerBase
    {
        private IResourceFunctionService _resourceFunctionService;
        private readonly ILogger<ResourceFunctionController> _logger;

        public ResourceFunctionController(IResourceFunctionService resourceFunctionService, ILogger<ResourceFunctionController> logger)
        {
            _resourceFunctionService = resourceFunctionService;
            _logger = logger;
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
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var functions = await _resourceFunctionService.GetAllFunctions();
                return Ok(functions);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
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
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var client = await _resourceFunctionService.GetFunctionById(functionId);

                if (client == null) return NotFound();

                return Ok(client);
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
