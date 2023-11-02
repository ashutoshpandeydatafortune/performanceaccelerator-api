using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectResourceController : ControllerBase
    {
        IProjectResourceService _projectResourceService;
        public ProjectResourceController(IProjectResourceService projectResourceService)
        {
            _projectResourceService = projectResourceService;
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
                var projectResources = await _projectResourceService.GetAllProjectResources();
                if (projectResources == null) return NotFound();
                return Ok(projectResources);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
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
                var projectResources = await _projectResourceService.GetAllProjectResourcesByResourceId(resourceId);
                if (projectResources == null) return NotFound();
                return Ok(projectResources);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
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
                var projectResources = await _projectResourceService.GetAllProjectResourcesByProjectId(projectId);
                if (projectResources == null) return NotFound();
                return Ok(projectResources);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        #endregion
    }
}
