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
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateResourceSkill(List<ResourceSkill> resourceSkillModel)
        {
            try
            {
                var response = await _resourceSkillService.CreateResourceSkill(resourceSkillModel);
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
        [HttpPut]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateResourceSkill(List<ResourceSkill> resourceSkillModels)
        {
            try
            {
                var response = await _resourceSkillService.UpdateResourceSkill(resourceSkillModels);
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
        [Route("[Action]")]
        public async Task<IActionResult> GetResourceSkills(int? skillId, int? subSkillId)
        {
            try
            {
                var response = await _resourceSkillService.GetResourceSkills(skillId, subSkillId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
