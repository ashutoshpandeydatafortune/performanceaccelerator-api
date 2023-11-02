using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        IProjectService _projectService;
        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        #region Project

        /// <summary>
        /// get all projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var projects = await _projectService.GetAllProjects();
                if (projects == null) return NotFound();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        /// <summary>
        /// get all projects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/projectId")]
        public async Task<IActionResult> GetProjectByProjectId(int projectId)
        {
            try
            {
                var projects = await _projectService.GetProjectByProjectId(projectId);
                if (projects == null) return NotFound();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }
        #endregion

        //GetProjectstatusbyProjectstatusid
        //projectleadid
        //projectmanagerid
    }
}
