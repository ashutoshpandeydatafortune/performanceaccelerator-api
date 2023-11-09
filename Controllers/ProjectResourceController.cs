using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using DF_EvolutionAPI.Services;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectResourceController : ControllerBase
    {
        private IProjectResourceService _projectResourceService;

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
                return Ok(projectResources);
            }
            catch (Exception ex)
            {
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
                var projectResources = await _projectResourceService.GetAllProjectResourcesByResourceId(resourceId);
                return Ok(projectResources);
            }
            catch (Exception ex)
            {
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
                var projectResources = await _projectResourceService.GetAllProjectResourcesByProjectId(projectId);
                return Ok(projectResources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
