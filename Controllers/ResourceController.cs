using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        IResourceService _resourceService;
        public ResourceController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        /// <summary>
        /// get all resources
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllResources()
        {
            try
            {
                var resources = await _resourceService.GetAllResources();
                if (resources == null) return NotFound();

                return Ok(resources);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

        /// <summary>
        /// get all projects of resource
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllResourceDetailsByResourceId/{resourceId}")]
        public async Task<IActionResult> GetAllResourceDetailsByResourceId(int resourceId)
        {
            try
            {
                List<ProjectResource> projectResources = new List<ProjectResource>();
                List<Resource> resourcesDetails = new List<Resource>();
                Project projects = new Project();

                resourcesDetails = await _resourceService.GetAllResourceDetailsByResourceId(resourceId);
                if (resourcesDetails == null) return NotFound();

                return Ok(resourcesDetails);
            }
            catch (Exception ex)
            {
                var error = ex.Message.ToString();
                return BadRequest();
            }
        }

    }
}
