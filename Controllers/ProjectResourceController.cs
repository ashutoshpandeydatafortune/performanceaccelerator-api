using System;
using DF_EvolutionAPI.Utils;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using DF_EvolutionAPI.Services;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectResourceController : ControllerBase
    {
        private IProjectResourceService _projectResourceService;
        private readonly ILogger<ProjectResourceController> _logger;

        public ProjectResourceController(IProjectResourceService projectResourceService, ILogger<ProjectResourceController> logger)
        {
            _projectResourceService = projectResourceService;
            _logger = logger;
        }

        #region Project Resources

        /// <summary>
        /// get all project resources
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllProjectResources()
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var projectResources = await _projectResourceService.GetAllProjectResources();
                return Ok(projectResources);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all project resources
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/resourceId")]
        public async Task<IActionResult> GetAllProjectResourcesByResourceId(int resourceId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var projectResources = await _projectResourceService.GetAllProjectResourcesByResourceId(resourceId);
                return Ok(projectResources);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all project resources by project
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/projectId")]
        public async Task<IActionResult> GetAllProjectResourcesByProjectId(int projectId)
        {
            try
            {
                // Log the API endpoint path being hit for request tracing and monitoring
                _logger.LogInformation("{{API:{Path}}}", HttpContext.Request.Path.Value);
                var projectResources = await _projectResourceService.GetAllProjectResourcesByProjectId(projectId);
                return Ok(projectResources);
            }
            catch (Exception ex)
            {
                // Log detailed error information including exception message and stack trace
                _logger.LogError(string.Format(Constant.ERROR_MESSAGE, ex.Message, ex.StackTrace));
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
