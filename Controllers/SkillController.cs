using DF_EvolutionAPI.Models;
using DF_EvolutionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace DF_EvolutionAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : Controller
    {
        private ISkillService _skillService;
       
        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        /// <summary>
        /// It is used to insert the skill.
        /// </summary>
        /// <param name="skillModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> CreateSkill(Skill skillModel)
        {
            try
            {
                var response = await _skillService.CreateSkill(skillModel);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to update the skill.
        /// </summary>
        /// <param name="skillModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[Action]")]
        public async Task<IActionResult> UpdateBySkillId(Skill skillModel)
        {
            try
            {
                var response = await _skillService.UpdateBySkillId(skillModel);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to get all the skill.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[Action]")]
        public async Task<IActionResult> GetAllSkills()
        {
            try
            {
                var response = await _skillService.GetAllSkills();
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to fetch skill by id.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSkillById/{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            try
            {
                var response = await _skillService.GetSkillById(id);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// It is used to delete the skill.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteSkillById/{id}")]
        public async Task<IActionResult> DeleteSkillById(int id)
        {
            try
            {
                var response = await _skillService.DeleteSkillById(id);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Search resources by skills
        /// </summary>
        /// <param name="skillModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> SearchBySkills(SearchSkill searchSkillModel)
        {
            try
            {
                var response = await _skillService.SearchBySkills(searchSkillModel);
                return Ok(response);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
