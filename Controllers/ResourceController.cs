using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using DF_PA_API.Models;
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
        private IResourceService _resourceService;

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
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                List<Resource> resources = await _resourceService.GetAllResourceDetailsByResourceId(resourceId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all projects of resource
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetChildResources/{userName}")]
        public async Task<IActionResult> GetChildResources(string userName)
        {
            try
            {
                string data = await _resourceService.GetChildResources(userName);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all projects of resource
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProfileDetails/{resourceId}")]
        public async Task<IActionResult> GetProfileDetails(int resourceId)
        {
            try
            {
                var resources = await _resourceService.GetProfileDetails(resourceId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all the team members of a memeber.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMyTeamDetails/{userId}")]
        public async Task<IActionResult> GetMyTeamDetails(int userId)
        {
            try
            {
                string data = await _resourceService.GetMyTeamDetails(userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all designations of particular function.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetDesignationsByFunctionId(int functionId)
        {
            try
            {
                var resources = await _resourceService.GetDesignationsByFunctionId(functionId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get all designated roles of particular function.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetDesignatedRolesByFunctionId(int functionId)
        {
            try
            {
                var resources = await _resourceService.GetDesignatedRolesByFunctionId(functionId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// get all resources kras
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetResourcesKrasStatus(SearchKraStatus searchKraStatus)
        {
            try
            {
                var resources = await _resourceService.GetResourcesKrasStatus(searchKraStatus);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// get the reportingtTo name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserManagerName(int userId)
        {
            try
            {
                var resources = await _resourceService.GetUserManagerName(userId);
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        ///<Summary>
        /// // Gets the list of resources whose evaluation is pending by the manager.
        /// </Summary>
        /// <return></return>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPendingResourceEvaluations(int? userId)
        {
            try
            {
                var resourceList = await _resourceService.GetPendingResourceEvaluations(userId);
                return Ok(resourceList);
            }
            catch ( Exception ex)
            {
                return BadRequest(ex.Message );
            }
        }

        ///<summary>
        /// // Gets the list of resources whose evaluations are completed.
        ///</summary>
        ///<return></return>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCompletedResourceEvaluation(int? userId)
        {
            try
            {
                var resourceList = await _resourceService.GetCompletedResourceEvaluations(userId);
                return Ok(resourceList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message );
            }
        }

        ///<summary>
        /// Gets the list of resources whose self-evaluation is pending.
        /// </summary>
        /// <return></return>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPendingSelfEvaluations(int? userId)
        {
            try
            {
                var resourceList = await _resourceService.GetPendingSelfEvaluations(userId);
                return Ok(resourceList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message );
            }                        
        }

    }
}
