using System;
using System.Threading.Tasks;
using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;



namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceSkillController : Controller
    {
        private IResourceSkillService _resourceSkillService;

        public ResourceSkillController(IResourceSkillService resourceSkillService)
        {
            _resourceSkillService = resourceSkillService;
        }

      
        /// <summary>
        /// It is used to update the Resourceskill.
        /// </summary>
        /// <param name="resourceSkillModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateResourceSkill(ResourceSkillRequestModel resourceSkillRequestModel)
        {
            try
            {
                var response = await _resourceSkillService.UpdateResourceSkill(resourceSkillRequestModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to insert the Resourceskill.
        /// </summary>
        /// <param name="resourceSkillModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> InsertResourceSkill(ResourceSkillRequestModel resourceSkillRequestModel)
        {
            try
            {
                var response = await _resourceSkillService.InsertResourceSkill(resourceSkillRequestModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to get the resource skills.
        /// </summary>
        /// <param SkillId="skillId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllResourceSkills")]
        public async Task<IActionResult> GetResourceSkills()
        {
            try
            {
                var response = await _resourceSkillService.GetAllResourceSkills();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to update the Resourceskill.
        /// </summary>
        /// <param ResourceId="resourceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetResourceSkillsById")]
        public async Task<IActionResult> GetResourceSkillsById(int resourceId)
        {
            try
            {
                var response = await _resourceSkillService.GetResourceSkillsById(resourceId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to search the resources by its skill.
        /// </summary>
        /// <param name="searchSkillModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> GetResourcesBySkill(SearchSkill searchSkillModel)
        {
            try
            {
                var response = await _resourceSkillService.GetResourcesBySkill(searchSkillModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This method updates the approval status of a resource skill.
        /// </summary>
        /// <param name="updateApproval"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateApprovalStatus(UpdateApprovalStatusRequestModel updateApproval)
        {
            try
            {
                var response = await _resourceSkillService.UpdateApprovalStatus(updateApproval);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This method checks if the skills of a resource have been updated, including approval or rejection status.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> CheckResourceSkillsUpdated(int resourceId)
        {
            try
            {
                var response = await _resourceSkillService.CheckResourceSkillsUpdated(resourceId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// This method marks the specified skills and subskills as inactive for a resource.
        /// </summary>
        /// <param name="resourceSkillRequestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> MarkResourceSkillAsInactive(ResourceSkillRequestModel resourceSkillRequestModel)
        {
            try
            {
                var response = await _resourceSkillService.MarkResourceSkillAsInactive(resourceSkillRequestModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
