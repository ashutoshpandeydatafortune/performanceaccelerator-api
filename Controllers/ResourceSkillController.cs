using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.ComponentModel.Design;
using System.Collections.Generic;


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
        /// It is used to insert the Resourceskill.
        /// </summary>
        /// <param name="resourceSkillModel"></param>
        /// <returns></returns>
        //[HttpPost]
        //[Route("")]
        //public async Task<IActionResult> CreateResourceSkill(ResourceSkillRequestModel resourceSkillModel)
        //{
        //    try
        //    {
        //        var response = await _resourceSkillService.CreateResourceSkill(resourceSkillModel);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

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
        /// It is used to update the Resourceskill.
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
        /// It is used to update the Resourceskill.
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
        /// It is used to update the Resourceskill.
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
        /// It is used to update the Resourceskill.
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
    }
}
